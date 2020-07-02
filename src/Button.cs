using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Button : Node
    {
        public Vector2 Size { get; }

        public event Action? OnPressed;

        public Button(string label)
        {
            var margin = new Vector2(8);
            var text = new Text()
            {
                Position = margin,
                Content = label,
                Color = Color.White
            };
            var rect = new Rect()
            {
                Color = Color.Black,
                Size = Text.Font.MeasureString(label) + margin * 2
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