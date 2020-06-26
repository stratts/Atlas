using Microsoft.Xna.Framework;

namespace Industropolis
{
    public static class VectorExtensions
    {
        public static Vector2 Floor(this Vector2 v) => new Vector2((int)v.X, (int)v.Y);

        public static Vector2 DirectionTo(this Vector2 v, Vector2 dest) => Vector2.Normalize(dest - v);

        public static Vector2 Rotated(this Vector2 v, float rotation)
        {
            var matrix = Matrix.CreateRotationZ(rotation);
            return Vector2.Transform(v, matrix);
        }
    }

    public static class ColorExtensions
    {

    }
}