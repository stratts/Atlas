using System;
using System.IO;
using System.Collections.Generic;
using SharpFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public struct Glyph
    {
        public char Character { get; }
        public float Kerning { get; }
        public int Advance { get; }

        public Glyph(char character, int advance, float kerning = 0)
        {
            Character = character;
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

    public class Font
    {
        private Dictionary<char, GlyphBitmap> _textureCache = new();
        private Dictionary<uint, Glyph> _glyphCache = new();
        private Face _face;
        private int _size;

        public int NominalHeight { get; }
        public int Ascender { get; }
        public int Height { get; }

        internal Font(Library library, Face face, int size)
        {
            _face = face;
            _face.SetPixelSizes(0, (uint)size);
            _size = size;

            NominalHeight = _face.Size.Metrics.NominalHeight;
            Ascender = _face.Size.Metrics.Ascender.ToInt32();
            Height = _face.Size.Metrics.Height.ToInt32();
        }

        public Glyph GetGlyph(char character)
        {
            var index = _face.GetCharIndex(character);
            var cached = _glyphCache.TryGetValue(index, out var glyph);

            if (!cached)
            {
                var advance = _face.GetAdvance(index, LoadFlags.Default);
                glyph = new Glyph(character, advance.ToInt32());
                _glyphCache[index] = glyph;
            }

            return glyph;
        }

        public GlyphBitmap GetBitmap(Glyph glyph, GraphicsDevice device)
        {
            var character = glyph.Character;
            bool rendered = _textureCache.TryGetValue(character, out var texture);
            if (!rendered)
            {
                var index = _face.GetCharIndex(character);
                texture = RenderGlyph(glyph, device);
                _textureCache[character] = texture;
            }
            return texture;
        }

        private GlyphBitmap RenderGlyph(Glyph glyph, GraphicsDevice device)
        {
            var index = _face.GetCharIndex(glyph.Character);
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
                        colors[i] = Colors.White * ((float)value / 255f);
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

    public static class FontService
    {
        private static Library _library = new Library();
        private static byte[] _fontData = null!;
        private static Dictionary<int, Font> _fonts = new();

        public static void SetFont(string path)
        {
            using (var f = File.OpenRead(path))
            {
                _fontData = new byte[f.Length];
                f.Read(_fontData);
            }
        }

        public static void SetFont(byte[] data) => _fontData = data;

        public static Font GetFont(int size)
        {
            var cached = _fonts.TryGetValue(size, out var font);
            if (font == null || !cached)
            {
                var face = new Face(_library, _fontData, 0);
                font = new Font(_library, face, size);
                _fonts[size] = font;
            }
            return font;
        }
    }
}