using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public class Rect : Node, IDrawable
    {
        public Color Color { get; set; } = Color.White;
        public new Vector2 Size { get; set; }

        public Rect() { }

        public Rect(Vector2 size, Color color)
        {
            Size = size;
            Color = color;
        }

        public Rect(Vector2 position, Vector2 size, Color color) : this(size, color)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position) =>
            CustomDrawing.DrawRect(position, Size, GetRenderColor(Color));
    }
}