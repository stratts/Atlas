using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

// A simple pong game made using Atlas

namespace Atlas.Examples.Pong
{
    public enum Controls { MoveUp, MoveDown }

    public class MainScene : Scene
    {
        public MainScene()
        {
            var viewport = Camera.Viewport;

            // Add ball, player paddle, and AI paddle
            var ball = new Ball(Camera.Viewport.Center.ToVector2());
            ball.Reset();
            AddNode(ball);

            var player = new PlayerPaddle() { Position = new Vector2(20, 400) };
            AddNode(player);

            var ai = new AIPaddle(ball);
            ai.Position = new Vector2(Camera.Viewport.Right - 20 - ai.Size.X, 400);
            AddNode(ai);

            // Add top and bottom bounds for ball to bounce on
            var topBounds = new Node()
            {
                Position = new Vector2(-50, -50),
                Size = new Vector2(Camera.Size.X + 100, 50)
            };
            topBounds.AddComponent<Collision>();
            AddNode(topBounds);

            var bottomBounds = new Node()
            {
                Position = new Vector2(-50, Camera.Viewport.Bottom),
                Size = new Vector2(Camera.Size.X + 100, 50)
            };
            bottomBounds.AddComponent<Collision>();
            AddNode(bottomBounds);

            // Add goals on each end
            var playerGoal = new Goal(ai, ball, Camera.Size.Y);
            playerGoal.Position = new Vector2(-50, 0);
            AddNode(playerGoal);

            var aiGoal = new Goal(player, ball, Camera.Size.Y);
            aiGoal.Position = new Vector2(Camera.Viewport.Right + 50, 0);
            AddNode(aiGoal);

            // Add score displays
            var playerScore = new ScoreDisplay(player);
            playerScore.Position = new Vector2(Camera.Viewport.Center.X - 100, 20);
            AddNode(playerScore);

            var aiScore = new ScoreDisplay(ai);
            aiScore.Position = new Vector2(Camera.Viewport.Center.X + 100, 20);
            AddNode(aiScore);

            // Set up keymap
            VKeyboard<Controls>.AddKeys(
                (Controls.MoveUp, Keys.Up),
                (Controls.MoveDown, Keys.Down)
            );
        }

        public override void Update(float elapsed)
        {
            // Update controls using keyboard state
            var kstate = Keyboard.GetState();
            VKeyboard<Controls>.Update(kstate, elapsed);

            base.Update(elapsed);
        }
    }

    public class ScoreDisplay : Node
    {
        private Text _text;
        private Paddle _paddle;

        public ScoreDisplay(Paddle paddle)
        {
            _paddle = paddle;
            _text = new Text()
            {
                Color = Color.Black,
                FontSize = 32
            };

            AddChild(_text);
            AddComponent(new Updateable() { UpdateMethod = Update });
        }

        private void Update(Scene scene, float elapsed)
        {
            _text.Content = _paddle.Score.ToString();
        }
    }

    public class Goal : Node
    {
        private Paddle _target;
        private Ball _ball;

        public Goal(Paddle target, Ball ball, float height)
        {
            Size = new Vector2(50, height);
            _target = target;
            _ball = ball;

            var collision = new Collision() { OnCollision = HandleCollision };
            AddComponent(collision);
        }

        private void HandleCollision(CollisionInfo info)
        {
            if (info.Source.Parent == _ball)
            {
                _ball.Reset();
                _target.Score++;
            }
        }
    }
}
