using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public class Scene
    {
        private List<IDrawable> _drawable = new List<IDrawable>();
        protected Vector2 _camera;

        public void AddNode(Node node)
        {
            if (node is IDrawable d) _drawable.Add(d);

            foreach (var child in node.Children) AddNode(child);
        }

        public virtual void Update(float elapsed)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var d in _drawable)
            {
                var pos = d.GlobalPosition.Floor() - _camera.Floor();

                if (!(pos.Y + d.Size.Y < 0 || pos.X + d.Size.X < 0 || pos.X > 1280 || pos.Y > 720))
                    d.Draw(spriteBatch, pos);
            }
        }
    }
}
