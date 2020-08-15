using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public class Text : Node
    {
        public string Content { get; set; } = "";
        public Color Color { get; set; } = Color.White;

        public static SpriteFont Font { get; set; } = null!;
        public override Vector2 Size => new Vector2(Font.MeasureString(Content).X, FontService.NominalHeight);

        public Text()
        {
            AddComponent(new Drawable() { Draw = Draw });
            AddComponent<Modulate>();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Vector2 pos = position + new Vector2(0, FontService.Ascender);
            for (int i = 0; i < Content.Length; i++)
            {
                char c = Content[i];
                if (c == '\n')
                {
                    pos = new Vector2(position.X, pos.Y + FontService.Height);
                    continue;
                }

                Glyph glyph;
                if (i < Content.Length - 1) glyph = FontService.GetGlyph(c, Content[i + 1], spriteBatch.GraphicsDevice);
                else glyph = FontService.GetGlyph(c, spriteBatch.GraphicsDevice);

                spriteBatch.Draw(glyph.Bitmap.Texture, pos - new Vector2(-glyph.Bitmap.Left, glyph.Bitmap.Top), Color.White);
                pos.X += glyph.Advance + glyph.Kerning;
            }
        }
    }
}