using Microsoft.Xna.Framework;

namespace Atlas
{
    public class Circle : CustomDrawingNode
    {
        private int _radius = 0;

        public int Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                Size = new Vector2(Radius * 2);
            }
        }
        public Color Color { get; set; }

        public override void Draw() => DrawCircle(new Vector2(Radius / 2), Radius, Color);
    }
}