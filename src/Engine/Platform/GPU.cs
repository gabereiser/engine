using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using GLFW;
using Red.Common;
using Red.Geometry;
using Red.Graphics;
using Red.Math3D;
using Red.Platform.WGPU.Wrappers;
using Red.Platform.WGPU;
using System.Threading.Tasks;
using System.Net.Http;

namespace Red.Platform
{
    public class GPU : IGraphicsContext
    {
        internal static Instance Instance;
        internal static Device Device;
        internal static Adapter Adapter;
        internal static Surface Surface;
        internal static SwapChain SwapChain;
        internal static Queue Queue;

        #region Public Static Methods

        public static void Register(Window window)
        {
            Initialize(window);
        }

        static void Initialize(Window window)
        {
            Log.Debug("Initializing Wgpu");
            Instance = new Instance();

            var hwnd = window.Hwnd;
            IntPtr hinstance = (IntPtr)window;

            CreateSurface(window);
            Log.Debug("Requesting Adapter");
            Instance.RequestAdapter(Surface, Wgpu.PowerPreference.HighPerformance, false, (status, adapter, label) =>
            {
                if (status == Wgpu.RequestAdapterStatus.Success)
                {
                    Log.Debug("Adapter Found");
                    Adapter = adapter;
                    SetupDevice();
                }
            }, Wgpu.BackendType.Vulkan);

        }
        static void SetupDevice()
        {

            Log.Debug("Initializing Device");
            Adapter.GetProperties(out var properties);

            Adapter.GetLimits(out var supportedLimits);

            RequiredLimits requiredLimits = default;
            requiredLimits.Limits = supportedLimits.limits;

            requiredLimits.Limits.maxVertexAttributes = 7; // position, normal, tangent, bi-tangent, texcoord, blendweights, blendindices
            requiredLimits.Limits.maxVertexBuffers = 7;
            requiredLimits.Limits.maxBufferSize = (ulong)(500_000U * (sizeof(float) * 4 * 7));
            requiredLimits.Limits.maxVertexBufferArrayStride = (sizeof(float) * 4 * 7);
            requiredLimits.Limits.minStorageBufferOffsetAlignment = supportedLimits.limits.minStorageBufferOffsetAlignment;
            requiredLimits.Limits.minUniformBufferOffsetAlignment = supportedLimits.limits.minUniformBufferOffsetAlignment;
            requiredLimits.Limits.maxInterStageShaderComponents = 11;
            requiredLimits.Limits.maxBindGroups = supportedLimits.limits.maxBindGroups;
            requiredLimits.Limits.maxUniformBuffersPerShaderStage = 8;
            requiredLimits.Limits.maxUniformBufferBindingSize = 16 * 12 * sizeof(float); // model + view + proj matrices.
            // Allow textures up to 8K
            requiredLimits.Limits.maxTextureDimension1D = 1024 * 8;
            requiredLimits.Limits.maxTextureDimension2D = 1024 * 8;
            requiredLimits.Limits.maxTextureArrayLayers = 16;
            requiredLimits.Limits.maxSampledTexturesPerShaderStage = 8;
            requiredLimits.Limits.maxSamplersPerShaderStage = 8;

            var queueDescriptor = new Wgpu.QueueDescriptor()
            {
                nextInChain = default,
                label = "Main Queue"
            };

            Log.Debug("Requesting Device");
            Adapter.RequestDevice((dstatus, device, dlabel) =>
            {
                if (dstatus == Wgpu.RequestDeviceStatus.Success)
                {
                    Log.Debug("Device Found");
                    Device = device;
                    Queue = Device.Queue;
                    Device.SetUncapturedErrorCallback(OnUnhandledError);
                    SetupSwapChain(Adapter, Device);
                }
            }, "Main Device", new Wgpu.NativeFeature[] { }, queueDescriptor, requiredLimits.Limits, null, null);
        }
        private static Wgpu.SwapChainDescriptor swapChainDescriptor;
        private static Wgpu.TextureDescriptor depthBufferDescriptor;
        static unsafe void SetupSwapChain(Adapter Adapter, Device Device)
        {
            Log.Debug("Initializing SwapChain");

            var preferredFormat = Surface.GetPreferredFormat(Adapter);
            Glfw.GetFramebufferSize(Application.Instance.Window, out var width, out var height);
            swapChainDescriptor = new Wgpu.SwapChainDescriptor()
            {
                format = preferredFormat,
                width = (uint)width,
                height = (uint)height,
                usage = (uint)Wgpu.TextureUsage.RenderAttachment,
                presentMode = Wgpu.PresentMode.Fifo,
                nextInChain = default,
            };

            SwapChain = Device.CreateSwapChain(Surface, swapChainDescriptor);

            var depthFormat = Wgpu.TextureFormat.Depth24Plus;

            depthBufferDescriptor = new Wgpu.TextureDescriptor()
            {
                dimension = Wgpu.TextureDimension.TwoDimensions,
                format = depthFormat,
                label = "DepthBuffer",
                sampleCount = 1U,
                size = new Wgpu.Extent3D() { depthOrArrayLayers = 1, height = swapChainDescriptor.height, width = swapChainDescriptor.width },
                mipLevelCount = 1,
                usage = (uint)Wgpu.TextureUsage.RenderAttachment,
                viewFormatCount = 1,
                viewFormats = &depthFormat,
            };
            DepthBuffer = GPU.Device.CreateTexture(depthBufferDescriptor);

            DepthBufferView = DepthBuffer.CreateTextureView();
            Log.Debug("Initialization of SwapChain Complete.");

        }

        public static void CreateSurface(Window window)
        {
            Surface = window.CreateWebGPUSurface(ref Instance);
        }

        #endregion Public Static Methods

        #region Error Handling

        public static void CheckError()
        {

        }

        static void OnUnhandledError(Wgpu.ErrorType type, string message)
        {
            if (type == Wgpu.ErrorType.NoError)
                return;
            Log.Error(message);
        }

        #endregion Error Handling

        #region IGraphicsContext Implementation

        public void Dispose()
        {
            unsafe
            {
                Device.Dispose();
                SwapChain.Dispose();
                Surface.Dispose();
            }

        }
        void IGraphicsContext.CheckError()
        {
            GPU.CheckError();
        }

        public void Clear()
        {

        }

        private static RenderPass renderPass;
        internal static WGPU.Wrappers.Texture DepthBuffer { get; set; }
        internal static TextureView DepthBufferView { get; private set; }
        internal static TextureView BackBuffer { get; set; }
        public void Begin()
        {
            BackBuffer = SwapChain.GetCurrentTextureView();
            if (BackBuffer == null)
            {
                return;
            }

            renderPass = new RenderPass();
            renderPass.Begin();
        }

        public void End()
        {
            renderPass?.End();
            renderPass?.Dispose();
            renderPass = null;
        }
        public void Present()
        {
            SwapChain?.Present();

            BackBuffer?.Dispose();
            BackBuffer = null;
        }

        public void Resize()
        {
            DepthBufferView?.Dispose();
            DepthBufferView = null;
            DepthBuffer?.Dispose();
            DepthBuffer = null;
            BackBuffer?.Dispose();

            renderPass?.Dispose();

            SwapChain?.Dispose();
            Surface.Dispose();
            Surface = Application.Instance.Window.CreateWebGPUSurface(ref Instance);
            SetupSwapChain(Adapter, Device);
        }
        #endregion IGraphicsContext Implementation


        public static void ProcessEvents()
        {
            Glfw.PollEvents();
            //Instance.ProcessEvents();
        }
    }
}