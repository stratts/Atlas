using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Node
    {
        private List<Node> _children = new List<Node>();
        private List<Component> _components = new List<Component>();
        private bool _enabled = true;

        public Vector2 Position;
        public Vector2 ScenePosition => Parent != null ? Position + Parent.ScenePosition : Position;

        public virtual Vector2 Size { get; set; } = Vector2.Zero;

        public int Layer { get; set; } = -1;
        public int Sort { get; set; }
        public int Depth { get; set; } = 0;
        public float SceneSort { get; set; }

        public bool Enabled
        {
            get => _enabled && Parent != null ? Parent.Enabled : _enabled;
            set
            {
                _enabled = value;
                if (_enabled) OnEnabled?.Invoke(this);
            }
        }

        public float Opacity { get; set; } = 1f;
        public Color Tint { get; set; } = Color.White;

        public Node? Parent { get; set; }
        public IReadOnlyList<Node> Children => _children;
        public IReadOnlyList<Component> Components => _components;

        public event Action<Node>? ChildAdded;
        public event Action<Node>? ChildRemoved;

        public event Action<Component>? ComponentAdded;
        public event Action<Component>? ComponentRemoved;

        public event Action<Node>? OnEnabled;
        public event Action<Node>? Deleted;

        public event Action<Node>? BroughtToFront;

        public virtual void AddChild(Node node)
        {
            node.Depth = Depth + 1;
            node.Sort = Children.Count + 1;
            node.Parent = this;
            _children.Add(node);
            ChildAdded?.Invoke(node);
        }

        public void RemoveChild(Node node)
        {
            node.Parent = null;
            _children.Remove(node);
            ChildRemoved?.Invoke(node);
        }

        public void Delete() => Deleted?.Invoke(this);

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

        public void BringToFront() => BroughtToFront?.Invoke(this);

        protected Color GetRenderColor(Color color)
        {
            var opacity = Parent != null ? Opacity * Parent.Opacity : Opacity;
            var tint = Parent != null && Parent.Tint != Color.White ? Parent.Tint : Tint;
            var c = color;
            if (tint != Color.White)
            {
                var tHsv = tint.ToHsv();
                var cHsv = c.ToHsv();
                cHsv.Hue = tHsv.Hue;
                cHsv.Saturation = tHsv.Saturation;
                c = cHsv.ToRgb();
            }
            return c * opacity;
        }
    }
}