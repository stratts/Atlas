
using Microsoft.Xna.Framework;
using Necs;

namespace Atlas
{
    public class Container : Node, IContainer
    {
        public Vector2 Offset { get; set; }
        public LayoutBorder Padding { get; set; }

        public Container(Vector2 size, LayoutBorder padding = default(LayoutBorder), Vector2 offset = default(Vector2))
        {
            Size = size;
            Padding = padding;
            Offset = offset;
        }

        public override void AddChild(Entity child)
        {
            base.AddChild(child);
            ((Node)child).GetOrAddComponent<Layout>().Container = this;
        }
    }
}