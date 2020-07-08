using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class StackBox : Node
    {
        private int _spacing;
        private Vector2 _size = Vector2.Zero;
        private Vector2 _currentPos = Vector2.Zero;
        private Direction _direction;

        public enum Direction { Vertical, Horizontal }

        public StackBox(int spacing, Direction direction)
        {
            _spacing = spacing;
            _direction = direction;
        }

        public override void AddChild(Node node)
        {
            base.AddChild(node);

            if (_direction == Direction.Vertical)
            {
                node.Position = new Vector2(0, _currentPos.Y);
                _currentPos.Y += (int)node.Size.Y + _spacing;
                _size.Y += (int)node.Size.Y + _spacing;
                if (node.Size.X > _size.X) _size.X = (int)node.Size.X;
            }
            else
            {
                node.Position = new Vector2(_currentPos.X, 0);
                _currentPos.X += (int)node.Size.X + _spacing;
                _size.X += (int)node.Size.X + _spacing;
                if (node.Size.Y > _size.Y) _size.Y = (int)node.Size.Y;
            }

            Size = _size;
        }

        public void Clear()
        {
            for (int i = Children.Count - 1; i >= 0; i--) RemoveChild(Children[i]);
            _size = Vector2.Zero;
            _currentPos = Vector2.Zero;
        }
    }

}