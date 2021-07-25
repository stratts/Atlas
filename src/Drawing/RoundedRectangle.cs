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
            var color = GetRenderColor(Color);
            var radius = Radius;
            CustomDrawing.DrawRect(position + new Vector2(0, radius), Size - new Vector2(0, radius * 2), color);
            CustomDrawing.DrawRect(position + new Vector2(radius, 0), new Vector2(Size.X - radius * 2, radius), color);
            CustomDrawing.DrawRect(position + new Vector2(radius, Size.Y - radius), new Vector2(Size.X - radius * 2, radius), color);
            CustomDrawing.DrawCircleCorner(position, radius, color, TopLeft);
            CustomDrawing.DrawCircleCorner(position + new Vector2(Size.X - radius, 0), radius, color, TopRight);
            CustomDrawing.DrawCircleCorner(position + new Vector2(0, Size.Y - radius), radius, color, BottomLeft);
            CustomDrawing.DrawCircleCorner(position + new Vector2(Size.X - radius, Size.Y - radius), radius, color, BottomRight);

        }
    }
}