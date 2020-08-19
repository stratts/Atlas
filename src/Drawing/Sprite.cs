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

        public Sprite(string path)
        {
            _path = path;
            AddComponent(new Drawable { Draw = Draw });
            AddComponent(new Modulate());
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Texture2D? texture;
            if (!_textures.TryGetValue(_path, out texture))
            {
                using (var f = File.OpenRead(Path.Join("Content", _path)))
                {
                    texture = Texture2D.FromStream(spriteBatch.GraphicsDevice, f);
                    _textures[_path] = texture;
                    if (Size == Vector2.Zero) Size = new Vector2(texture.Width, texture.Height);
                }
            }
            spriteBatch.Draw(texture, new Rectangle(Position.ToPoint(), Size.ToPoint()), GetRenderColor(Color.White));
        }
    }
}