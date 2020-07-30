using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public abstract class CustomDrawingNode : Node
    {
        private Vector2 _screenPos;
        private bool boundsSet = false;
        private Drawable _drawable;

        protected Drawable Drawable => _drawable;

        public CustomDrawingNode()
        {
            _drawable = new Drawable() { Draw = Draw };
            AddComponent(_drawable);
            AddComponent<Modulate>();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            _screenPos = position;
            _drawable.DrawBounds = Rectangle.Empty;
            boundsSet = false;
            Draw();
        }

        public abstract void Draw();

        public void DrawRect(Vector2 position, Vector2 size, Color color)
        {
            CustomDrawing.DrawRect(_screenPos + position, size, color);
            UpdateDrawBounds(position, size);
        }

        public void DrawRect(Vector2 centrePos, Vector2 size, Color color, float angle)
        {
            CustomDrawing.DrawRect(_screenPos + centrePos, size, GetRenderColor(color), angle);
            UpdateDrawBounds(centrePos - size / 2, size);
        }

        public void DrawCircle(Vector2 position, int radius, Color color)
        {
            CustomDrawing.DrawCircle(_screenPos + position, radius, GetRenderColor(color));
            UpdateDrawBounds(position - new Vector2(radius), new Vector2(radius) * 2);
        }

        public void DrawEllipse(Vector2 position, Vector2 size, Color color)
        {
            CustomDrawing.DrawEllipse(_screenPos + position, size, color);
            UpdateDrawBounds(position, size);
        }

        public void DrawEqTriangle(Vector2 basePos, Vector2 size, Color color)
        {
            CustomDrawing.DrawEqTriangle(_screenPos + basePos, size, GetRenderColor(color));
            UpdateDrawBounds(basePos - new Vector2(size.X / 2, size.Y), size);
        }

        public void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            CustomDrawing.DrawLine(_screenPos + start, _screenPos + end, width, GetRenderColor(color));
            UpdateDrawBounds(start - new Vector2(width / 2), new Vector2(width));
            UpdateDrawBounds(end - new Vector2(width / 2), new Vector2(width));
        }

        private void UpdateDrawBounds(Vector2 pos, Vector2 size)
        {
            var bounds = _drawable.DrawBounds;
            int left = bounds.Left, right = bounds.Right, up = bounds.Top, down = bounds.Bottom;
            if (!boundsSet || pos.X < left) left = (int)pos.X;
            if (!boundsSet || pos.Y < up) up = (int)pos.Y;
            if (!boundsSet || pos.X + size.X > right) right = (int)(pos.X + size.X);
            if (!boundsSet || pos.Y + size.Y > down) down = (int)(pos.Y + size.Y);
            _drawable.DrawBounds = new Rectangle(left, up, right - left, down - up);
            boundsSet = true;
        }
    }
}