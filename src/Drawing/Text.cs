using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public class Text : Node
    {
        private Font _font;
        private int _fontSize = 16;
        private string _content = "";

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                Size = MeasureString(value);
            }
        }
        public Color Color { get; set; } = Colors.White;
        public int FontSize
        {
            get => _fontSize;
            set
            {
                _fontSize = value;
                _font = FontService.GetFont(value);
            }
        }

        public Text()
        {
            AddComponent(new Drawable() { Draw = Draw });
            AddComponent<Modulate>();
            _font = FontService.GetFont(FontSize);
        }

        public int LineHeight => _font.Height;

        public int IndexAt(float x)
        {
            Vector2 pos = new Vector2(0, _font.Ascender);
            int idx = 0;
            foreach (var c in Content)
            {
                var glyph = _font.GetGlyph(c);
                pos.X += glyph.Advance + glyph.Kerning;
                if (pos.X > x) break;
                idx++;
            }
            return idx;
        }

        public Vector2 MeasureString(ReadOnlySpan<char> s)
        {
            Vector2 pos = new Vector2(0, _font.Ascender);
            foreach (var c in s)
            {
                if (c == '\n')
                {
                    pos = new Vector2(0, pos.Y + _font.Height);
                    continue;
                }
                var glyph = _font.GetGlyph(c);
                pos.X += glyph.Advance + glyph.Kerning;
            }
            return new Vector2(pos.X, pos.Y + _font.NominalHeight - _font.Ascender);
        }

        private void Draw(SpriteBatch spriteBatch, DrawContext ctx)
        {
            Vector2 pos = ctx.Position + new Vector2(0, _font.Ascender);
            foreach (var c in Content)
            {
                if (c == '\n')
                {
                    pos = new Vector2(ctx.Position.X, pos.Y + _font.Height);
                    continue;
                }
                var glyph = _font.GetGlyph(c);

                var bitmap = _font.GetBitmap(glyph, spriteBatch.GraphicsDevice);
                spriteBatch.Draw(bitmap.Texture, pos - new Vector2(-bitmap.Left, bitmap.Top), ctx.Modulate.ModulateColor(Color));

                pos.X += glyph.Advance + glyph.Kerning;
            }
        }
    }
}