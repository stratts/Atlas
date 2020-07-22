
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Container : Node, IContainer
    {
        public Vector2 Offset { get; }
        public LayoutBorder Padding { get; }

        public Container(Vector2 size, LayoutBorder padding = default(LayoutBorder), Vector2 offset = default(Vector2))
        {
            Size = size;
            Padding = padding;
            Offset = offset;
        }
    }
}