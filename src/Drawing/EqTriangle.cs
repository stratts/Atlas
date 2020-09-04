using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public class EqTriangle : Node
    {
        public Color Color { get; set; }

        public EqTriangle()
        {
            AddComponent(new Drawable() { Draw = Draw });
        }

        public EqTriangle(Vector2 size, Color color)
        {
            Size = size;
            Color = color;
            AddComponent(new Drawable() { Draw = Draw });
        }

        public EqTriangle(Vector2 position, Vector2 size, Color color) : this(size, color)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position) => CustomDrawing.DrawEqTriangle(position, Size, GetRenderColor(Color));
    }
}