using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Button : Node, IContainer
    {
        private LayoutBorder _padding;
        private string _label;

        public Vector2 Offset => Vector2.Zero;
        public LayoutBorder Padding { get => _padding; set => SetPadding(value); }

        public event Action? OnPressed;
        public Action? OnClick { get; set; }

        public Button(string label) : this(label, 8) { }

        public Button(string label, int padding)
        {
            _label = label;

            var text = new Text()
            {
                Content = label,
                Color = Color.White
            };
            text.AddComponent(new Layout());
            var rect = new RoundedRect()
            {
                Color = Color.Black,
                Radius = 4
            };
            rect.AddComponent(new Layout() { Fill = new Vector2(1), IgnorePadding = true });
            AddChild(rect);
            AddChild(text);

            AddComponent(new MouseInput()
            {
                OnClick = (Vector2 _) => { OnPressed?.Invoke(); OnClick?.Invoke(); },
                OnMouseEnter = () => rect.Color = Color.DimGray,
                OnMouseExit = () => rect.Color = Color.Black,
            });

            Padding = new LayoutBorder(padding);
        }

        private void SetPadding(LayoutBorder padding)
        {
            _padding = padding;
            Size = Text.Font.MeasureString(_label) + padding.Size;
        }
    }
}