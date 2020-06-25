using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Industropolis
{
    public class Node
    {
        private List<Node> _children = new List<Node>();

        public Vector2 Position { get; set; }
        public Vector2 GlobalPosition => Parent != null ? Position + Parent.Position : Position;

        public Node? Parent { get; set; }
        public IReadOnlyList<Node> Children => _children;

        public void AddChild(Node node)
        {
            node.Parent = this;
            _children.Add(node);
        }

        public void RemoveChild(Node node)
        {
            node.Parent = null;
            _children.Remove(node);
        }
    }
}