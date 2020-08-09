using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Button : Node, IContainer
    {
        private LayoutBorder _padding;
        private Node _label;

        public Vector2 Offset => Vector2.Zero;
        public LayoutBorder Padding { get => _padding; set => SetPadding(value); }

        public event Action? OnPressed;
        public Action? OnClick { get; set; }

        public Button(string label, int padding = 8)
            : this(new Text() { Content = label, Color = Color.White }, padding) { }

        public Button(Node label, int padding = 8)
        {
            _label = label;

            _label.AddComponent(new Layout());
            var rect = new RoundedRect()
            {
                Color = Color.Black,
                Radius = 4
            };
            rect.AddComponent(new Layout() { Fill = new Vector2(1), IgnorePadding = true });
            AddChild(rect);
            AddChild(_label);

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
            Size = _label.Size + padding.Size;
        }
    }
}