
using Microsoft.Xna.Framework;

namespace Industropolis
{
    public class Camera : Node
    {
        public Point Size;

        public Camera(int width, int height)
        {
            Size = new Point(width, height);
        }
    }
}