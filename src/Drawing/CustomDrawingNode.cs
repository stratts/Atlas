using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public abstract class CustomDrawingNode : Node
    {
        private Vector2 _screenPos;
        private Modulate _modulate = null!;
        private bool boundsSet = false;
        private ref Drawable _drawable => ref GetComponent<Drawable>();

        protected ref Drawable Drawable => ref _drawable;

        public CustomDrawingNode()
        {
            AddComponent(new Drawable() { Draw = Draw });
            AddComponent<Modulate>();
        }

        public void Draw(SpriteBatch spriteBatch, DrawContext context)
        {
            _modulate = context.Modulate;
            _screenPos = context.Position;
            _drawable.DrawBounds = Rectangle.Empty;
            boundsSet = false;
            Draw();
        }

        public abstract void Draw();

        public void DrawRect(Vector2 position, Vector2 size, Color color)
        {
            CustomDrawing.DrawRect(_screenPos + position, size, _modulate.ModulateColor(color));
            UpdateDrawBounds(position, size);
        }

        public void DrawRect(Vector2 centrePos, Vector2 size, Color color, float angle)
        {
            CustomDrawing.DrawRect(_screenPos + centrePos, size, _modulate.ModulateColor(color), angle);
            UpdateDrawBounds(centrePos - size / 2, size);
        }

        public void DrawCircle(Vector2 position, int radius, Color color)
        {
            CustomDrawing.DrawCircle(_screenPos + position, radius, _modulate.ModulateColor(color));
            UpdateDrawBounds(position - new Vector2(radius), new Vector2(radius) * 2);
        }

        public void DrawEllipse(Vector2 position, Vector2 size, Color color)
        {
            CustomDrawing.DrawEllipse(_screenPos + position, size, _modulate.ModulateColor(color));
            UpdateDrawBounds(position, size);
        }

        public void DrawEqTriangle(Vector2 basePos, Vector2 size, Color color, bool flip = false)
        {
            CustomDrawing.DrawEqTriangle(_screenPos + basePos, size, _modulate.ModulateColor(color), flip);
            UpdateDrawBounds(basePos - new Vector2(size.X / 2, size.Y), size);
        }

        public void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            CustomDrawing.DrawLine(_screenPos + start, _screenPos + end, width, _modulate.ModulateColor(color));
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