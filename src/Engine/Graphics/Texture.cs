using System;
using System.Drawing;
using System.IO;
using Red.Common;
using Red.Platform;
using Red.Platform.WGPU;
using Red.Types;



namespace Red.Graphics
{
    public class Texture : IDisposable
    {
        internal static Texture2D defaultWhite = new Texture2D(true);

        private bool bound;

        public Rectangle Bounds;

        public string Filename;

        public uint Id;

        internal Platform.WGPU.Wrappers.Texture Handle;
        internal Platform.WGPU.Wrappers.Sampler Sampler;

        public string Name;

        public PixelFormat PixelFormat { get; set; } = PixelFormat.Rgba;

        public PixelType PixelType { get; set; } = PixelType.UnsignedByte;

        internal TextureTarget textureTarget;

        public TextureMagFilter MagFilter { get; set; }
        public TextureMinFilter MinFilter { get; set; }
        public TextureWrapMode WrapS { get; set; }
        public TextureWrapMode WrapT { get; set; }
        public TextureWrapMode WrapR { get; set; }

        #region IDisposable implementation

        public Texture()
        { }

        internal Texture(Red.Platform.WGPU.Wrappers.Texture texture, PixelFormat pixelFormat)
        {
            Name = texture.Label;
            Filename = texture.Label;
            Handle = texture;
            MinFilter = TextureMinFilter.Nearest;
            MagFilter = TextureMagFilter.Nearest;
            WrapS = TextureWrapMode.ClampToBorder;
            WrapT = TextureWrapMode.ClampToBorder;
            WrapR = TextureWrapMode.ClampToBorder;
            PixelFormat = pixelFormat;
            PixelType = PixelType.UnsignedByte;
            textureTarget = texture.Dimension == Wgpu.TextureDimension.TwoDimensions
                ? TextureTarget.Texture2D
                : TextureTarget.Texture3D;

            GPU.Device.CreateSampler(texture.Label + "-sampler", Wgpu.AddressMode.ClampToEdge,
                Wgpu.AddressMode.ClampToEdge, Wgpu.AddressMode.ClampToEdge, Wgpu.FilterMode.Nearest,
                Wgpu.FilterMode.Nearest, Wgpu.MipmapFilterMode.Nearest, 0, 100, Wgpu.CompareFunction.LessEqual, 0);
        }
        public void Dispose()
        {
            if (bound) Unbind();

            if (Id != 0)
            {
                unsafe
                {
                    Handle.Dispose();
                }
            }
        }

        #endregion

        internal void LoadFromData(byte[] data, string name, bool isCompressed)
        {
            if (isCompressed)
                try
                {
                    //ImageDDS.LoadFromData(data, name, out Id, out textureTarget, out pixelFormat, out pixelType);
                }
                catch (Exception e)
                {
                    Log.Error("Error loading texture for: " + name);
                    Log.Error(e.Message);
                    Log.Error(e);
                }
            else
                try
                {
                    //ImageGDI.LoadFromData(data, out Id, out textureTarget, out pixelFormat, out pixelType, out Bounds);
                }
                catch (Exception e)
                {
                    Log.Error("Error loading texture for: " + name);
                    Log.Error(e.Message);
                    Log.Error(e);
                }

            if (Id == 0 || textureTarget == 0) Log.Error("Error generating OpenGL texture for: " + name);

            CreateProperties(TextureTarget.Texture2D);
            Log.Info("Texture loaded for: " + name);
            Name = name.TrimEnd(Path.GetExtension(name).ToCharArray());
            Filename = name;
            Unbind();
        }

        internal void LoadFromBitmap(Bitmap bitmap)
        {
            try
            {
                //ImageGDI.LoadFromBitmap(ref bitmap, out Id, out textureTarget, out pixelFormat, out pixelType,
                //out Bounds);
            }
            catch (Exception e)
            {
                Log.Error("Error loading texture from bitmap...");
                Log.Error(e);
            }

            if (Id == 0 || textureTarget == 0) Log.Error("Error generating OpenGL texture from bitmap");
            CreateProperties(TextureTarget.Texture2D);
            Bounds = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        }

        internal void LoadFromDisk(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".dds")
                try
                {
                    //ImageDDS.LoadFromDisk(FileSystem.Instance.GetFilePath(filename), out Id, out textureTarget,
                    //out pixelFormat, out pixelType);
                }
                catch (Exception e)
                {
                    Log.Error("Error loading texture from: " + filename);
                    Log.Error(e);
                }
            else
                try
                {
                    //ImageGDI.LoadFromDisk(FileSystem.Instance.GetFilePath(filename), out Id, out textureTarget,
                    //out pixelFormat, out pixelType, out Bounds);
                }
                catch (Exception e)
                {
                    Log.Error("Error loading texture from: " + filename);
                    Log.Error(e);
                    return;
                }

            if (Id == 0 || textureTarget == 0)
            {
                Log.Error("Error generating OpenGL texture from: " + filename);
                return;
            }

            CreateProperties(TextureTarget.Texture2D);
            Log.Info("Texture loaded from: " + filename);
            Name = filename.TrimEnd(Path.GetExtension(filename).ToCharArray());
            Filename = filename;
            Unbind();
        }

        internal void Bind()
        {
            //GPU.BindTexture(textureTarget, Id);
            //GPU.CheckError();
            bound = true;
        }

        internal void SetActive(TextureLayer layer)
        {
            //GPU.ActiveTexture(GLEnum.Texture0 + (int)layer);
            //GPU.CheckError();
        }

        internal void Unbind()
        {
            //GPU.BindTexture(textureTarget, 0);
            bound = false;
        }

        public void SetTextureMagFilter(TextureMagFilter value)
        {
            Bind();
            //GPU.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, (int)value);
            //GPU.CheckError();
            Unbind();
        }

        public void SetTextureMinFilter(TextureMinFilter value)
        {
            Bind();
            //GPU.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, (int)value);
            //GPU.CheckError();
            Unbind();
        }

        public void SetTextureWrapMode(TextureWrapMode modeS, TextureWrapMode modeT)
        {
            Bind();
            //GPU.TexParameter(textureTarget, TextureParameterName.TextureWrapS, (int)modeS);
            //GPU.CheckError();
            //GPU.TexParameter(textureTarget, TextureParameterName.TextureWrapT, (int)modeT);
            //GPU.CheckError();
            Unbind();
        }


        protected bool isPowerOfTwo(uint x)
        {
            while (x % 2 == 0 && x > 1) /* While x is even and > 1 */
                x /= 2;
            return x == 1;
        }

        public T[] GetData<T>() where T : struct
        {
            /*GPU.ActiveTexture(TextureUnit.Texture0);
            GPU.CheckError();
            Bind();
            GPU.CheckError();
            var pixels = new T[Bounds.Width * Bounds.Height];
            unsafe
            {
                fixed (T* ptr = &pixels[0])
                {
                    GPU.GetTexImage(textureTarget, 0, (PixelFormat)pixelFormat, pixelType, ptr);
                }
            }

            GPU.CheckError();
            Unbind();
            GPU.CheckError();

            return pixels;
            */
            return null;
        }

        public void SetData<T>(T[] data, PixelFormat format, int x, int y, int width, int height,
            bool packAlignment = true) where T : struct
        {
            /*if (!packAlignment)
                GPU.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            Bind();
            unsafe
            {
                fixed (T* ptr = &data[0])
                {
                    GPU.TexSubImage2D(textureTarget, 0, x, y, (uint)width, (uint)height, format, PixelType.UnsignedByte, ptr);
                }
            }

            GPU.CheckError();
            Unbind();
            if (!packAlignment)
                GPU.PixelStore(PixelStoreParameter.PackAlignment, 1);
                */
        }

        public void SetData(RColor[] colors, PixelFormat format, int x, int y, int width, int height)
        {
            Bind();
            unsafe
            {
                fixed (RColor* ptr = &colors[0])
                {
                    //GPU.TexSubImage2D(textureTarget, 0, x, y, (uint)width, (uint)height, format, PixelType.UnsignedByte, ptr);
                }
            }

            GPU.CheckError();
            Unbind();
            GPU.CheckError();
        }

        public void GenerateMipmaps()
        {
            //GPU.GenerateMipmap(GLEnum.Texture2D);
        }

        protected void CreateProperties(TextureTarget target, bool mipmapped = false)
        {
            textureTarget = target;
            Bind();
            /*GPU.TexParameter(textureTarget, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            
            GPU.TexParameter(textureTarget, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

            GPU.TexParameter(textureTarget, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);

            GPU.TexParameter(textureTarget, TextureParameterName.TextureWrapR, (int)GLEnum.Repeat);

            GPU.TexParameter(textureTarget, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);

            GPU.CheckError();
            Unbind();
            GPU.CheckError();
            */
        }
    }

    public enum TextureMagFilter
    {
        Nearest = 0,
        Linear
    }

    public enum TextureMinFilter
    {
        Nearest = 0,
        Linear,
        NearestMipmapNearest,
        LinearMipmapNearest,
        NearestMipmapLinear,
        LinearMipmapLinear,
    }

    public enum TextureWrapMode
    {
        Clamp = 0,
        ClampToBorder,
        Repeat,
        Mirror,
    }

    public enum TextureLayer
    {
        TEXTURE0 = 0,
        TEXTURE1,
        TEXTURE2,
        TEXTURE3,
        TEXTURE4,
        TEXTURE5,
        TEXTURE6,
        TEXTURE7,
        TEXTURE8,
        TEXTURE9,
        TEXTURE10,
        TEXTURE11,
        TEXTURE12,
        TEXTURE13,
        TEXTURE14,
        TEXTURE15,
        TEXTURE16,
        TEXTURE17,
        TEXTURE18,
        TEXTURE19,
        TEXTURE20,
        TEXTURE21,
        TEXTURE22,
        TEXTURE23,
        TEXTURE24,
        TEXTURE25,
        TEXTURE26,
        TEXTURE27,
        TEXTURE28,
        TEXTURE29,
        TEXTURE30,
        TEXTURE31
    }

    public enum DepthFormat
    {
        None = -1,
        Depth16 = 54,
        Depth24 = 51,
        Depth24Stencil8 = 48,
        Depth32Stencil8 = 49
    }
    public enum SurfaceFormat
    {
        Color = 0,
        Bgr565 = 1,
        Bgra5551 = 2,
        Bgra4444 = 3,
        Dxt1 = 4,
        Dxt3 = 5,
        Dxt5 = 6,
        NormalizedByte2 = 7,
        NormalizedByte4 = 8,
        Rgba1010102 = 9,
        Rg32 = 10,
        Rgba64 = 11,
        Alpha8 = 12,
        Single = 13,
        Vector2 = 14,
        Vector4 = 15,
        HalfSingle = 16,
        HalfVector2 = 17,
        HalfVector4 = 18,
        HdrBlendable = 19,
        HdrFull = 20,

        // BGRA formats are required for compatibility with WPF D3DImage.
        Bgr32 = 21, // B8G8R8X8
        Bgra32 = 22, // B8G8R8A8

        // Good explanation of compressed formats for mobile devices (aimed at Android, but describes PVRTC)
        // http://developer.motorola.com/docstools/library/understanding-texture-compression/

        // PowerVR texture compression (iOS and Android)
        RgbPvrtc2Bpp = 50,
        RgbPvrtc4Bpp = 51,
        RgbaPvrtc2Bpp = 52,
        RgbaPvrtc4Bpp = 53,

        // Ericcson Texture Compression (Android)
        RgbEtc1 = 60,

        // DXT1 also has a 1-bit alpha form
        Dxt1a = 70
    }


}