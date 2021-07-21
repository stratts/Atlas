using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public class RoundedRect : Node
    {
        public Color Color { get; set; } = Colors.White;
        public int Radius { get; set; }

        public RoundedRect()
        {
            AddComponent(new Drawable() { Draw = Draw });
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            var color = GetRenderColor(Color);
            var radius = Radius;
            var circleSize = new Vector2(radius * 2);
            CustomDrawing.DrawRect(position + new Vector2(0, radius), Size - new Vector2(0, radius * 2), color);
            CustomDrawing.DrawRect(position + new Vector2(radius, 0), Size - new Vector2(radius * 2, 0), color);
            CustomDrawing.DrawEllipse(position, circleSize, color);
            CustomDrawing.DrawEllipse(position + new Vector2(Size.X - circleSize.X, 0), circleSize, color);
            CustomDrawing.DrawEllipse(position + new Vector2(0, Size.Y - circleSize.Y), circleSize, color);
            CustomDrawing.DrawEllipse(position + Size - circleSize, circleSize, color);
        }
    }
}