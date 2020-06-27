using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Industropolis
{
    public static class CustomDrawing
    {
        private static Texture2D _circleTexture = null!;
        private static Texture2D _texture = null!;
        private static SpriteBatch _spriteBatch = null!;

        public static void Init(SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            _texture.SetData(new Color[] { Color.White });
            _circleTexture = CreateCircleTexture(128);
        }

        public static void DrawRect(Vector2 position, Vector2 size, Color color)
        {
            _spriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
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

        public static void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            var dist = end - start;
            var length = dist.Length();
            var distN = Vector2.Normalize(dist);
            var angle = (float)Math.Atan2(distN.Y, distN.X);

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