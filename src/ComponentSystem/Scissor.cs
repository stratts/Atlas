using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Industropolis.Engine
{
    public class Scissor : Component
    {
        public LayoutBorder Bounds { get; set; }
    }

    public class ScissorSystem : BaseComponentSystem<Scissor>
    {
        public ScissorSystem()
        {
            ComponentRemoved += c => DisableScissor(c.Parent);
        }

        public override void UpdateComponents(Scene scene, IReadOnlyList<Scissor> components, float elapsed)
        {
            foreach (var c in components)
            {
                var areaPos = new Vector2(c.Bounds.Left, c.Bounds.Top);
                var areaSize = (c.Parent.Size - areaPos - new Vector2(c.Bounds.Right, c.Bounds.Bottom)).ToPoint();
                EnableScissor(c.Parent, new Rectangle(areaPos.ToPoint(), areaSize));
            }
        }

        private void DisableScissor(Node node)
        {
            if (node.GetComponent<Drawable>() is Drawable d) d.ScissorArea = null;
            foreach (var child in node.Children) DisableScissor(node);
        }

        private void EnableScissor(Node node, Rectangle area)
        {
            if (node.GetComponent<Drawable>() is Drawable d) d.ScissorArea = area;

            foreach (var child in node.Children)
            {
                var offsetArea = area;
                offsetArea.Offset(-child.Position);
                EnableScissor(child, offsetArea);
            }
        }
    }
}