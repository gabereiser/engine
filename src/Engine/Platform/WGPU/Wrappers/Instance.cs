using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Red.Platform.WGPU.Helpers;

using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public class Instance : IDisposable
    {
        private InstanceImpl _impl;

        public Instance()
        {
            _impl = CreateInstance(new InstanceDescriptor());
        }

        public Surface CreateSurfaceFromAndroidNativeWindow(IntPtr window, string label = default)
        {
            unsafe
            {
                return new Surface(InstanceCreateSurface(_impl, new SurfaceDescriptor()
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                    .AddSurfaceDescriptorFromAndroidNativeWindow(window)
                    .GetPointer()
                }));
            }
        }

        public Surface CreateSurfaceFromCanvasHTMLSelector(string selector, string label = default)
        {
            unsafe
            {
                return new Surface(InstanceCreateSurface(_impl, new SurfaceDescriptor()
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                    .AddSurfaceDescriptorFromCanvasHTMLSelector(selector)
                    .GetPointer()
                }));
            }
        }

        public Surface CreateSurfaceFromMetalLayer(IntPtr layer, string label = default)
        {
            unsafe
            {
                return new Surface(InstanceCreateSurface(_impl, new SurfaceDescriptor()
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                    .AddSurfaceDescriptorFromMetalLayer(layer)
                    .GetPointer()
                }));
            }
        }

        public Surface CreateSurfaceFromWaylandSurface(IntPtr display, string label = default)
        {
            unsafe
            {
                return new Surface(InstanceCreateSurface(_impl, new SurfaceDescriptor()
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                    .AddSurfaceDescriptorFromWaylandSurface(display)
                    .GetPointer()
                }));
            }
        }

        public Surface CreateSurfaceFromWindowsHWND(IntPtr hinstance, IntPtr hwnd, string label = default)
        {
            unsafe
            {
                return new Surface(InstanceCreateSurface(_impl, new SurfaceDescriptor()
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                    .AddSurfaceDescriptorFromWindowsHWND(hinstance, hwnd)
                    .GetPointer()
                }));
            }
        }

        public Surface CreateSurfaceFromXcbWindow(IntPtr connection, uint window, string label = default)
        {
            unsafe
            {
                return new Surface(InstanceCreateSurface(_impl, new SurfaceDescriptor()
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                    .AddSurfaceDescriptorFromXcbWindow(connection, window)
                    .GetPointer()
                }));
            }
        }

        public Surface CreateSurfaceFromXlibWindow(IntPtr display, uint window, string label = default)
        {
            unsafe
            {
                return new Surface(InstanceCreateSurface(_impl, new SurfaceDescriptor()
                {
                    label = label,
                    nextInChain = (ChainedStruct*)new WgpuStructChain()
                    .AddSurfaceDescriptorFromXlibWindow(display, window)
                    .GetPointer()
                }));
            }
        }

        public void ProcessEvents() => InstanceProcessEvents(_impl);

        public void RequestAdapter(Surface compatibleSurface, PowerPreference powerPreference, bool forceFallbackAdapter, RequestAdapterCallback callback, BackendType backendType)
        {
            var context = new CallbackContext<RequestAdapterStatus, AdapterImpl>
            {
                Delegate = (s, p, m, _) => callback?.Invoke(s, new Adapter(p), m),
                UserData = IntPtr.Zero,
            };
            var handle = GCHandle.Alloc(context);
            try
            {
                unsafe
                {
                    InstanceRequestAdapter(_impl,
                        new RequestAdapterOptions()
                        {
                            compatibleSurface = compatibleSurface.Impl,
                            powerPreference = powerPreference,
                            forceFallbackAdapter = forceFallbackAdapter,
                            backendType = backendType
                        },
                        &RequestAdapterCallback, (void*)GCHandle.ToIntPtr(handle));
                }
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void RequestAdapterCallback(RequestAdapterStatus status, AdapterImpl impl, byte* message, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (CallbackContext<RequestAdapterStatus, AdapterImpl>)handle.Target;
                string messageStr = (message == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(message, int.MaxValue).Slice(0, (int)Util.StrLen(message)));
                context.Delegate?.Invoke(status, impl, messageStr, context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        public void Dispose()
        {
            InstanceRelease(_impl);
            _impl = default;
        }
    }

    public delegate void RequestAdapterCallback(RequestAdapterStatus status, Adapter adapter, string message);
}
