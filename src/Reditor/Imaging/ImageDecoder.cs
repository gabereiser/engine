
using System;
using System.Drawing;
using System.IO;
using Red.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Reditor.Imaging
{
    public static class ImageDecoder
    {
        public static void ReadRGBA(Stream stream, out Texture2D texture)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(stream);
            Span<byte> data = new Span<byte>();
            image.CopyPixelDataTo(data);
            texture = new Texture2D();
            texture.Create(image.Width, image.Height, PixelFormat.Rgba, SurfaceFormat.Color, false);
            texture.SetData(data.ToArray(), PixelFormat.Rgba, 0, 0, image.Width, image.Height, false);
        }
        public static void ReadBGRA(Stream stream, out Texture2D texture)
        {
            Image<Rgba32> image = Image.Load<Rgba32>(stream);
            Span<byte> imageData = new Span<byte>();
            byte[] data = imageData.ToArray();
            // Convert rgba to bgra
            for (int i = 0; i < image.Width * image.Height; ++i)
            {
                byte r = data[i * 4];
                byte g = data[i * 4 + 1];
                byte b = data[i * 4 + 2];
                byte a = data[i * 4 + 3];


                data[i * 4] = b;
                data[i * 4 + 1] = g;
                data[i * 4 + 2] = r;
                data[i * 4 + 3] = a;
            }
            texture = new Texture2D();
            texture.Create(image.Width, image.Height, PixelFormat.Bgra, SurfaceFormat.Color, false);
            texture.SetData(data, PixelFormat.Bgra, 0, 0, image.Width, image.Height, false);
        }

    }
}