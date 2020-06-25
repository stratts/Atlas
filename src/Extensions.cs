using Microsoft.Xna.Framework;

namespace Industropolis
{
    public static class VectorExtensions
    {
        public static Vector2 Floor(this Vector2 v) => new Vector2((int)v.X, (int)v.Y);
    }

    public static class ColorExtensions
    {

    }
}