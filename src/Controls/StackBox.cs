using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Necs;
using static Atlas.StackBox.Direction;

namespace Atlas
{
    public class StackBox : Node, IEnumerable<Node>
    {
        public class BasicContainer : IContainer
        {
            private Vector2 _size;

            public Vector2 Offset { get; set; }
            public LayoutBorder Padding { get; set; }
            public ref Vector2 Size => ref _size;
        }

        public class Expand { }

        private int _spacing;
        private Vector2 _currentOffset = Vector2.Zero;
        private Direction _direction;
        private Dictionary<Node, (float? min, float? max)> _sizes = new Dictionary<Node, (float?, float?)>();

        public void SetWidth(int width)
        {
            SetSize(new Vector2(width, Size.Y));
        }

        public int Spacing
        {
            get => _spacing;
            set { _spacing = value; Layout(); }
        }

        public enum Direction { Vertical, Horizontal }

        public StackBox(int spacing, Direction direction)
        {
            AddComponent(new Updateable() { UpdateMethod = Update });
            _spacing = spacing;
            _direction = direction;
        }

        public void AddChildren(params Node?[] children)
        {
            foreach (var c in children)
            {
                if (c != null) AddChild(c);
            }
        }

        public override void AddChild(Entity node) => AddChild((Node)node, null, null);

        public void Add(Node node) => AddChild(node);

        public void AddChild(Node node, float? minSize = null, float? maxSize = null)
        {
            base.AddChild(node);
            _sizes[node] = (minSize, maxSize);
            Layout();
        }

        protected void Layout()
        {
            if (_direction == Direction.Horizontal) Size.X = 0;
            else Size.Y = 0;
            float s = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                var node = Children[i];
                if (!node.Enabled) continue;
                var (min, max) = _sizes[node];
                ref var layout = ref node.GetOrAddComponent<Layout>();

                var width = node.Size.X + layout.Margin.Size.X;
                var height = node.Size.Y + layout.Margin.Size.Y;
                var size = _direction == Direction.Horizontal ? width : height;

                float minSpace = min.HasValue ? min.Value : size;
                float? maxSpace = !max.HasValue && !node.HasComponent<Expand>() ? size : max;

                layout.Container = new BasicContainer()
                {
                    Offset = new Vector2(_direction == Horizontal ? s : 0, _direction == Vertical ? s : 0),
                    Size = new Vector2(_direction == Horizontal ? width : Size.X, _direction == Vertical ? height : Size.Y)
                };

                if (width > Size.X) Size = new Vector2(width, Size.Y);
                if (height > Size.Y) Size = new Vector2(Size.X, height);

                s += size;
                if (i < Children.Count - 1) s += Spacing;
            }

            if (_direction == Horizontal) Size.X = s;
            else Size.Y = s;
        }

        private void SetSize(Vector2 size)
        {
            Size = size;
        }

        public void Update(Scene scene, float elapsed)
        {

        }

        public void Clear()
        {
            _sizes.Clear();
            for (int i = Children.Count - 1; i >= 0; i--) RemoveChild(Children[i]);
            Size = Vector2.Zero;
        }

        public IEnumerator<Node> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
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