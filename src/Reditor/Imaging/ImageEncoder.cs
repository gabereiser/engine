using System;
using System.Collections.Generic;
using Reactor.Graphics;
using Reactor.Types;

namespace Red.Imaging
{
    public class ImageEncoder
    {
        public static RedFile SaveTexture(Texture2D texture, string name)
        {
            var pixelFormat = texture.PixelFormat;
            var width = texture.Bounds.Width;
            var height = texture.Bounds.Height;
            var surface = SurfaceFormat.Color;
            var data = texture.GetData<byte>();
            var red = new RedFile();
            red.Magic = new[] { 'R', 'E', 'D', '\0' };
            red.Name = name;
            red.DataType = (int)RedDataType.Texture2D;
            red.DataLength = data.Length;
            red.Data = data;
            return red;
        }
        
        public static RedFile SaveTexture(TextureCube texture, string name)
        {
            List<byte> data = new List<byte>();
            
            var pixelFormat = texture.PositiveX.PixelFormat;
            var width = texture.PositiveX.Bounds.Width;
            var height = texture.PositiveX.Bounds.Height;
            var surface = SurfaceFormat.Color;
            
            var red = new RedFile();
            red.Magic = new[] { 'R', 'E', 'D', '\0' };
            red.Name = name;
            red.DataType = (int)RedDataType.TextureCube;
            
            data.AddRange(texture.PositiveX.GetData<byte>());
            data.AddRange(texture.NegativeX.GetData<byte>());
            data.AddRange(texture.PositiveY.GetData<byte>());
            data.AddRange(texture.NegativeY.GetData<byte>());
            data.AddRange(texture.PositiveZ.GetData<byte>());
            data.AddRange(texture.NegativeZ.GetData<byte>());
            red.Data = data.ToArray();
            red.DataLength = red.Data.Length;
            return red;
        }
    }
}