using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Necs;

namespace Atlas
{
    public struct TagCollection
    {
        private ulong _tags;

        public void AddTag(Tag tag) => _tags = _tags | tag.Value;

        public void RemoveTag(Tag tag) => _tags = _tags ^ tag.Value;

        public bool HasTag(Tag tag) => (_tags & tag.Value) > 0;
    }

    public struct Tag
    {
        private static ulong _currentTag = 1;

        public ulong Value { get; }

        private Tag(ulong value) => Value = value;

        internal static Tag New()
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
    public class Node : Entity
    {
        private bool _enabled = true;
        private TagCollection _tags;
        private List<Node> _children = new();

        public List<Node> Children => _children;

        public bool Removed = false;

        public ref Vector2 Position => ref GetComponent<Transform>().Position;
        public ref Vector2 Size => ref GetComponent<Transform>().Size;
        public virtual Rectangle Bounds => GetBounds();
        public bool Enabled { get; set; } = true;

        public bool PlaceInScene { get; set; } = false;
        public uint? Layer { get; set; }

        public event Action<Node>? OnEnabled;
        public event Action<Node>? Deleted;

        public event Action<Node>? BroughtToFront;

        public Node()
        {
            AddComponent(new Transform());
        }

        public override void AddChild(Entity child)
        {
            base.AddChild(child);
            _children.Add((Node)child);
        }

        public override void RemoveChild(Entity child)
        {
            base.RemoveChild(child);
            _children.Remove((Node)child);
        }

        public bool HasComponent<T>()
        {
            try
            {
                GetComponent<T>();
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public ref T AddComponent<T>() where T : new()
        {
            var t = new T();
            AddComponent(t);
            return ref GetComponent<T>();
        }

        public ref T GetOrAddComponent<T>() where T : new()
        {
            try
            {
                return ref GetComponent<T>();
            }
            catch (ArgumentException)
            {
                return ref AddComponent<T>();
            }
        }

        public void Delete() => Deleted?.Invoke(this);

        public void BringToFront() => BroughtToFront?.Invoke(this);

        public void AddTag(Tag tag) => _tags.AddTag(tag);

        public void RemoveTag(Tag tag) => _tags.RemoveTag(tag);

        public bool HasTag(Tag tag) => _tags.HasTag(tag);

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