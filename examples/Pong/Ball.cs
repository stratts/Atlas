using System;
using Microsoft.Xna.Framework;

namespace Atlas.Examples.Pong
{
    public class Ball : Node
    {
        private Random _random = new Random();
        private Vector2 _startPos;
        private Vector2 _velocity;
        private const float _speed = 800;

        public Vector2 Velocity => _velocity;

        public Ball(Vector2 startPos)
        {
            var radius = 10;
            Position = startPos;
            Size = new Vector2(radius * 2);
            _startPos = startPos;

            // Colored circle to represent ball
            var circle = new Circle()
            {
                Color = Color.Black,
                Radius = radius
            };
            AddChild(circle);

            // Update method called every frame
            var update = new Updateable() { UpdateMethod = Update };
            AddComponent(update);

            // Collision component that calls method when collision occurs
            var collision = new Collision() { OnCollision = HandleCollision };
            AddComponent(collision);
        }

        public void Reset()
        {
            // Move ball to start position and launch towards random side
            Position = _startPos - (Size / 2);
            var dir = _random.NextDouble() > 0.5 ? 1 : -1;
            var yVelocity = (float)_random.NextDouble() - 0.5f;
            _velocity = new Vector2(dir, yVelocity).Normalized() * _speed * 0.7f;
        }

        private void Update(Scene scene, float elapsed)
        {
            Position += _velocity * elapsed;
        }

        private void HandleCollision(CollisionInfo info)
        {
            // Bounce and reposition beyond collision area
            switch (info.Direction)
            {
                case Collision.Direction.Left:
                    _velocity.X = -_velocity.X;
                    Position.X = info.Coordinate;
                    break;
                case Collision.Direction.Right:
                    _velocity.X = -_velocity.X;
                    Position.X = info.Coordinate - Size.X;
                    break;
                case Collision.Direction.Top:
                    _velocity.Y = -_velocity.Y;
                    Position.Y = info.Coordinate;
                    break;
                case Collision.Direction.Bottom:
                    _velocity.Y = -_velocity.Y;
                    Position.Y = info.Coordinate - Size.Y;
                    break;
            }

            // If collided with paddle, set velocity based on position of ball relative to paddle
            if (info.Source.Parent is Paddle paddle)
            {
                var bounceVector = (BoundingBox.Center - paddle.BoundingBox.Center).ToVector2().Normalized();
                _velocity = (_velocity.Normalized() + bounceVector).Normalized() * _speed;
            }
        }
    }
}