using System;
using Microsoft.Xna.Framework;

namespace Atlas.Examples.Pong
{
    public abstract class Paddle : Node
    {
        public enum Move { None, Up, Down };

        public int Score { get; set; } = 0;

        public Paddle()
        {
            Size = new Vector2(30, 120);

            // Coloured rectangle to represent paddle
            var paddleRect = new Rect()
            {
                Color = Color.Black,
                Size = Size
            };
            AddChild(paddleRect);

            // Update method that will be called every frame
            var update = new Updateable() { UpdateMethod = Update };
            AddComponent(update);

            // Collision component so the ball can collide with the paddle
            AddComponent<Collision>();
        }

        private void Update(Scene scene, float elapsed)
        {
            var move = DecideMove();    // Subclasses implement decision logic
            var speed = 300f;
            var viewport = scene.Camera.Viewport;

            // Move paddle up or down
            switch (move)
            {
                case Move.Up:
                    Position.Y -= speed * elapsed; break;
                case Move.Down:
                    Position.Y += speed * elapsed; break;
                default:
                    break;
            }

            // ...but not any further than the top or bottom of the screen
            if (BoundingBox.Top < viewport.Top) Position.Y = 0;
            if (BoundingBox.Bottom > viewport.Bottom) Position.Y = viewport.Bottom - Size.Y;
        }

        protected abstract Move DecideMove();
    }

    public class PlayerPaddle : Paddle
    {
        protected override Move DecideMove()
        {
            var state = VKeyboard<Controls>.GetState();

            if (state.IsHeld(Controls.MoveUp)) return Move.Up;
            else if (state.IsHeld(Controls.MoveDown)) return Move.Down;
            else return Move.None;
        }
    }

    public class AIPaddle : Paddle
    {
        private Ball _ball;

        public AIPaddle(Ball ball)
        {
            _ball = ball;
        }

        protected override Move DecideMove()
        {
            // Roughly calculate where ball will be when it reaches paddle
            var dist = Math.Abs(_ball.ScenePosition.X - ScenePosition.X);
            var yChange = (dist / Math.Abs(_ball.Velocity.X)) * _ball.Velocity.Y;
            var pos = _ball.ScenePosition.Y + yChange;

            var center = BoundingBox.Center;
            if (center.Y + 5 < pos) return Move.Down;
            else if (center.Y - 5 > pos) return Move.Up;
            else return Move.None;
        }
    }
}