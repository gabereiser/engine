using Reactor.Math3D;
using Reactor.Types;

namespace Reactor.Graphics
{
    public class TextureSprite : Texture2D
    {
        public Vector2 Offset = new Vector2();
        public Vector2 Origin = new Vector2();
        public Rectangle ScaledBounds = new Rectangle();

        public RColor Color { get; set; }

        public void Render(bool flipped = false)
        {
            var bounds = new Rectangle((int)(Origin.X + Offset.X), (int)(Origin.Y + Offset.Y), Bounds.Width,
                Bounds.Height);
            if (flipped)
                Screen.RenderTexture(this, bounds, Color, Matrix.CreateRotationY(MathHelper.ToRadians(180f)));
            else
                Screen.RenderTexture(this, bounds, Color, Matrix.Identity);
        }
    }
}