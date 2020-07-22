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

        public override void AddChild(Node node) => AddChild(node);

        public void AddChild(Node node, float minSize = -1, float maxSize = -1)
        {
            base.AddChild(node);

            Layout layout;

            if (node.GetComponent<Layout>() is Layout l) layout = l;
            else layout = node.AddComponent<Layout>();

            var width = node.Size.X + layout.Margin.Size.X;
            var height = node.Size.Y + layout.Margin.Size.Y;
            var size = _direction == Direction.Horizontal ? width : height;

            if (maxSize == -1) maxSize = size + _spacing;
            else maxSize = maxSize + _spacing;
            if (minSize == -1) minSize = size + _spacing;
            else minSize = minSize + _spacing;

            layout.Container = _container.AddSection(minSize, maxSize);
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