using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public class Sprite : Node
    {
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private Point _size;
        private Texture2D _texture;

        public int? CurrentFrame { get; set; } = null;
        public bool HFlip { get; set; } = false;

        public Sprite(string path, Point size)
        {
            Texture2D? texture;
            if (!_textures.TryGetValue(path, out texture))
            {
                texture = Texture2D.FromFile(Config.GraphicsDevice, Path.Join(Config.ContentPath, path));
                _textures[path] = texture;
                if (Size == Vector2.Zero) Size = new Vector2(texture.Width, texture.Height);
            }
            _texture = texture;

            _size = size;
            Size = _size.ToVector2();

            AddComponent(new Drawable { Draw = Draw });
            AddComponent(new Modulate());
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Rectangle sourceRect;
            if (CurrentFrame.HasValue)
            {
                var framesX = _texture.Width / _size.X;
                var framePos = new Point(CurrentFrame.Value % framesX, CurrentFrame.Value / framesX);
                sourceRect = new Rectangle(new Point(framePos.X * _size.X, framePos.Y * _size.Y), _size);
            }
            else sourceRect = new Rectangle(Point.Zero, new Point(_texture.Width, _texture.Height));

            spriteBatch.Draw(
                _texture,
                new Rectangle(position.ToPoint(), Size.ToPoint()),
                sourceRect,
                GetRenderColor(Color.White),
                rotation: 0,
                origin: Vector2.Zero,
                HFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0
            );
        }
    }
}