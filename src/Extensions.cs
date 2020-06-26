using System;
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
        public static float GetHue(this Color c)
        {
            var (h, _, _) = c.GetHsv();
            return h;
        }

        public static void SetHue(this ref Color c, float hue)
        {
            var (_, s, v) = c.GetHsv();
            c.SetHsv(hue, s, v);
        }

        public static float GetSaturation(this Color c)
        {
            var (_, s, _) = c.GetHsv();
            return s;
        }

        public static void SetSaturation(this ref Color c, float saturation)
        {
            var (h, _, v) = c.GetHsv();
            c.SetHsv(h, saturation, v);
        }

        public static float GetValue(this Color c)
        {
            var (_, _, v) = c.GetHsv();
            return v;
        }

        public static void SetValue(this ref Color c, float value)
        {
            var (h, s, _) = c.GetHsv();
            c.SetHsv(h, s, value);
        }

        private static (float h, float s, float v) GetHsv(this Color c) => RgbToHsv(c.R, c.G, c.B);

        private static void SetHsv(this ref Color c, float h, float s, float v)
        {
            var (r, g, b) = HsvToRgb(h, s, v);
            c.R = (byte)r;
            c.G = (byte)g;
            c.B = (byte)b;
        }

        public static (float h, float s, float v) RgbToHsv(float r, float g, float b)
        {
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var c = max - min;

            float hueDeriv = 0;

            if (c == 0) hueDeriv = 0;
            else if (max == r) hueDeriv = ((g - b) / c) % 6;
            else if (max == g) hueDeriv = (b - r) / c + 2;
            else if (max == b) hueDeriv = (r - g) / c + 4;

            float hue = hueDeriv * 60;
            float value = max / 255f;
            float sat = max == 0 ? 0 : (c / 255f) / value;

            return (hue / 360f, sat, value);
        }

        public static (float r, float g, float b) HsvToRgb(float h, float s, float v)
        {
            float c = v * s;
            float hDeriv = (h * 360) / 60;
            float x = c * (1 - Math.Abs(hDeriv % 2 - 1));

            (float r, float g, float b) set1 = (0, 0, 0);

            if (hDeriv <= 1) set1 = (c, x, 0);
            else if (hDeriv <= 2) set1 = (x, c, 0);
            else if (hDeriv <= 3) set1 = (0, c, x);
            else if (hDeriv <= 4) set1 = (0, x, c);
            else if (hDeriv <= 5) set1 = (x, 0, c);
            else if (hDeriv <= 6) set1 = (c, 0, x);

            var m = v - c;

            return ((set1.r + m) * 255, (set1.g + m) * 255, (set1.b + m) * 255);
        }
    }
}