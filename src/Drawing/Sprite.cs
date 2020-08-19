using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public class Sprite : Node
    {
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private string _path;
        private Point _size;

        public int? CurrentFrame { get; set; } = null;

        public Sprite(string path, Point size)
        {
            _size = size;
            Size = _size.ToVector2();
            _path = path;
            AddComponent(new Drawable { Draw = Draw });
            AddComponent(new Modulate());
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Texture2D? texture;
            if (!_textures.TryGetValue(_path, out texture))
            {
                using (var f = File.OpenRead(Path.Join(Config.ContentPath, _path)))
                {
                    texture = Texture2D.FromStream(spriteBatch.GraphicsDevice, f);
                    _textures[_path] = texture;
                    if (Size == Vector2.Zero) Size = new Vector2(texture.Width, texture.Height);
                }
            }

            Rectangle sourceRect;
            if (CurrentFrame.HasValue)
            {
                var framesX = texture.Width / _size.X;
                var framePos = new Point(CurrentFrame.Value % framesX, CurrentFrame.Value / framesX);
                sourceRect = new Rectangle(new Point(framePos.X * _size.X, framePos.Y * _size.Y), _size);
            }
            else sourceRect = new Rectangle(Point.Zero, new Point(texture.Width, texture.Height));

            spriteBatch.Draw(
                texture,
                new Rectangle(position.ToPoint(), Size.ToPoint()),
                sourceRect,
                GetRenderColor(Color.White)
            );
        }
    }
}