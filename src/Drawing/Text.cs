using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public class Text : Node
    {
        public string Content { get; set; } = "";
        public Color Color { get; set; } = Color.White;

        public static SpriteFont Font { get; set; } = null!;
        public override Vector2 Size => Font.MeasureString(Content);

        public Text()
        {
            AddComponent(new Drawable() { Draw = Draw });
            AddComponent<Modulate>();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.DrawString(Font, Content, position, GetRenderColor(Color));
        }
    }
}