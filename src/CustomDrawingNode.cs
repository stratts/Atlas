
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public abstract class CustomDrawingNode : Node, IDrawable
    {
        private Vector2 _screenPos;

        public abstract Vector2 Size { get; protected set; }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            _screenPos = position;
            Draw();
        }

        public abstract void Draw();

        public void DrawRect(Vector2 position, Vector2 size, Color color) => DrawRect(position, size, color);

        public void DrawRect(Vector2 centrePos, Vector2 size, Color color, float angle) =>
            CustomDrawing.DrawRect(_screenPos + Position, size, color * SceneOpacity, angle);

        public void DrawCircle(Vector2 position, int radius, Color color) =>
            CustomDrawing.DrawCircle(_screenPos + position, radius, color * SceneOpacity);

        public void DrawEqTriangle(Vector2 basePos, Vector2 size, Color color) =>
            CustomDrawing.DrawEqTriangle(_screenPos + basePos, size, color * SceneOpacity);

        public void DrawLine(Vector2 start, Vector2 end, int width, Color color) =>
            CustomDrawing.DrawLine(_screenPos + start, _screenPos + end, width, color * SceneOpacity);
    }
}