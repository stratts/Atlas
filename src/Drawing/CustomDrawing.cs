using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
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
            _texture.SetData(new Color[] { Colors.White });
            _circleTexture = CreateCircleTexture(64);
            _triangleTexture = CreateTriangleTexture(64);
        }

        public static void DrawRect(Vector2 position, Vector2 size, Color color)
        {
            _spriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
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

        public static void DrawEllipse(Vector2 position, Vector2 size, Color color)
        {
            _spriteBatch.Draw(_circleTexture,
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (int)size.X,
                    (int)size.Y),
                    color);
        }

        public enum CircleCorner { TopLeft, TopRight, BottomLeft, BottomRight }

        public static void DrawCircleCorner(Vector2 position, int size, Color color, CircleCorner corner)
        {
            Point sourcePos = corner switch
            {
                CircleCorner.TopLeft => new Point(0, 0),
                CircleCorner.TopRight => new Point(32, 0),
                CircleCorner.BottomLeft => new Point(0, 32),
                CircleCorner.BottomRight => new Point(32, 32),
                _ => Point.Zero
            };

            _spriteBatch.Draw(
                _circleTexture,
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    size,
                    size),
                new Rectangle(sourcePos, new Point(32)),
                color);
        }

        public static void DrawEqTriangle(Vector2 basePos, Vector2 size, Color color, bool flip = false)
        {
            _spriteBatch.Draw(
                _triangleTexture,
                destinationRectangle: new Rectangle(
                    (int)basePos.X - (int)size.X / 2, (int)basePos.Y - (int)size.Y, (int)size.X, (int)size.Y),
                sourceRectangle: null,
                color: color,
                rotation: 0,
                origin: new Vector2(0.5f, 1f),
                effects: flip ? SpriteEffects.FlipVertically : SpriteEffects.None,
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
                    if (Math.Abs(x - size / 2) <= Math.Abs(y / 2)) colors[idx] = Colors.White;
                    else colors[idx] = Colors.Transparent;
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

                    if (xc * xc + yc * yc <= r * r) colors[idx] = Colors.White;
                    else colors[idx] = Colors.Transparent;
                }
            }

            Texture2D texture = new Texture2D(_spriteBatch.GraphicsDevice, size, size);
            texture.SetData(colors);
            return texture;
        }
    }
}