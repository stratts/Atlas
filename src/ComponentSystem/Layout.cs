using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Necs;

namespace Atlas
{
    public struct LayoutBorder
    {
        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }

        public Vector2 Size => new Vector2(Left + Right, Top + Bottom);

        public static LayoutBorder None => new LayoutBorder();

        public LayoutBorder(float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public LayoutBorder(float border) : this(border, border, border, border) { }

        public static bool operator ==(LayoutBorder a, LayoutBorder b) => (a.Left, a.Right, a.Top, a.Bottom) == (b.Left, b.Right, b.Top, b.Bottom);
        public static bool operator !=(LayoutBorder a, LayoutBorder b) => !(a == b);

        public override bool Equals(object? other)
        {
            if (!(other is LayoutBorder l)) return false;
            else return this == l;
        }

        public override int GetHashCode() => (Left, Right, Top, Bottom).GetHashCode();
    }

    public interface IContainer
    {
        Vector2 Offset { get; }
        LayoutBorder Padding { get; }
        ref Vector2 Size { get; }

        Vector2 PaddedSize => Size - Padding.Size;
        float Left => Offset.X + Padding.Left;
        float Right => Offset.X + Size.X - Padding.Right;
        float Top => Offset.Y + Padding.Top;
        float Bottom => Offset.Y + Size.Y - Padding.Bottom;
    }

    public enum HAlign { Default, None, Left, Right, Centre }
    public enum VAlign { Default, None, Top, Bottom, Centre }

    public class Layout
    {
        public Vector2 Offset { get; set; }
        public IContainer? Container { get; set; }
        public LayoutBorder Margin;
        public bool IgnorePadding { get; set; } = false;
        public HAlign HAlign { get; set; }
        public VAlign VAlign { get; set; }
        public Vector2 Fill { get; set; } = Vector2.Zero;
    }

    public class LayoutSystem : IComponentSystem<UpdateContext>
    {
        private class Container : IContainer
        {
            private Vector2 _size;

            public Vector2 Offset { get; }
            public LayoutBorder Padding => LayoutBorder.None;
            public ref Vector2 Size => ref _size;
            public Container(Vector2 size) : this(Vector2.Zero, size) { }

            public Container(Vector2 offset, Vector2 size)
            {
                Offset = offset;
                Size = size;
            }
        }

        public void Process(UpdateContext context, IEcsContext ecs) => ecs.QueryParent<Layout, Transform>(Query);

        private void Query(ref Layout c, ref Transform transform, ref Transform parent, bool hasParent)
        {
            //Console.WriteLine(hasParent);
            IContainer container;
            if (c.Container != null) container = c.Container;
            else if (hasParent) container = new Container(parent.Size);
            else return;

            if (c.IgnorePadding) container = new Container(container.Offset, container.Size);

            if (c.Fill.X > 0)
            {
                transform.Size = new Vector2((container.PaddedSize.X - c.Margin.Left - c.Margin.Right) * c.Fill.X, transform.Size.Y);
            }
            if (c.Fill.Y > 0)
            {
                transform.Size = new Vector2(transform.Size.X, (container.PaddedSize.Y - c.Margin.Top - c.Margin.Bottom) * c.Fill.Y);
            }

            transform.Position.X = c.Offset.X + c.HAlign switch
            {
                HAlign.Left => container.Left + c.Margin.Left,
                HAlign.Right => container.Right - transform.Size.X - c.Margin.Right,
                HAlign.Centre => container.Left + c.Margin.Left + (container.PaddedSize.X / 2 - transform.Size.X / 2),
                HAlign.None => transform.Position.X,
                _ => container.Left + c.Margin.Left
            };

            transform.Position.Y = c.Offset.Y + c.VAlign switch
            {
                VAlign.Top => container.Top + c.Margin.Top,
                VAlign.Bottom => container.Bottom - transform.Size.Y - c.Margin.Bottom,
                VAlign.Centre => container.Top + c.Margin.Top + (container.PaddedSize.Y / 2 - transform.Size.Y / 2),
                VAlign.None => transform.Position.Y,
                _ => container.Top + c.Margin.Top
            };
        }
    }
}