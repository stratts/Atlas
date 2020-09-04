using System;
using Microsoft.Xna.Framework;

namespace Atlas
{
    public class Button : Node, IContainer
    {
        private LayoutBorder _padding;
        private Node _label;

        public Vector2 Offset => Vector2.Zero;
        public LayoutBorder Padding { get => _padding; set => SetPadding(value); }

        public event Action? OnPressed;
        public Action? OnClick { get; set; }

        private Color _baseColor = Color.Black;
        private Color _hoverColor = Color.DimGray;

        public Button(string label, int padding = 8)
            : this(new Text() { Content = label, Color = Color.White }, padding) { }

        public Button(Node label, int padding = 8)
        {
            _label = label;

            if (!(_label.GetComponent<Layout>() is Layout l)) _label.AddComponent(new Layout());
            var rect = new RoundedRect()
            {
                Color = _baseColor,
                Radius = 4
            };
            rect.AddComponent(new Layout() { Fill = new Vector2(1), IgnorePadding = true });
            AddChild(rect);
            AddChild(_label);

            AddComponent(new MouseInput()
            {
                OnClick = (Vector2 _) => { OnPressed?.Invoke(); OnClick?.Invoke(); },
                OnMouseEnter = () => rect.Color = _hoverColor,
                OnMouseExit = () => rect.Color = _baseColor,
            });

            Padding = new LayoutBorder(padding);
        }

        public void SetColor(Color color)
        {
            _baseColor = color;
            _hoverColor = new Color(color.R + 80, color.G + 80, color.B + 80);
            foreach (var child in Children)
            {
                if (child is RoundedRect r) r.Color = color;
            }
        }

        private void SetPadding(LayoutBorder padding)
        {
            _padding = padding;
            Size = _label.Size + padding.Size;
        }
    }
}