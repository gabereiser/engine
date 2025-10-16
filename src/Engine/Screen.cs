using System.Drawing;
using System.Numerics;
using Red.Graphics;
using Red.Types;

namespace Red
{
    public class Screen
    {
        public static Vector2 GetDPI()
        {
            return new Vector2(72f, 72f);
        }
        public static void RenderTexture(Texture texture, Rectangle bounds, RColor color, Matrix4x4 model) { }
    }
}