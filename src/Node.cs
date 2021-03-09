using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;

namespace Atlas
{
    public struct Tag
    {
        private static ulong _currentTag = 1;

        public ulong Value { get; }

        private Tag(ulong value) => Value = value;

        public static Tag New()
        {
            var curr = _currentTag;
            _currentTag *= 2;
            return new Tag(curr);
        }

        public static Tag operator |(Tag a, Tag b) => new Tag(a.Value | b.Value);
    }

    /// <summary>
    /// Base node class, to which components and child nodes can be added
    /// </summary>
    public class Node
    {
        private List<Node> _children = new List<Node>();
        private List<IComponent> _components = new List<IComponent>();
        private bool _enabled = true;
        private ulong _tags = 0;
        private Rectangle? _bounds;

        /// <summary> Node position relative to parent (if any) </summary>
        public Vector2 Position;
        public virtual Vector2 Centre
        {
            get => Position + Size / 2;
            set => Position = value - Size / 2;
        }

        /// <summary> Node position relative to scene </summary>
        public Vector2 ScenePosition => Parent != null ? Position + Parent.ScenePosition : Position;
        public uint? SceneLayer => RootNode != null ? RootNode.Layer : Layer;

        public virtual Vector2 Size { get; set; } = Vector2.Zero;
        public virtual Rectangle Bounds => GetBounds();

        public uint? Layer { get; set; }
        internal ulong SceneSort { get; set; }
        internal Vector2 LastPos { get; set; }

        public int Depth => Parent != null ? Parent.Depth + 1 : 0;

        public bool Enabled
        {
            get => _enabled && Parent != null ? Parent.Enabled : _enabled;
            set
            {
                _enabled = value;
                if (_enabled) OnEnabled?.Invoke(this);
            }
        }

        public Node? RootNode { get; internal set; }
        public Node? Parent { get; internal set; }
        public List<Node> Children => _children;
        public IReadOnlyList<IComponent> Components => _components;

        public event Action<Node>? ChildAdded;
        public event Action<Node>? ChildRemoved;

        public event Action<IComponent>? ComponentAdded;
        public event Action<IComponent>? ComponentRemoved;

        public event Action<Node>? OnEnabled;
        public event Action<Node>? Deleted;

        public event Action<Node>? BroughtToFront;

        public virtual void AddChild(Node node)
        {
            node.Parent = this;
            node.RootNode = RootNode != null ? RootNode : this;
            _children.Add(node);
            ChildAdded?.Invoke(node);
        }

        public void RemoveChild(Node node)
        {
            node.Parent = null;
            node.RootNode = null;
            _children.Remove(node);
            ChildRemoved?.Invoke(node);
        }

        public void Delete() => Deleted?.Invoke(this);

        public void AddComponent(IComponent component)
        {
            _components.Add(component);
            component.Parent = this;
            ComponentAdded?.Invoke(component);
        }

        public T AddComponent<T>(string? name = null) where T : IComponent, new()
        {
            var c = new T();
            if (name != null) c.Name = name;
            AddComponent(c);
            return c;
        }

        public void RemoveComponent(IComponent component)
        {
            component.Enabled = false;
            _components.Remove(component);
            ComponentRemoved?.Invoke(component);
        }

        public void RemoveComponent(string name)
        {
            foreach (var component in ((IEnumerable<IComponent>)_components).Reverse())
            {
                if (component.Name == name)
                {
                    RemoveComponent(component);
                }
            }
        }

        [return: MaybeNull]
        public T GetComponent<T>() where T : IComponent
        {
            foreach (var component in _components)
            {
                if (component is T c) return c;
            }
            return default(T);
        }

        [return: MaybeNull]
        public T GetComponentByName<T>(string name) where T : IComponent
        {
            foreach (var component in _components)
            {
                if (component.Name == name && component is T c) return c;
            }
            return default(T);
        }

        public void BringToFront() => BroughtToFront?.Invoke(this);

        public Color GetRenderColor(Color c) => GetComponent<Modulate>() is Modulate m ? m.ModulateColor(c) : c;

        public void AddTag(Tag tag) => _tags = _tags | tag.Value;

        public void RemoveTag(Tag tag) => _tags = _tags ^ tag.Value;

        public bool HasTag(Tag tag) => (_tags & tag.Value) > 0;

        private Rectangle GetBounds()
        {
            var bounds = Size.ToRectangle();
            foreach (var child in Children)
            {
                var childBounds = child.Bounds;
                childBounds.Offset(child.Position);
                bounds = Rectangle.Union(bounds, childBounds);
            }
            return bounds;
        }
    }
}