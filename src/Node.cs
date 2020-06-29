using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Industropolis
{
    public abstract class Node
    {
        private List<Node> _children = new List<Node>();
        private List<Component> _components = new List<Component>();

        public Vector2 Position;
        public Vector2 ScenePosition => Parent != null ? Position + Parent.ScenePosition : Position;

        public Node? Parent { get; set; }
        public IReadOnlyList<Node> Children => _children;
        public IReadOnlyList<Component> Components => _components;

        public event Action<Component>? ComponentAdded;
        public event Action<Component>? ComponentRemoved;

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

        public void AddComponent(Component component)
        {
            _components.Add(component);
            component.Parent = this;
            ComponentAdded?.Invoke(component);
        }

        public void RemoveComponent(Component component)
        {
            _components.Remove(component);
            ComponentRemoved?.Invoke(component);
        }

        public T? GetComponent<T>() where T : Component
        {
            foreach (var component in _components)
            {
                if (component is T c) return c;
            }
            return null;
        }
    }
}