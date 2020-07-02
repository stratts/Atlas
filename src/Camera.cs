
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Camera : Node
    {
        public Point Size;
        public float Zoom { get; set; } = 1f;
        public Camera(int width, int height)
        {
            Size = new Point(width, height);
        }

        public Rectangle Viewport
        {
            get
            {
                var sizeV = Size.ToVector2();
                var centre = Position + sizeV / 2;
                var viewSize = sizeV / Zoom;
                return new Rectangle((centre - viewSize / 2).ToPoint(), viewSize.ToPoint());
            }
        }
    }
}