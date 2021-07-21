using Microsoft.Xna.Framework;

namespace Atlas
{
    public class Panel : Container
    {
        public Panel() : this(Vector2.Zero, new LayoutBorder(10)) { }

        public Panel(Vector2 size, LayoutBorder padding) : base(size, padding)
        {
            // Add background and border
            var border = new Rect()
            {
                Color = Colors.Black
            };
            border.AddComponent(new Layout()
            {
                Fill = new Vector2(1),
                IgnorePadding = true
            });

            var b = 60;
            var background = new Rect()
            {
                Color = new Color(b, b, b)
            };
            background.AddComponent(new MouseInput());
            background.AddComponent(new Layout()
            {
                Margin = new LayoutBorder(1),
                Fill = new Vector2(1),
                IgnorePadding = true,
            });
            AddChild(border);
            AddChild(background);
        }
    }
}