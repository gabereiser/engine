using System;
using Reactor.Types;

namespace Reactor.Graphics
{
    public class TextureCube : Texture
    {
        public Texture NegativeX { get; }
        public Texture NegativeY { get; }
        public Texture NegativeZ { get; }
        public Texture PositiveX { get; }
        public Texture PositiveY { get; }
        public Texture PositiveZ { get; }

        //private static uint[] uint1 = new uint[] { 0 };
        public void Create(PixelFormat format, ref Texture2D posX, ref Texture2D posY, ref Texture2D posZ,
            ref Texture2D negX, ref Texture2D negY, ref Texture2D negZ)
        {
            /*var inf = PixelFormat.Rgba;
            var pf = PixelFormat.Rgba;
            if (format == PixelFormat.Bgr)
            {
                inf = PixelFormat.Rgb;
                pf = PixelFormat.Bgr;
            }

            if (format == PixelFormat.Rgb)
            {
                inf = PixelFormat.Rgb;
                pf = PixelFormat.Rgb;
            }

            if (format == PixelFormat.Bgra)
            {
                inf = PixelFormat.Rgba;
                pf = PixelFormat.Bgra;
            }

            if (format == PixelFormat.Rgba)
            {
                inf = PixelFormat.Rgba;
                pf = PixelFormat.Rgba;
            }

            GPU.Gl.ActiveTexture(TextureUnit.Texture0);
            var posXcolors = posX.GetData<RColor>();
            var posYcolors = posY.GetData<RColor>();
            var posZcolors = posZ.GetData<RColor>();
            var negXcolors = negX.GetData<RColor>();
            var negYcolors = negY.GetData<RColor>();
            var negZcolors = negZ.GetData<RColor>();

            Bounds = posX.Bounds;
            Id = GPU.Gl.GenTexture();
            textureTarget = TextureTarget.TextureCubeMap;
            GPU.Gl.BindTexture(textureTarget, Id);
            SetTextureMagFilter(TextureMagFilter.Linear);
            SetTextureMinFilter(TextureMinFilter.Linear);
            SetTextureWrapMode(TextureWrapMode.ClampToBorder, TextureWrapMode.ClampToBorder);
            unsafe
            {
                fixed (RColor* ptr = &posXcolors[0])
                {
                    var p = new IntPtr(ptr);
                    GPU.Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 0, inf, (uint)Bounds.Width, (uint)Bounds.Height, 0, pf,
                        PixelType.UnsignedByte, p);
                }

                fixed (RColor* ptr = &posYcolors[0])
                {
                    var p = new IntPtr(ptr);
                    GPU.Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveY, 0, inf, (uint)Bounds.Width, (uint)Bounds.Height, 0, pf,
                        PixelType.UnsignedByte, p);
                }

                fixed (RColor* ptr = &posZcolors[0])
                {
                    var p = new IntPtr(ptr);
                    GPU.Gl.TexImage2D(TextureTarget.TextureCubeMapPositiveZ, 0, inf, (uint)Bounds.Width, (uint)Bounds.Height, 0, pf,
                        PixelType.UnsignedByte, p);
                }

                fixed (RColor* ptr = &negXcolors[0])
                {
                    var p = new IntPtr(ptr);
                    GPU.Gl.TexImage2D(TextureTarget.TextureCubeMapNegativeX, 0, inf, (uint)Bounds.Width, (uint)Bounds.Height, 0, pf,
                        PixelType.UnsignedByte, p);
                }

                fixed (RColor* ptr = &negYcolors[0])
                {
                    var p = new IntPtr(ptr);
                    GPU.Gl.TexImage2D(TextureTarget.TextureCubeMapNegativeY, 0, inf, (uint)Bounds.Width, (uint)Bounds.Height, 0, pf,
                        PixelType.UnsignedByte, p);
                }

                fixed (RColor* ptr = &negZcolors[0])
                {
                    var p = new IntPtr(ptr);
                    GPU.Gl.TexImage2D(TextureTarget.TextureCubeMapNegativeZ, 0, inf, (uint)Bounds.Width, (uint)Bounds.Height, 0, pf,
                        PixelType.UnsignedByte, p);
                }
            }
            */
        }
    }
}