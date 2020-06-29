using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public class Text : Node, IDrawable
    {
        public string Content { get; set; } = "";
        public Color Color { get; set; } = Color.White;

        public static SpriteFont Font { get; set; } = null!;
        public Vector2 Size => Font.MeasureString(Content);

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.DrawString(Font, Content, position, Color);
        }
    }
}