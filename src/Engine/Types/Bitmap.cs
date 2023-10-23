using System;
using System.Drawing;
using Reactor.Graphics;

namespace Reactor.Types
{
    public class Bitmap : IDisposable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        
        public RColor[] Pixels { get; set; }

        public PixelFormat PixelFormat { get; set; } = PixelFormat.Rgba;


        public Bitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Pixels = new RColor[width * height];
        }

        public void Clear(RColor color)
        {
            for(var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                {
                    Pixels[x * y] = color;
                }
        }

        public void SetData(byte[] data, PixelFormat format)
        {
            PixelFormat = format;
            switch (PixelFormat)
            {
                case PixelFormat.Alpha:
                case PixelFormat.Red:
                    setR8(data);
                    break;
                case PixelFormat.Bgra:
                    setBGRA(data);
                    break;
                case PixelFormat.Rgba:
                    setRGBA(data);
                    break;
                default:
                    setRGBA(data);
                    break;
            }
        }

        void setRGBA(byte[] data)
        {
            var i = 0;
            for(var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                {
                    var c = new RColor();
                    c.R = data[i];
                    c.G = data[i + 1];
                    c.B = data[i + 2];
                    c.A = data[i + 3];
                    Pixels[x * y] = c;
                    i += 4;
                }
        }
        
        void setBGRA(byte[] data)
        {
            var i = 0;
            for(var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                {
                    var c = new RColor();
                    c.B = data[i];
                    c.G = data[i + 1];
                    c.R = data[i + 2];
                    c.A = data[i + 3];
                    Pixels[x * y] = c;
                    i += 4;
                }
        }
        
        void setARGB(byte[] data)
        {
            var i = 0;
            for(var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                {
                    var c = new RColor();
                    c.A = data[i];
                    c.R = data[i + 1];
                    c.G = data[i + 2];
                    c.B = data[i + 3];
                    Pixels[x * y] = c;
                    i += 4;
                }
        }
        void setR8(byte[] data)
        {
            var i = 0;
            for(var y = 0; y < Height; ++y)
                for (var x = 0; x < Width; ++x)
                {
                    var c = new RColor();
                    c.R = data[i];
                    c.G = c.R;
                    c.B = c.R;
                    c.A = c.R;
                    Pixels[x * y] = c;
                    i++;
                }
        }

        public void SetPixel(int x, int y, RColor color)
        {
            Pixels[x * y] = color;
        }
        public void DrawImage(Bitmap bitmap, Rectangle bounds)
        {
            if (bitmap.PixelFormat != PixelFormat)
                throw new Exception("PixelFormat mismatch");
            
            
        }
        public void Dispose()
        {
        }
        
    }
}