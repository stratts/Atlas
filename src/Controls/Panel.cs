using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Panel : Node, IContainer
    {
        public LayoutBorder Padding { get; } = new LayoutBorder(10);
        public Vector2 Offset => Vector2.Zero;

        public Panel()
        {
            // Add background and border
            var border = new Rect()
            {
                Color = Color.Black
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