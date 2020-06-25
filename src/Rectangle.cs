using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public class Rect : Node, IDrawable
    {
        public Vector2 Size { get; set; }
        public Color Color { get; set; }

        private static Texture2D? _texture;

        public Rect(Vector2 position, Vector2 size, Color color)
        {
            Position = position;
            Size = size;
            Color = color;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _texture.SetData(new Color[] { Color.White });
            }

            spriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int)Size.Y), Color);
        }
    }
}