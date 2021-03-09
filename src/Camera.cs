
using Microsoft.Xna.Framework;

namespace Atlas
{
    public class Camera : Node
    {
        public float Zoom { get; set; } = 1f;
        public Rectangle Viewport => new Rectangle(Position.ToPoint(), (Size / Zoom).ToPoint());

        public override Vector2 Centre
        {
            get => Position + Viewport.Size.ToVector2() / 2;
            set => Position = value - Viewport.Size.ToVector2() / 2;
        }

        public Camera(int width, int height)
        {
            Size = new Vector2(width, height);
        }

        public void ZoomTowards(Vector2 pos, float zoom)
        {
            var vectorFrom = Centre - pos;
            double change = (double)this.Zoom / (double)zoom;   // To avoid camera shifting due to rounding errors
            this.Zoom = zoom;
            Centre = pos + vectorFrom * (float)change;
        }
    }
}