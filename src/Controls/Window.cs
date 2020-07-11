using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Window : Node
    {
        private int _titleSize = 20;

        public Window(string title, Vector2 size)
        {
            Size = size;
            // Add title bar
            var titleText = new Text()
            {
                Color = Color.White,
                Content = title,
                Position = new Vector2(4)
            };

            _titleSize = (int)titleText.Size.Y + 8;

            var titleBar = new Rect()
            {
                Color = Color.Black,
                Size = new Vector2(Size.X, _titleSize),
                Position = new Vector2(0, -_titleSize)
            };
            titleBar.AddChild(titleText);
            AddChild(titleBar);

            // Add close button
            var closeButton = new Button("  x  ", 2);
            closeButton.Position = new Vector2(titleBar.Size.X - closeButton.Size.X - 2, 2);
            titleBar.AddChild(closeButton);
            closeButton.OnPressed += () => this.Enabled = false;

            // Add titlebar input component for dragging window around
            var input = new MouseInput();
            var inputRect = new Rectangle(Point.Zero, titleBar.Size.ToPoint());
            input.InputArea = inputRect;
            input.OnMove = (Vector2 pos, Vector2 change) =>
            {
                if (input.ButtonHeld)
                {
                    Position += change;
                    input.InputArea = Rectangle.Empty;  // Set input area to capture mouse even if it moves outside
                    input.HandleConsumed = true;        // Capture mouse even if something has already consumed the input
                }
                else
                {
                    input.InputArea = inputRect;
                    input.HandleConsumed = false;
                }
            };
            titleBar.AddComponent(input);

            // Add window background
            var b = 60;
            var background = new Rect()
            {
                Size = Size,
                Color = new Color(b, b, b)
            };
            background.AddComponent(new MouseInput() { InputArea = new Rectangle(Point.Zero, background.Size.ToPoint()) });
            AddChild(background);
        }
    }
}