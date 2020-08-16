using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public class Text : Node
    {
        public string Content { get; set; } = "";
        public Color Color { get; set; } = Color.White;
        public int FontSize { get; set; } = 16;

        public override Vector2 Size => MeasureString(Content);

        public Text()
        {
            AddComponent(new Drawable() { Draw = Draw });
            AddComponent<Modulate>();
        }

        public int LineHeight
        {
            get
            {
                FontService.SetSize(FontSize);
                return FontService.Height;
            }
        }

        public int IndexAt(float x)
        {
            FontService.SetSize(FontSize);
            Vector2 pos = new Vector2(0, FontService.Ascender);
            int idx = 0;
            foreach (var c in Content)
            {
                var glyph = FontService.GetGlyph(c);
                pos.X += glyph.Advance + glyph.Kerning;
                if (pos.X > x) break;
                idx++;
            }
            return idx;
        }

        public Vector2 MeasureString(ReadOnlySpan<char> s)
        {
            FontService.SetSize(FontSize);
            Vector2 pos = new Vector2(0, FontService.Ascender);
            foreach (var c in s)
            {
                if (c == '\n')
                {
                    pos = new Vector2(0, pos.Y + FontService.Height);
                    continue;
                }
                var glyph = FontService.GetGlyph(c);
                pos.X += glyph.Advance + glyph.Kerning;
            }
            return new Vector2(pos.X, pos.Y + FontService.NominalHeight - FontService.Ascender);
        }

        private void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            FontService.SetSize(FontSize);
            Vector2 pos = position + new Vector2(0, FontService.Ascender);
            foreach (var c in Content)
            {
                if (c == '\n')
                {
                    pos = new Vector2(position.X, pos.Y + FontService.Height);
                    continue;
                }
                var glyph = FontService.GetGlyph(c);

                var bitmap = FontService.GetBitmap(glyph, spriteBatch.GraphicsDevice);
                spriteBatch.Draw(bitmap.Texture, pos - new Vector2(-bitmap.Left, bitmap.Top), Color);

                pos.X += glyph.Advance + glyph.Kerning;
            }
        }
    }
}