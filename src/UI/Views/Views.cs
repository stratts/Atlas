using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Atlas.UI.Views
{
    public interface IPaddableView
    {
        void SetPadding(LayoutBorder padding);
    }

    public class TextView : View
    {
        private Text _node = new Text();

        public string Content { get => _node.Content; set => _node.Content = value; }

        internal override Node Node => _node;

        public TextView Color(Color c) => Modify(this, () => _node.Color = c);

        public TextView FontSize(int size) => Modify(this, () => _node.FontSize = size);

        public TextView Bind(Func<string> getContent)
        {
            _node.AddComponent(new Updateable() { UpdateMethod = (scene, _) => Content = getContent() });
            return this;
        }
    }

    public class ButtonView : View, IPaddableView
    {
        private Button _button;

        internal override Node Node => _button;

        public ButtonView(string label, Action? onClick)
        {
            _button = new Button(label);
            _button.OnClick = onClick;
        }

        public ButtonView(View label, Action? onClick)
        {
            _button = new Button(label.GetNode());
            _button.OnClick = onClick;
        }

        public ButtonView Color(Color c) => Modify(this, () => _button.SetColor(c));

        void IPaddableView.SetPadding(LayoutBorder padding) => _button.Padding = padding;
    }

    public class BoxView : View
    {
        internal StackBox _box;

        internal override Node Node => _box;

        public BoxView(StackBox.Direction direction, params View?[] children)
        {
            _box = new StackBox(10, direction);

            foreach (var c in children)
            {
                if (c != null) _box.AddChild(c.GetNode());
            }
        }

        public BoxView Spacing(int spacing) { _box.Spacing = spacing; return this; }
    }

    public class HBoxView : BoxView
    {
        public HBoxView(params View?[] children) : base(StackBox.Direction.Horizontal, children) { }
    }

    public class VBoxView : BoxView
    {
        public VBoxView(params View?[] children) : base(StackBox.Direction.Vertical, children) { }

        public VBoxView Width(int width) => Modify(this, () => _box.SetWidth(width));
    }

    public class NodeView : View
    {
        internal override Node Node { get; }

        public NodeView(Node node) => Node = node;
    }

    public class PanelView : View, IPaddableView
    {
        private View _view;
        private Panel _panel;
        private Vector2 _size;

        internal override Node Node => _panel;

        public PanelView(View view, LayoutBorder? padding = null)
        {
            _view = view;
            var node = view.GetNode();
            if (padding == null) padding = new LayoutBorder(10);
            _size = view.Size;
            var panel = new Panel(_size + padding.Value.Size, padding.Value);
            panel.AddChild(node);
            _panel = panel;
        }

        public PanelView WithSize(Vector2 size)
        {
            _size = size;
            _panel.Size = size + _panel.Padding.Size;
            return this;
        }

        void IPaddableView.SetPadding(LayoutBorder padding)
        {
            _panel.Size = _size + padding.Size;
            _panel.Padding = padding;
        }
    }

    public class ContainerView : View, IPaddableView
    {
        private Color _background = Colors.Transparent;
        private View _view;
        private Container _container;
        internal override Node Node => _container;

        public Color BackgroundColor { get; private set; }

        public ContainerView(View view, LayoutBorder padding = default(LayoutBorder))
        {
            _view = view;
            var node = view.GetNode();
            var container = new Container(view.Size + padding.Size, padding);
            container.AddChild(node);
            _container = container;
            _container.AddComponent(new Drawable()
            {
                Draw = (_, ctx) => CustomDrawing.DrawRect(ctx.Position, Size, BackgroundColor)
            });
        }

        public ContainerView Height(int height)
        {
            _container.Size = new Vector2(_container.Size.X, height);
            return this;
        }

        public ContainerView Background(Color color)
        {
            _background = color;
            BackgroundColor = _background;
            return this;
        }

        public ContainerView HoverBackground(Color color)
        {
            _container.AddComponent(new MouseInput()
            {
                ConsumeInput = false,
                OnMouseEnter = () => BackgroundColor = color,
                OnMouseExit = () => BackgroundColor = _background
            });
            return this;
        }

        void IPaddableView.SetPadding(LayoutBorder padding)
        {
            _container.Size = _view.Size + padding.Size;
            _container.Padding = padding;
        }
    }

    public class TextInputView : View
    {
        private TextInput _input = new TextInput();

        internal override Node Node => _input;

        public TextInputView Bind(Action<string> set, Func<string>? get = null) { _input.Bind(set, get); return this; }

        public TextInputView Color(Color color) { _input.Color = color; return this; }

        public TextInputView EnableInput(bool enabled) => Modify(this, () => _input.EnableInput = enabled);

        public TextInputView NumbersOnly() { _input.NumbersOnly = true; return this; }
    }
}