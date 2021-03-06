using System;
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

        /// <summary> Node position relative to parent (if any) </summary>
        public Vector2 Position;

        /// <summary> Node position relative to scene </summary>
        public Vector2 ScenePosition => Parent != null ? Position + Parent.ScenePosition : Position;

        public virtual Vector2 Size { get; set; } = Vector2.Zero;

        public uint? Layer { get; set; }
        internal ulong SceneSort { get; set; }

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

        public T AddComponent<T>() where T : IComponent, new()
        {
            var c = new T();
            AddComponent(c);
            return c;
        }

        public void RemoveComponent(IComponent component)
        {
            _components.Remove(component);
            ComponentRemoved?.Invoke(component);
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

        public void BringToFront() => BroughtToFront?.Invoke(this);

        public Color GetRenderColor(Color c) => GetComponent<Modulate>() is Modulate m ? m.ModulateColor(c) : c;

        public void AddTag(Tag tag) => _tags = _tags | tag.Value;

        public void RemoveTag(Tag tag) => _tags = _tags ^ tag.Value;

        public bool HasTag(Tag tag) => (_tags & tag.Value) > 0;
    }
}