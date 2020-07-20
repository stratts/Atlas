using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Industropolis.Engine
{
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
        public IContainer? Container { get; set; }
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

                if (c.Fill.X > 0) parent.Size = new Vector2(container.Size.X * c.Fill.X, parent.Size.Y);
                if (c.Fill.Y > 0) parent.Size = new Vector2(parent.Size.X, container.Size.Y * c.Fill.Y);

                parent.Position.X = c.HAlign switch
                {
                    HAlign.Left => container.Left,
                    HAlign.Right => container.Right - parent.Size.X,
                    _ => parent.Position.X
                };

                parent.Position.Y = c.VAlign switch
                {
                    VAlign.Top => container.Top,
                    VAlign.Bottom => container.Bottom - parent.Size.Y,
                    _ => parent.Position.Y
                };
            }
        }
    }
}