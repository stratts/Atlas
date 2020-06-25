using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public class Scene
    {
        private List<IDrawable> _drawable = new List<IDrawable>();

        public void AddNode(Node node)
        {
            if (node is IDrawable d) _drawable.Add(d);

            foreach (var child in node.Children) AddNode(child);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos)
        {
            foreach (var d in _drawable) d.Draw(spriteBatch, d.GlobalPosition.Floor() - pos.Floor());
        }
    }
}