using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public class Rect : Node
    {
        public Color Color { get; set; } = Colors.White;

        public Rect()
        {
            AddComponent(new Drawable() { Draw = Draw });
            AddComponent<Modulate>();
        }

        public Rect(Vector2 size, Color color) : this()
        {
            Size = size;
            Color = color;
        }

        public Rect(Vector2 position, Vector2 size, Color color) : this(size, color)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, DrawContext ctx) =>
            CustomDrawing.DrawRect(ctx.Position, ctx.Size, ctx.Modulate.ModulateColor(Color));
    }
}