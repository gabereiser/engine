using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Reactor.Common;
using Reactor.Fonts;
using Reactor.Graphics;


namespace Reactor.Types
{
    public enum RedDataType
    {
        Unknown = 0,
        BitmapFont,
        Font,
        Gltf,
        Texture2D,
        Texture3D,
        TextureCube
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TextureHeader
    {
        public PixelFormat PixelFormat;
        public SurfaceFormat SurfaceFormat;
        public uint Width;
        public uint Height;
        public int MinFilter;
        public int MagFilter;
        public int WrapS;
        public int WrapT;
        public byte[] Data;
    }
    
    public struct RedFile
    {
        public char[] Magic;
        public uint Version;
        public string Name;
        public int DataType;
        public int DataLength;
        public byte[] Data;
        
        Texture readTexture(Stream s)
        {
            var textureHeader = Serialization.ReadStruct<TextureHeader>(s);
            var texture = new Texture2D();
            texture.Create((int)textureHeader.Width, (int)textureHeader.Height, textureHeader.PixelFormat,
                textureHeader.SurfaceFormat);
            texture.SetData(textureHeader.Data, textureHeader.PixelFormat, 0, 0, (int)textureHeader.Width, (int)textureHeader.Height);
            texture.SetTextureMinFilter((TextureMinFilter)textureHeader.MinFilter);
            texture.SetTextureMagFilter((TextureMagFilter)textureHeader.MagFilter);
            texture.SetTextureWrapMode((TextureWrapMode)textureHeader.WrapS, (Graphics.TextureWrapMode)textureHeader.WrapT);
            return texture;
        }

        public BitmapFont BitmapFont()
        {
            if (DataType == (int)RedDataType.BitmapFont)
            {
                using (var s = new MemoryStream(Data))
                {
                    return new BitmapFont(s);
                }
            }
            throw new AccessViolationException("RedFile is not a BitmapFont");
        }
        public Texture2D Texture2D()
        {
            if (DataType == (int)RedDataType.Texture2D)
            {
                using (var s = new MemoryStream(Data))
                {
                    return (Texture2D)readTexture(s);
                }
            }

            throw new AccessViolationException("RedFile is not a Texture2D");
        }
        public TextureCube TextureCube()
        {
            if (DataType == (int)RedDataType.TextureCube)
            {
                var cube = new TextureCube();
                List<Texture2D> textures = new List<Texture2D>();
                using (var s = new MemoryStream(Data))
                {
                    //POS_X
                    textures.Add((Texture2D)readTexture(s));
                    //NEG_X
                    textures.Add((Texture2D)readTexture(s));
                    //POS_Y
                    textures.Add((Texture2D)readTexture(s));
                    //NEG_Y
                    textures.Add((Texture2D)readTexture(s));
                    //POS_Z
                    textures.Add((Texture2D)readTexture(s));
                    //NEG_Z
                    textures.Add((Texture2D)readTexture(s));
                }

                var px = textures[0];
                var nx = textures[1];
                var py = textures[2];
                var ny = textures[3];
                var pz = textures[4];
                var nz = textures[5];
                cube.Create(textures[0].PixelFormat, ref px, ref py, ref pz, ref nx, ref ny, ref nz);
                
                
            }
            throw new AccessViolationException("RedFile is not a TextureCube");
        }
    }
}