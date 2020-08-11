using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
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

        public static float Angle(this Vector2 v)
        {
            var n = Vector2.Normalize(v);
            return (float)Math.Atan2(n.Y, n.X);
        }

        public static Vector2 Normalized(this Vector2 v) => Vector2.Normalize(v);

        public static Vector2 Round(this Vector2 v) => new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));

        public static Vector2 ToXna(this System.Numerics.Vector2 v) => new Vector2(v.X, v.Y);

        public static Rectangle ToRectangle(this Vector2 v) => new Rectangle(Point.Zero, v.ToPoint());
    }

    public struct ColorHsv
    {
        private float _hue, _saturation, _value;

        public float Hue { get => _hue; set => _hue = Math.Clamp(value, 0, 1); }
        public float Saturation { get => _saturation; set => _saturation = Math.Clamp(value, 0, 1); }
        public float Value { get => _value; set => _value = Math.Clamp(value, 0, 1); }

        public ColorHsv(float hue, float saturation, float value)
        {
            _hue = hue;
            _saturation = saturation;
            _value = value;
        }

        public Color ToRgb()
        {
            var (r, g, b) = HsvToRgb(Hue, Saturation, Value);
            return new Color((byte)Math.Round(r), (byte)Math.Round(g), (byte)Math.Round(b));
        }

        private static (float r, float g, float b) HsvToRgb(float h, float s, float v)
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

    public static class ColorExtensions
    {
        public static ColorHsv ToHsv(this Color c)
        {
            var (h, s, v) = RgbToHsv(c.R, c.G, c.B);
            return new ColorHsv(h, s, v);
        }

        private static (float h, float s, float v) RgbToHsv(float r, float g, float b)
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
    }
}