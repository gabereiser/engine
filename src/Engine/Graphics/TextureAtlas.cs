using System;
using System.Collections.Generic;
using System.Drawing;
using Reactor.Common;
using Reactor.Platform;

namespace Reactor.Graphics
{
    public class TextureAtlas : Texture2D
    {
        private const int FixedWidth = 64;

        public TextureAtlas()
        {
            textureTarget = TextureTarget.Texture2D;
        }

        public void BuildAtlas(List<TextureSprite> textures)
        {
            GPU.CheckError();
            textures.Sort(new TextureSizeSorter());
            textures.Reverse();
            var largest = textures[0].Bounds;
            //int cellSize = System.Math.Max(largest.Width, largest.Height);
            //double sqr = System.Math.Sqrt((double)textures.Count);
            //int remainder = ((int)(sqr*100) % 100);
            /*Rectangle bounds = new Rectangle();
            foreach(RTextureSprite sprite in textures)
            {
                bounds = Rectangle.Union(bounds, sprite.Bounds);
            }
            RTextureSprite previous = null;
            foreach(RTextureSprite sprite in textures)
            {
                if(previous == null){
                    sprite.Offset.X = 0;
                    previous = sprite;
                }
                else {
                    sprite.Offset.X = previous.Offset.X + previous.Bounds.Width;
                    sprite.Bounds.Offset(sprite.Offset);
                    previous = sprite;
                }

                bounds = Rectangle.Union(bounds, previous.Bounds);
            }
            while(!this.isPowerOfTwo((uint)bounds.Height))
                bounds.Height += 1;
            while(!this.isPowerOfTwo((uint)bounds.Width))
                bounds.Width += 1;
                */
            var root = new AtlasNode();
            root.bounds = new Rectangle(0, 0, 512, 512);
            Create(512, 512, textures[0].PixelFormat, SurfaceFormat.Color);
            uint index = 0;
            var unclaimed = 0;

            GPU.CheckError();
            foreach (var sprite in textures)
            {
                try
                {
                    var node = root.Insert(sprite.Bounds);
                    if (node != null)
                    {
                        Log.Info(node.ToString());
                        sprite.ScaledBounds = node.bounds;

                        //Pack(sprite, sprite.GetPixelFormat());
                    }
                    else
                    {
                        unclaimed++;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                index++;
            }
        }

        private void Pack(TextureSprite sprite, PixelFormat format)
        {
            var data = sprite.GetData<byte>();
            SetData(data, format, (int)sprite.Offset.X, (int)sprite.Offset.Y, sprite.ScaledBounds.Width,
                sprite.ScaledBounds.Height, false);
        }
    }

    internal class AtlasNode
    {
        public Rectangle bounds;
        public bool filled;
        public AtlasNode left;
        public AtlasNode right;

        public AtlasNode()
        {
            left = null;
            right = null;
            bounds = Rectangle.Empty;
            filled = false;
        }

        public AtlasNode Insert(Rectangle sBounds)
        {
            if (left != null)
            {
                var newNode = left.Insert(sBounds);
                if (newNode != null)
                    return newNode;

                return right.Insert(sBounds);
            }

            if (filled) // occupied!
                return null;
            if (bounds.Width < sBounds.Width || bounds.Height < sBounds.Height) // bounds are too small for this image
                return null;
            if (bounds.Width == sBounds.Width && bounds.Height == sBounds.Height)
            {
                // fits just right!
                filled = true;
                return this;
            }

            //otherwise split and traverse further...
            left = new AtlasNode();
            right = new AtlasNode();

            // decide which way to split
            var dw = bounds.Width - sBounds.Width;
            var dh = bounds.Height - sBounds.Height;
            if (dw > dh)
            {
                left.bounds = new Rectangle(bounds.X, bounds.Y, sBounds.Width, bounds.Height);
                right.bounds = new Rectangle(bounds.X + sBounds.Width, bounds.Y, bounds.Width - sBounds.Width,
                    bounds.Height);
            }
            else
            {
                left.bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, sBounds.Height);
                right.bounds = new Rectangle(bounds.X, bounds.Y + sBounds.Height, bounds.Width,
                    bounds.Height - sBounds.Height);
            }

            return left.Insert(sBounds);
        }
    }

    internal class TextureSizeSorter : IComparer<TextureSprite>
    {
        public int Compare(TextureSprite left, TextureSprite right)
        {
            if (left.Bounds.Height > right.Bounds.Height)
                return 1;
            return -1;
            return 0;
        }
    }
}