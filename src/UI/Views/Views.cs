using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine.UI.Views
{
    public class TextView : View
    {
        private Text _node = new Text();

        public string Content { get => _node.Content; set => _node.Content = value; }

        protected override Node Node => _node;

        public TextView Color(Color c) => Modify(this, () => _node.Color = c);
    }

    public class ButtonView : View
    {
        private Button _button;

        protected override Node Node => _button;

        public ButtonView(string label) => _button = new Button(label);

        public ButtonView OnClick(Action onClick)
        {
            _button.OnClick = onClick;
            return this;
        }
    }

    public class BoxView : View
    {
        private StackBox _box;

        protected override Node Node => _box;

        public BoxView(StackBox.Direction direction, params View?[] children)
        {
            _box = new StackBox(10, direction);
            AddChildren(children);
        }

        public void AddChild(View view) => _box.AddChild(view.GetNode());

        private BoxView AddChildren(IEnumerable<View?>? children)
        {
            if (children != null)
            {
                foreach (var c in children)
                {
                    if (c != null)
                    {
                        if (c is MultiView m) AddChildren(m.Views);
                        else AddChild(c);
                    }
                }
            }
            return this;
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

    public class MultiView : View
    {
        protected override Node Node => throw new NotImplementedException();
        public IEnumerable<View> Views { get; }

        public MultiView(IEnumerable<View> views)
        {
            Views = views;
        }
    }
}