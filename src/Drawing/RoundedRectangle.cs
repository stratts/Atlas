using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Atlas.CustomDrawing.CircleCorner;

namespace Atlas
{
    public class RoundedRect : Node
    {
        public Color Color { get; set; } = Colors.White;
        public int Radius { get; set; }

        public RoundedRect()
        {
            AddComponent(new Drawable() { Draw = Draw });
            AddComponent<Modulate>();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            var size = Size;
            var color = GetRenderColor(Color);
            var radius = Radius;
            CustomDrawing.DrawRect(position + new Vector2(0, radius), size - new Vector2(0, radius * 2), color);
            CustomDrawing.DrawRect(position + new Vector2(radius, 0), new Vector2(size.X - radius * 2, radius), color);
            CustomDrawing.DrawRect(position + new Vector2(radius, size.Y - radius), new Vector2(size.X - radius * 2, radius), color);
            CustomDrawing.DrawCircleCorner(position, radius, color, TopLeft);
            CustomDrawing.DrawCircleCorner(position + new Vector2(size.X - radius, 0), radius, color, TopRight);
            CustomDrawing.DrawCircleCorner(position + new Vector2(0, size.Y - radius), radius, color, BottomLeft);
            CustomDrawing.DrawCircleCorner(position + new Vector2(size.X - radius, size.Y - radius), radius, color, BottomRight);

        }
    }
}