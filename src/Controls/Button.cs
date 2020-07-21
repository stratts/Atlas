using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Button : Node, IContainer
    {
        public Vector2 Offset => Vector2.Zero;
        public LayoutBorder Padding { get; private set; }

        public event Action? OnPressed;

        public Button(string label) : this(label, 8) { }

        public Button(string label, int padding)
        {
            Padding = new LayoutBorder(padding);
            var text = new Text()
            {
                Content = label,
                Color = Color.White
            };
            text.AddComponent(new Layout());
            var rect = new RoundedRect()
            {
                Color = Color.Black,
                Size = Text.Font.MeasureString(label) + Padding.Size,
                Radius = 4
            };
            rect.AddComponent(new Layout() { Fill = new Vector2(1), IgnorePadding = true });
            Size = rect.Size;
            AddChild(rect);
            AddChild(text);

            AddComponent(new MouseInput()
            {
                OnClick = (Vector2 _) => OnPressed?.Invoke(),
                OnMouseEnter = () => rect.Color = Color.DimGray,
                OnMouseExit = () => rect.Color = Color.Black,
            });
        }
    }
}