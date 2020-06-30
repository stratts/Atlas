using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public class EqTriangle : Node, IDrawable
    {
        public Vector2 Size { get; set; }
        public Color Color { get; set; }

        public EqTriangle(Vector2 size, Color color)
        {
            Size = size;
            Color = color;
        }

        public EqTriangle(Vector2 position, Vector2 size, Color color) : this(size, color)
        {
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position) => CustomDrawing.DrawEqTriangle(position, Size, Color * SceneOpacity);
    }
}