using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Red.Geometry;
using Red.Graphics;
using Red.Platform;
using Red.Types;
using SharpFont;
using Red.Math3D;
using Rectangle = System.Drawing.Rectangle;

namespace Red.Fonts
{
    public class TrueTypeFont : Font
    {
        public int Size;
        public string Name;
        public bool Kerning;
        public int LineHeight;
        public int SpaceWidth;
        public int Ascent;
        public int Descent;
        internal List<TrueTypeFontGlyph> Glyphs;
        public Texture2D Texture;
        internal VertexData2D[] quadVerts = new VertexData2D[4];
        internal static Font Default = new TrueTypeFont(FontResources.SystemFont);

        public TrueTypeFont()
        {
        }

        internal TrueTypeFont(Face face)
        {
            Vector2 dpi = Screen.GetDPI();
            if (Environment.OSVersion.Platform == PlatformID.Unix)
                Generate(face, 16, (int)dpi.X);
            else
                Generate(face, 16, (int)dpi.Y);
        }
        internal void Save(ref BinaryWriter stream)
        {
            stream.Write(Size);
            stream.Write(Name);
            stream.Write(Kerning);
            stream.Write(LineHeight);
            stream.Write(SpaceWidth);
            stream.Write(Ascent);
            stream.Write(Descent);
            stream.Write(Glyphs.Count);
            foreach (var glyph in Glyphs)
            {
                glyph.Save(ref stream);
            }
            stream.Close();
        }
        internal void Load(ref BinaryReader stream)
        {
            Size = stream.ReadInt32();
            Name = stream.ReadString();
            Kerning = stream.ReadBoolean();
            LineHeight = stream.ReadInt32();
            SpaceWidth = stream.ReadInt32();
            Ascent = stream.ReadInt32();
            Descent = stream.ReadInt32();
            int glyph_count = stream.ReadInt32();
            Glyphs = new List<TrueTypeFontGlyph>();
            for (var i = 0; i < glyph_count; i++)
            {
                var glyph = new TrueTypeFontGlyph();
                glyph.Load(ref stream);
                Glyphs.Add(glyph);
            }
            stream.Close();


        }
        internal void Generate(string filename, int size, int dpi)
        {
            Face face = new Face(FontResources.FreetypeLibrary, filename);
            Generate(face, size, dpi);
        }
        internal void Generate(Face face, int size, int dpi)
        {
            face.SetCharSize(0, new Fixed26Dot6(size), 0, (uint)dpi);
            Name = face.FamilyName;
            face.LoadChar((uint)32, (LoadFlags.Render | LoadFlags.Monochrome | LoadFlags.Pedantic), LoadTarget.Normal);
            SpaceWidth = face.Glyph.Metrics.HorizontalAdvance.ToInt32();
            LineHeight = face.Height >> 6;
            Kerning = face.HasKerning;
            Size = size;
            Ascent = face.Ascender >> 6;
            Descent = face.Descender >> 6;
            Glyphs = new List<TrueTypeFontGlyph>();


            for (int i = 33; i < 126; i++)
            {


                uint charIndex = face.GetCharIndex((uint)i);
                face.LoadGlyph(charIndex, (LoadFlags.Render | LoadFlags.Color | LoadFlags.Pedantic | LoadFlags.CropBitmap), LoadTarget.Normal);
                if (face.Glyph.Bitmap.PixelMode == PixelMode.None)
                    continue;
                TrueTypeFontGlyph glyph = new TrueTypeFontGlyph();
                var bitmap = face.Glyph.Bitmap;
                glyph.bitmap = new Bitmap(bitmap.Width, bitmap.Rows);
                glyph.bitmap.SetData(bitmap.BufferData, PixelFormat.Red);
                glyph.Bounds = new Rectangle(0, 0, bitmap.Width, bitmap.Rows);
                glyph.CharIndex = i;
                glyph.Offset = new Vector2(face.Glyph.Metrics.HorizontalBearingX.ToInt32(), face.Glyph.Metrics.HorizontalBearingY.ToInt32());
                glyph.Advance = face.Glyph.Advance.X.ToInt32();

                Glyphs.Add(glyph);
            }
            Glyphs.Sort(new FontGlyphSizeSorter());
            var missed = -1;
            var width = 16;
            Bitmap b = new Bitmap(1, 1);
            while (missed != 0)
            {
                missed = 0;
                AtlasNode root = new AtlasNode();
                root.bounds = new Rectangle(0, 0, width, width);
                b.Dispose();
                b = new Bitmap(width, width);

                b.Clear(RColor.Transparent);

                for (var i = 0; i < Glyphs.Count; i++)
                {
                    TrueTypeFontGlyph glyph = Glyphs[i];
                    AtlasNode result = root.Insert(glyph.Bounds);

                    if (result != null)
                    {
                        Rectangle bounds = result.bounds;
                        //g.DrawImageUnscaledAndClipped(glyph.bitmap, bounds);
                        b.DrawImage(glyph.bitmap, bounds);
                        glyph.Bounds = bounds;
                        glyph.UVBounds = new Vector4((float)bounds.X, (float)bounds.Y, (float)bounds.Width, (float)bounds.Height);
                        glyph.UVBounds /= (float)width;
                        Glyphs[i] = glyph;
                    }
                    else
                    {
                        missed += 1;
                        break;
                    }


                }
                width += 16;
            }
            Texture = new Texture2D();
            Texture.LoadFromBitmap(b);
            Texture.SetTextureMagFilter(TextureMagFilter.Nearest); // we want our font to be crisp and sharp. SharpFont.
            Texture.SetTextureMinFilter(TextureMinFilter.Nearest);
            Texture.SetTextureWrapMode(TextureWrapMode.ClampToBorder, TextureWrapMode.ClampToBorder);
            GPU.CheckError();

        }
        internal void Render(ref Program program,
            ref VertexBuffer vertexBuffer,
            ref IndexBuffer indexBuffer,
            string text,
            Vector2 location,
            RColor color,
            Matrix matrix)
        {
            Vector2 pen = location;
            pen.Y += MeasureString(text).Height;
            float x = pen.X;
            List<VertexData2D> quads = new List<VertexData2D>();
            foreach (char c in text)
            {
                if (c == '\r')
                {
                    continue;
                }
                if (c == '\n')
                {
                    pen.X = x;
                    pen.Y += LineHeight;
                    continue;
                }
                if (c == ' ')
                {
                    pen.X += SpaceWidth;
                    continue;
                }
                if (c == '\t')
                {
                    pen.X += (SpaceWidth * 3);
                    continue;
                }
                TrueTypeFontGlyph glyph = GetGlyphForChar(c);
                var dest = new Rectangle();
                dest.X = (int)(pen.X + glyph.Offset.X);
                dest.Y = (int)pen.Y - ((int)glyph.Offset.Y);
                dest.Width = glyph.Bounds.Width;
                dest.Height = glyph.Bounds.Height;

                vertexBuffer.SetData<VertexData2D>(AddQuads(dest, glyph.UVBounds));
                vertexBuffer.Bind();
                vertexBuffer.BindVertexArray();
                indexBuffer.Bind();
                vertexBuffer.VertexDeclaration.Apply(program, IntPtr.Zero);
                //GPU.Gl.DrawElements(PrimitiveType.Triangles, (uint)indexBuffer.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
                GPU.CheckError();
                indexBuffer.Unbind();
                vertexBuffer.Unbind();
                vertexBuffer.UnbindVertexArray();
                pen.X += glyph.Advance;

            }

        }

        public Rectangle MeasureString(string text)
        {
            Rectangle r = new Rectangle();
            foreach (char c in text)
            {
                if (c == ' ')
                {
                    r.Width += SpaceWidth;
                    continue;
                }
                if (c == '\t')
                {
                    r.Width += (SpaceWidth * 3);
                    continue;
                }
                TrueTypeFontGlyph glyph = GetGlyphForChar(c);
                r.Height = System.Math.Max(r.Height, glyph.Bounds.Height);
                r.Width += (int)glyph.Offset.X;
            }
            return r;
        }

        TrueTypeFontGlyph GetGlyphForChar(char ch)
        {
            foreach (var g in Glyphs)
            {
                if (g.CharIndex == (int)ch)
                    return g;
            }
            throw new InvalidOperationException(String.Format("Character {0} not found in character map", ch));
        }
        VertexData2D[] AddQuads(Rectangle placement, Vector4 UVs)
        {
            quadVerts[0].Position = new Vector2(placement.X, placement.Y);
            quadVerts[0].TexCoord = new Vector2(UVs.X, UVs.Y);
            quadVerts[1].Position = new Vector2(placement.X + placement.Width, placement.Y);
            quadVerts[1].TexCoord = new Vector2(UVs.X + UVs.Z, UVs.Y);
            quadVerts[2].Position = new Vector2(placement.X + placement.Width, placement.Y + placement.Height);
            quadVerts[2].TexCoord = new Vector2(UVs.X + UVs.Z, UVs.Y + UVs.W);
            quadVerts[3].Position = new Vector2(placement.X, placement.Y + placement.Height);
            quadVerts[3].TexCoord = new Vector2(UVs.X, UVs.Y + UVs.W);
            //vertexBuffer.SetData<RVertexData2D>(quadVerts);
            return quadVerts;
        }
    }
    internal static class FontResources
    {
        internal static Library FreetypeLibrary = new Library();
        internal static Assembly Assembly = Assembly.GetAssembly(typeof(FontResources));
        internal static Face GetResource(string resource)
        {
            var names = Assembly.GetManifestResourceNames();
            Stream stream = Assembly.GetManifestResourceStream(resource);
            System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
            byte[] buffer = new byte[reader.BaseStream.Length];
            reader.Read(buffer, 0, buffer.Length);
            reader.Close();

            Face face = FreetypeLibrary.NewMemoryFace(buffer, 0);
            return face;

        }

        internal static Face SystemFont = GetResource("Red.Resources.coders_crux.ttf");
        /*public static Bitmap ToGdipBitmap(this FTBitmap ftBitmap, Color color)
        {
            if (ftBitmap.IsDisposed)
				throw new ObjectDisposedException("FTBitmap", "Cannot access a disposed object.");

			if (ftBitmap.Width == 0 || ftBitmap.Rows == 0)
				throw new InvalidOperationException("Invalid image size - one or both dimensions are 0.");

			//TODO deal with negative pitch
			switch (ftBitmap.PixelMode)
			{
				case PixelMode.Mono:
					{
						Bitmap bmp = new Bitmap(ftBitmap.Width, ftBitmap.Rows, PixelFormat.Format1bppIndexed);
						var locked = bmp.LockBits(new Rectangle(0, 0, ftBitmap.Width, ftBitmap.Rows), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

						for (int i = 0; i < ftBitmap.Rows; i++)
							Copy(ftBitmap.Buffer, i * ftBitmap.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

						bmp.UnlockBits(locked);

						ColorPalette palette = bmp.Palette;
						palette.Entries[0] = Color.FromArgb(0, color);
						palette.Entries[1] = Color.FromArgb(255, color);

						bmp.Palette = palette;
						return bmp;
					}

				case PixelMode.Gray4:
					{
						Bitmap bmp = new Bitmap(ftBitmap.Width, ftBitmap.Rows, PixelFormat.Format4bppIndexed);
						var locked = bmp.LockBits(new Rectangle(0, 0, ftBitmap.Width, ftBitmap.Rows), ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

						for (int i = 0; i < ftBitmap.Rows; i++)
							Copy(ftBitmap.Buffer, i * ftBitmap.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

						bmp.UnlockBits(locked);

						ColorPalette palette = bmp.Palette;
						for (int i = 0; i < palette.Entries.Length; i++)
						{
							float a = (i * 17) / 255f;
							palette.Entries[i] = Color.FromArgb(i * 17, (int)(color.R * a), (int)(color.G * a), (int)(color.B * a));
						}

						bmp.Palette = palette;
						return bmp;
					}

				case PixelMode.Gray:
					{
						Bitmap bmp = new Bitmap(ftBitmap.Width, ftBitmap.Rows, PixelFormat.Format8bppIndexed);
						var locked = bmp.LockBits(new Rectangle(0, 0, ftBitmap.Width, ftBitmap.Rows), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

						for (int i = 0; i < ftBitmap.Rows; i++)
							Copy(ftBitmap.Buffer, i * ftBitmap.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

						bmp.UnlockBits(locked);

						ColorPalette palette = bmp.Palette;
						for (int i = 0; i < palette.Entries.Length; i++)
						{
							float a = i / 255f;
							palette.Entries[i] = Color.FromArgb(i, (int)(color.R * a), (int)(color.G * a), (int)(color.B * a));
						}
						

						bmp.Palette = palette;
						return bmp;
					}

				case PixelMode.Lcd:
					{
						//TODO apply color
						int bmpWidth = ftBitmap.Width / 3;
						Bitmap bmp = new Bitmap(bmpWidth, ftBitmap.Rows, PixelFormat.Format24bppRgb);
						var locked = bmp.LockBits(new Rectangle(0, 0, bmpWidth, ftBitmap.Rows), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

						for (int i = 0; i < ftBitmap.Rows; i++)
							Copy(ftBitmap.Buffer, i * ftBitmap.Pitch, locked.Scan0, i * locked.Stride, locked.Stride);

						bmp.UnlockBits(locked);

						return bmp;
					}
				/case PixelMode.VerticalLcd:
				{
					int bmpHeight = b.Rows / 3;
					Bitmap bmp = new Bitmap(b.Width, bmpHeight, PixelFormat.Format24bppRgb);
					var locked = bmp.LockBits(new Rectangle(0, 0, b.Width, bmpHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
					for (int i = 0; i < bmpHeight; i++)
						PInvokeHelper.Copy(Buffer, i * b.Pitch, locked.Scan0, i * locked.Stride, b.Width);
					bmp.UnlockBits(locked);

					return bmp;
				}/

				default:
					throw new InvalidOperationException("System.Drawing.Bitmap does not support this pixel mode.");
			}
            
        }*/

        static unsafe void Copy(IntPtr source, int sourceOffset, IntPtr destination, int destinationOffset, int count)
        {
            byte* src = (byte*)source + sourceOffset;
            byte* dst = (byte*)destination + destinationOffset;
            byte* end = dst + count;

            while (dst != end)
                *dst++ = *src++;
        }
    }


    internal class FontGlyphSizeSorter : IComparer<TrueTypeFontGlyph>
    {
        public int Compare(TrueTypeFontGlyph left, TrueTypeFontGlyph right)
        {
            if (left.Bounds.Height > right.Bounds.Height)
                return -1;  // this is reversed on purpose, we want largest to smallest!
            else
                return 1;
            return 0;

        }
    }
}