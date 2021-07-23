using Microsoft.Xna.Framework;

namespace Atlas
{
    public class Ellipse : CustomDrawingNode
    {
        public Color Color { get; set; }

        public override void Draw() => DrawEllipse(-Size / 2, Size, Color);
    }
}