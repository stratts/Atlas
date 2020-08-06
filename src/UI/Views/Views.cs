using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine.UI.Views
{
    public interface IPaddableView
    {
        void SetPadding(LayoutBorder padding);
    }

    public class TextView : View
    {
        private Text _node = new Text();

        public string Content { get => _node.Content; set => _node.Content = value; }

        protected override Node Node => _node;

        public TextView Color(Color c) => Modify(this, () => _node.Color = c);
    }

    public class ButtonView : View, IPaddableView
    {
        private Button _button;

        protected override Node Node => _button;

        public ButtonView(string label) => _button = new Button(label);

        public ButtonView OnClick(Action onClick)
        {
            _button.OnClick = onClick;
            return this;
        }

        void IPaddableView.SetPadding(LayoutBorder padding) => _button.Padding = padding;
    }

    public class BoxView : View
    {
        private StackBox _box;

        protected override Node Node => _box;

        public BoxView(StackBox.Direction direction, params View?[] children)
        {
            _box = new StackBox(10, direction);

            foreach (var c in children)
            {
                if (c != null) _box.AddChild(c.GetNode());
            }
        }
    }

    public class HBoxView : BoxView
    {
        public HBoxView(params View?[] children) : base(StackBox.Direction.Horizontal, children) { }
    }

    public class VBoxView : BoxView
    {
        public VBoxView(params View?[] children) : base(StackBox.Direction.Vertical, children) { }
    }

    public class NodeView : View
    {
        protected override Node Node { get; }

        public NodeView(Node node) => Node = node;
    }

    public class ContainerView : View, IPaddableView
    {
        private View _view;
        private Container _container;
        protected override Node Node => _container;

        public ContainerView(View view, LayoutBorder padding = default(LayoutBorder))
        {
            _view = view;
            var node = view.GetNode();
            var container = new Container(view.Size + padding.Size, padding);
            container.AddChild(node);
            _container = container;
        }

        public ContainerView Background(Color color)
        {
            _container.AddComponent(new Drawable()
            {
                Draw = (_, position) => CustomDrawing.DrawRect(position, Size, color)
            });
            return this;
        }

        void IPaddableView.SetPadding(LayoutBorder padding)
        {
            _container.Size = _view.Size + padding.Size;
            _container.Padding = padding;
        }
    }
}