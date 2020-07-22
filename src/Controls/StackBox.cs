using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Industropolis.Engine
{
    public class StackBox : Node
    {
        private int _spacing;
        private Vector2 _size = Vector2.Zero;
        private Vector2 _currentOffset = Vector2.Zero;
        private Direction _direction;
        private BaseSplitContainer _container;

        public override Vector2 Size
        {
            get { return _size; }
            set { SetSize(value); }
        }

        public enum Direction { Vertical, Horizontal }

        public StackBox(int spacing, Direction direction)
        {
            _spacing = spacing;
            _direction = direction;
            if (_direction == Direction.Horizontal) _container = new ColumnContainer();
            else _container = new RowContainer();
        }

        public override void AddChild(Node node) => AddChild(node, false);

        public void AddChild(Node node, bool stretch)
        {
            base.AddChild(node);

            Layout layout;

            if (node.GetComponent<Layout>() is Layout l) layout = l;
            else
            {
                layout = new Layout();
                node.AddComponent(layout);
            }

            var width = node.Size.X + layout.Margin.Size.X;
            var height = node.Size.Y + layout.Margin.Size.Y;
            float space = _direction == Direction.Horizontal ? width + _spacing : height + _spacing;

            layout.Container = stretch ? _container.AddSection(space) : _container.AddSection(space, space);
            _container.Layout();

            _size = _container.Size;
            if (width > Size.X) Size = new Vector2(width, Size.Y);
            if (height > Size.Y) Size = new Vector2(Size.X, height);
        }

        private void SetSize(Vector2 size)
        {
            _size = size;
            _container.Size = size;
        }

        public void Clear()
        {
            _container.ClearSections();
            for (int i = Children.Count - 1; i >= 0; i--) RemoveChild(Children[i]);
            Size = Vector2.Zero;
        }
    }

    public class HStackBox : StackBox
    {
        public HStackBox(int spacing = 10) : base(spacing, Direction.Horizontal) { }
    }

    public class VStackBox : StackBox
    {
        public VStackBox(int spacing = 10) : base(spacing, Direction.Vertical) { }
    }

}