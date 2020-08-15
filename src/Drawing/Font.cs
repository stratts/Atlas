using System;
using System.Collections.Generic;
using SharpFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public struct Glyph
    {
        public char Character { get; }
        public float Kerning { get; }
        public int Advance { get; }
        public int Size { get; }

        public Glyph(char character, int size, int advance, float kerning = 0)
        {
            Size = size;
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

    public static class FontService
    {
        private static Library _library = new Library();
        private static Face _face = new Face(_library, "Content/FreeSans.ttf");
        private static Dictionary<int, Dictionary<char, GlyphBitmap>> _textures = new Dictionary<int, Dictionary<char, GlyphBitmap>>();

        public static int NominalHeight => _face.Size.Metrics.NominalHeight;
        public static int Ascender => _face.Size.Metrics.Ascender.ToInt32();
        public static int Height => _face.Size.Metrics.Height.ToInt32();


        public static Glyph GetGlyph(char character, int size)
        {
            _face.SetPixelSizes(0, (uint)size);
            var index = _face.GetCharIndex(character);
            var advance = _face.GetAdvance(index, LoadFlags.Default);
            return new Glyph(character, size, advance.ToInt32());
        }

        public static GlyphBitmap GetBitmap(Glyph glyph, GraphicsDevice device)
        {
            var character = glyph.Character;
            bool rendered = GetTextures(glyph.Size).TryGetValue(character, out var texture);
            if (!rendered)
            {
                var index = _face.GetCharIndex(character);
                texture = RenderGlyph(glyph, device);
                GetTextures(glyph.Size)[character] = texture;
            }
            return texture;
        }

        private static Dictionary<char, GlyphBitmap> GetTextures(int size)
        {
            if (!_textures.ContainsKey(size)) _textures[size] = new Dictionary<char, GlyphBitmap>();
            return _textures[size];
        }

        private static GlyphBitmap RenderGlyph(Glyph glyph, GraphicsDevice device)
        {
            _face.SetPixelSizes(0, (uint)glyph.Size);
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