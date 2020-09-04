
using Microsoft.Xna.Framework;

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
    }
}