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
        Vector2 Offset => Vector2.Zero;
        Vector2 Size { get; }
        float Left => Offset.X;
        float Right => Offset.X + Size.X;
        float Top => Offset.Y;
        float Bottom => Offset.Y + Size.Y;
    }

    public enum HAlign { None, Left, Right }
    public enum VAlign { None, Top, Bottom }

    public class Layout : Component
    {
        public Vector2 Offset { get; set; }
        public IContainer? Container { get; set; }
        public LayoutBorder Margin;
        public HAlign HAlign { get; set; }
        public VAlign VAlign { get; set; }
        public Vector2 Fill { get; set; } = Vector2.Zero;
    }

    public class LayoutSystem : BaseComponentSystem<Layout>
    {
        public override void UpdateComponents(Scene scene, IReadOnlyList<Layout> components, float elapsed)
        {
            foreach (var c in components)
            {
                var parent = c.Parent;
                var container = c.Container == null && parent != null ? parent.Parent : c.Container;
                if (container == null || parent == null) continue;

                if (c.Fill.X > 0)
                {
                    parent.Size = new Vector2((container.Size.X - c.Margin.Left - c.Margin.Right) * c.Fill.X, parent.Size.Y);
                }
                if (c.Fill.Y > 0)
                {
                    parent.Size = new Vector2(parent.Size.X, (container.Size.Y - c.Margin.Top - c.Margin.Bottom) * c.Fill.Y);
                }

                parent.Position.X = c.HAlign switch
                {
                    HAlign.Left => container.Left + c.Margin.Left,
                    HAlign.Right => container.Right - parent.Size.X - c.Margin.Right,
                    _ => c.Offset.X + c.Margin.Left
                };

                parent.Position.Y = c.VAlign switch
                {
                    VAlign.Top => container.Top + c.Margin.Top,
                    VAlign.Bottom => container.Bottom - parent.Size.Y - c.Margin.Bottom,
                    _ => c.Offset.Y + c.Margin.Top
                };
            }
        }
    }
}