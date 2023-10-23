using System;
using System.Drawing;
using Reactor.Platform;
using Reactor.Types;
using Reactor.Platform.WGPU;

namespace Reactor.Graphics
{
    public class Texture2D : Texture
    {
        public Texture2D()
        {
        }

        internal Texture2D(bool defaultWhite)
        {
            Create(1, 1, PixelFormat.Rgba, SurfaceFormat.Color);
            SetData(new[] { new RColor(1f, 1f, 1f, 1f) }, PixelFormat.Rgba, 0, 0, 1, 1, true);
            GPU.CheckError();
        }

        public void Create(int width, int height, PixelFormat format, SurfaceFormat surfaceFormat,
            bool multisample = false, Wgpu.TextureUsage usage = Wgpu.TextureUsage.TextureBinding)
        {
            Handle = GPU.Device.CreateTexture(null,
                usage,
                Wgpu.TextureDimension.ThreeDimensions,
                new Wgpu.Extent3D()
                {
                    depthOrArrayLayers = 1,
                    height = (uint)height,
                    width = (uint)width,
                },
                Wgpu.TextureFormat.BGRA8UnormSrgb,
                1,
                multisample ? 4U : 1U);
            Bounds = new Rectangle(0, 0, width, height);
            Id = (uint)new Random().Next(int.MaxValue);
            Filename = $"{Id}";
            Name = $"{Id}";
            textureTarget = TextureTarget.Texture2D;
            Sampler = GPU.Device.CreateSampler(null, Wgpu.AddressMode.ClampToEdge, Wgpu.AddressMode.ClampToEdge,
                Wgpu.AddressMode.ClampToEdge, Wgpu.FilterMode.Nearest, Wgpu.FilterMode.Nearest,
                Wgpu.MipmapFilterMode.Nearest, 0, 1f, Wgpu.CompareFunction.Always, 16);

            MinFilter = TextureMinFilter.Nearest;
            MagFilter = TextureMagFilter.Nearest;
            WrapS = TextureWrapMode.ClampToBorder;
            WrapR = TextureWrapMode.ClampToBorder;
            WrapT = TextureWrapMode.ClampToBorder;
            
        }

        public void CreateDepth(int width, int height, PixelFormat format, DepthFormat depthFormat)
        {
            /*GPU.Gl.GenTextures(1, out Id);
            textureTarget = TextureTarget.Texture2D;
            GPU.Gl.BindTexture(textureTarget, Id);
            GPU.CheckError();
            GPU.Gl.TexImage2D(textureTarget, 0, GetPixelInternalForDepth(depthFormat), (uint)width, (uint)height, 0,
                (PixelFormat)format, GetPixelTypeForDepth(depthFormat), IntPtr.Zero);
            GPU.CheckError();
            CreateProperties(textureTarget);
            GPU.CheckError();
            */
        }

        private PixelType GetPixelTypeForSurface(SurfaceFormat surface)
        {
            switch (surface)
            {
                case SurfaceFormat.HalfVector2:
                case SurfaceFormat.HalfVector4:
                case SurfaceFormat.HalfSingle:
                case SurfaceFormat.HdrBlendable:
                    return PixelType.HalfFloat;
                case SurfaceFormat.Single:
                case SurfaceFormat.Vector2:
                case SurfaceFormat.Vector4:
                    return PixelType.Float;
                case SurfaceFormat.Rgba64:
                    return PixelType.UnsignedInt;
                default:
                    return PixelType.UnsignedByte;
            }
        }

        private Wgpu.TextureFormat GetPixelInternalForDepth(DepthFormat depthFormat)
        {
            switch (depthFormat)
            {
                case DepthFormat.Depth16:
                    return Wgpu.TextureFormat.Depth16Unorm;
                case DepthFormat.Depth24:
                    return Wgpu.TextureFormat.Depth24Plus;
                case DepthFormat.Depth24Stencil8:
                    return Wgpu.TextureFormat.Depth24PlusStencil8;
                case DepthFormat.Depth32Stencil8:
                    return Wgpu.TextureFormat.Depth32FloatStencil8;
                default:
                    return Wgpu.TextureFormat.Stencil8;
            }
        }

        private PixelType GetPixelTypeForDepth(DepthFormat depthFormat)
        {
            switch (depthFormat)
            {
                case DepthFormat.Depth16:
                case DepthFormat.Depth24:
                    return PixelType.UnsignedInt;
                case DepthFormat.Depth24Stencil8:
                case DepthFormat.Depth32Stencil8:
                    return PixelType.Float;
                default:
                    return PixelType.UnsignedInt;
            }
        }
    }
}