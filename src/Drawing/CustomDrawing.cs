using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public static class CustomDrawing
    {
        private static Texture2D _circleTexture = null!;
        private static Texture2D _triangleTexture = null!;
        private static Texture2D _texture = null!;
        private static SpriteBatch _spriteBatch = null!;

        public static void Init(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.White });
            _circleTexture = CreateCircleTexture(64);
            _triangleTexture = CreateTriangleTexture(64);
        }

        public static void DrawRect(Vector2 position, Vector2 size, Color color)
        {
            DrawRect(position + size / 2, size, color, 0);
        }

        public static void DrawRect(Vector2 centrePos, Vector2 size, Color color, float angle)
        {
            _spriteBatch.Draw(
                _texture,
                destinationRectangle: new Rectangle((int)centrePos.X, (int)centrePos.Y, (int)size.X, (int)size.Y),
                sourceRectangle: null,
                color: color,
                rotation: angle,
                origin: new Vector2(0.5f),
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }

        public static void DrawCircle(Vector2 position, int radius, Color color)
        {
            _spriteBatch.Draw(_circleTexture,
                new Rectangle(
                    (int)position.X - radius,
                    (int)position.Y - radius,
                    radius * 2,
                    radius * 2),
                    color);
        }

        public static void DrawEqTriangle(Vector2 basePos, Vector2 size, Color color)
        {
            _spriteBatch.Draw(
                _triangleTexture,
                destinationRectangle: new Rectangle((int)basePos.X, (int)basePos.Y, (int)size.X, (int)size.Y),
                sourceRectangle: null,
                color: color,
                rotation: 0,
                origin: new Vector2(0.5f, 1f),
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }

        public static void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            var dist = end - start;
            var length = dist.Length();
            var angle = dist.Angle();

            _spriteBatch.Draw(
                _texture,
                destinationRectangle: new Rectangle((int)start.X, (int)start.Y, (int)length, width),
                sourceRectangle: null,
                color: color,
                rotation: angle,
                origin: new Vector2(0, 0.5f),
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }

        private static Texture2D CreateTriangleTexture(int size)
        {
            var colors = new Color[size * size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var idx = x + size * y;
                    if (Math.Abs(x - size / 2) <= Math.Abs(y / 2)) colors[idx] = Color.White;
                    else colors[idx] = Color.Transparent;
                }
            }

            Texture2D texture = new Texture2D(_spriteBatch.GraphicsDevice, size, size);
            texture.SetData(colors);
            return texture;
        }

        private static Texture2D CreateCircleTexture(int size)
        {
            var colors = new Color[size * size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    var idx = x + size * y;
                    var r = size / 2;
                    var xc = x - r;
                    var yc = y - r;

                    if (xc * xc + yc * yc <= r * r) colors[idx] = Color.White;
                    else colors[idx] = Color.Transparent;
                }
            }

            Texture2D texture = new Texture2D(_spriteBatch.GraphicsDevice, size, size);
            texture.SetData(colors);
            return texture;
        }
    }
}