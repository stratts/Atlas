using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Industropolis.Engine
{
    public class StackBox : Node
    {
        private class Container : IContainer
        {
            public Vector2 Offset { get; set; }
            public Vector2 Size { get; set; }
            public bool Stretch { get; set; } = false;
        }

        private int _spacing;
        private Vector2 _size = Vector2.Zero;
        private Vector2 _currentOffset = Vector2.Zero;
        private Direction _direction;
        private List<Container> _containers = new List<Container>();

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

            var container = new Container();
            container.Stretch = stretch;
            container.Size = node.Size + layout.Margin.Size;

            if (_direction == Direction.Vertical)
            {
                container.Offset = new Vector2(0, _currentOffset.Y);
                _currentOffset.Y += container.Size.Y + _spacing;
                _size.Y += container.Size.Y + _spacing;
                if (node.Size.X > _size.X) _size.X = (int)node.Size.X;
            }
            else
            {
                container.Offset = new Vector2(_currentOffset.X, 0);
                _currentOffset.X += container.Size.X + _spacing;
                _size.X += container.Size.X + _spacing;
                if (node.Size.Y > _size.Y) _size.Y = (int)node.Size.Y;
            }

            _containers.Add(container);
            layout.Container = container;
            Size = _size;
        }

        private void SetSize(Vector2 size)
        {
            if (_size == size) return;
            _size = size;
            _currentOffset = Vector2.Zero;

            var stretchContainers = _containers.Where(c => c.Stretch);

            if (_direction == Direction.Horizontal)
            {
                float fixedWidth = _containers.Where(c => !c.Stretch).Select(c => c.Size.X).Sum();
                float flexWidth = Size.X - fixedWidth;

                int stretchCount = stretchContainers.Count();

                foreach (var container in stretchContainers)
                {
                    container.Size = new Vector2(flexWidth / stretchCount, container.Size.Y);
                }

                foreach (var container in _containers)
                {
                    container.Offset = new Vector2(_currentOffset.X, 0);
                    System.Console.WriteLine($"Set offset {container.Offset}");
                    _currentOffset.X += container.Size.X + _spacing;
                }
            }
            else
            {
                foreach (var container in stretchContainers)
                {
                    container.Size = new Vector2(Size.X, container.Size.Y);
                }
            }
        }

        public void Clear()
        {
            _containers.Clear();
            for (int i = Children.Count - 1; i >= 0; i--) RemoveChild(Children[i]);
            Size = Vector2.Zero;
            _currentOffset = Vector2.Zero;
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