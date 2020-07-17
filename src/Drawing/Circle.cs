using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Circle : CustomDrawingNode
    {
        public int Radius { get; set; }
        public Color Color { get; set; }

        public override void Draw() => DrawCircle(new Vector2(Radius / 2), Radius, Color);
    }
}