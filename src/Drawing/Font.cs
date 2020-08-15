using System;
using System.Collections.Generic;
using SharpFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public struct Glyph
    {
        public float Kerning { get; }
        public int Advance { get; }
        public GlyphBitmap Bitmap { get; }

        public Glyph(GlyphBitmap texture, int advance, float kerning = 0)
        {
            Bitmap = texture;
            Advance = advance;
            Kerning = kerning;
        }
    }

    public struct GlyphBitmap
    {
        public Texture2D Texture { get; }
        public int Top { get; }
        public int Left { get; }

        public GlyphBitmap(Texture2D texture, int top, int left)
        {
            Texture = texture;
            Top = top;
            Left = left;
        }
    }

    public static class FontService
    {
        private static Library _library = new Library();
        private static Face _face = new Face(_library, "Content/FreeSans.ttf");
        private static Dictionary<char, GlyphBitmap> _textures = new Dictionary<char, GlyphBitmap>();

        public static int NominalHeight => _face.Size.Metrics.NominalHeight;
        public static int Ascender => _face.Size.Metrics.Ascender.ToInt32();
        public static int Height => _face.Size.Metrics.Height.ToInt32();

        public static void Init()
        {
            _face.SetPixelSizes(0, 16);
        }

        public static Glyph GetGlyph(char character, GraphicsDevice device)
        {
            var index = _face.GetCharIndex(character);
            var advance = _face.GetAdvance(index, LoadFlags.Default);
            return new Glyph(GetBitmap(character, device), advance.ToInt32());
        }

        public static Glyph GetGlyph(char character, char next, GraphicsDevice device)
        {
            _face.SetPixelSizes(0, 16);
            var index = _face.GetCharIndex(character);
            var advance = _face.GetAdvance(index, LoadFlags.Default);
            var kerning = _face.GetKerning(index, _face.GetCharIndex(next), KerningMode.Default);
            return new Glyph(GetBitmap(character, device), advance.ToInt32(), kerning.X.ToSingle());
        }

        private static GlyphBitmap GetBitmap(char character, GraphicsDevice device)
        {
            bool rendered = _textures.TryGetValue(character, out var texture);
            if (!rendered)
            {
                var index = _face.GetCharIndex(character);
                texture = RenderGlyph(index, device);
                _textures[character] = texture;
            }
            return texture;
        }

        private static GlyphBitmap RenderGlyph(uint index, GraphicsDevice device)
        {
            _face.LoadGlyph(index, LoadFlags.Render, LoadTarget.Normal);
            var bitmap = _face.Glyph.Bitmap;
            Texture2D texture;
            if (bitmap.Rows > 0 && bitmap.Width > 0)
            {
                var data = bitmap.BufferData;

                Color[] colors = new Color[bitmap.Width * bitmap.Rows];

                int i = 0;
                for (int y = 0; y < bitmap.Rows; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        var value = data[bitmap.Width * y + x];
                        colors[i] = Color.White * ((float)value / 255f);
                        i++;
                    }
                }

                texture = new Texture2D(device, bitmap.Width, bitmap.Rows);
                texture.SetData(colors);
            }
            else texture = new Texture2D(device, 1, 1);

            return new GlyphBitmap(texture, _face.Glyph.BitmapTop, _face.Glyph.BitmapLeft);
        }
    }
}