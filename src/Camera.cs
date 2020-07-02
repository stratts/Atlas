
using Microsoft.Xna.Framework;

namespace Industropolis
{
    public class Camera : Node
    {
        public Point Size;
        public float Zoom { get; set; } = 1f;

        public Camera(int width, int height)
        {
            Size = new Point(width, height);
        }
    }
}