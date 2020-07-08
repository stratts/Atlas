using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Button : Node
    {
        public event Action? OnPressed;

        public Button(string label) : this(label, 8) { }

        public Button(string label, int padding)
        {
            var margin = new Vector2(padding);
            var text = new Text()
            {
                Position = margin,
                Content = label,
                Color = Color.White
            };
            var rect = new RoundedRect()
            {
                Color = Color.Black,
                Size = Text.Font.MeasureString(label) + margin * 2,
                Radius = 4
            };
            Size = rect.Size;
            AddChild(rect);
            AddChild(text);

            AddComponent(new MouseInput()
            {
                OnClick = (Vector2 _) => OnPressed?.Invoke(),
                InputArea = new Rectangle(Point.Zero, Size.ToPoint()),
                OnMouseEnter = () => rect.Color = Color.DimGray,
                OnMouseExit = () => rect.Color = Color.Black,
            });
        }
    }
}