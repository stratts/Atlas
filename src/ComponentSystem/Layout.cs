using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Industropolis.Engine
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
    }

    public interface IContainer
    {
        Vector2 Offset { get; }
        LayoutBorder Padding { get; }
        Vector2 Size { get; }

        Vector2 PaddedSize => Size - Padding.Size;
        float Left => Offset.X + Padding.Left;
        float Right => Offset.X + Size.X - Padding.Right;
        float Top => Offset.Y + Padding.Top;
        float Bottom => Offset.Y + Size.Y - Padding.Bottom;
    }

    public enum HAlign { None, Left, Right, Centre }
    public enum VAlign { None, Top, Bottom }

    public class Layout : Component
    {
        public Vector2 Offset { get; set; }
        public IContainer? Container { get; set; }
        public LayoutBorder Margin;
        public bool IgnorePadding { get; set; } = false;
        public HAlign HAlign { get; set; }
        public VAlign VAlign { get; set; }
        public Vector2 Fill { get; set; } = Vector2.Zero;
    }

    public class LayoutSystem : BaseComponentSystem<Layout>
    {
        private struct Container : IContainer
        {
            public Vector2 Offset { get; }
            public LayoutBorder Padding => LayoutBorder.None;
            public Vector2 Size { get; }

            public Container(Vector2 size) : this(Vector2.Zero, size) { }

            public Container(Vector2 offset, Vector2 size)
            {
                Offset = offset;
                Size = size;
            }
        }

        protected override int SortMethod(Layout a, Layout b) => a.Priority.CompareTo(b.Priority);

        public override void UpdateComponents(Scene scene, IReadOnlyList<Layout> components, float elapsed)
        {
            foreach (var c in components)
            {
                if (!c.Enabled || c.Parent == null) continue;
                var parent = c.Parent;

                IContainer container;
                if (c.Container != null) container = c.Container;
                else if (parent.Parent is IContainer p) container = p;
                else if (parent.Parent != null) container = new Container(parent.Parent.Size);
                else continue;

                if (c.IgnorePadding) container = new Container(container.Offset, container.Size);

                if (c.Fill.X > 0)
                {
                    parent.Size = new Vector2((container.PaddedSize.X - c.Margin.Left - c.Margin.Right) * c.Fill.X, parent.Size.Y);
                }
                if (c.Fill.Y > 0)
                {
                    parent.Size = new Vector2(parent.Size.X, (container.PaddedSize.Y - c.Margin.Top - c.Margin.Bottom) * c.Fill.Y);
                }

                parent.Position.X = c.Offset.X + c.HAlign switch
                {
                    HAlign.Left => container.Left + c.Margin.Left,
                    HAlign.Right => container.Right - parent.Size.X - c.Margin.Right,
                    HAlign.Centre => container.Left + c.Margin.Left + (container.PaddedSize.X / 2 - c.Parent.Size.X / 2),
                    _ => container.Left + c.Margin.Left
                };

                parent.Position.Y = c.Offset.Y + c.VAlign switch
                {
                    VAlign.Top => container.Top + c.Margin.Top,
                    VAlign.Bottom => container.Bottom - parent.Size.Y - c.Margin.Bottom,
                    _ => container.Top + c.Margin.Top
                };
            }
        }
    }
}