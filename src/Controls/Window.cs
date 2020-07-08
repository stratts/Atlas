using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Window : Node
    {
        private int _titleSize = 20;
        private Vector2 _size = new Vector2(400);

        public Window(string title, Vector2 size)
        {
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
                Size = new Vector2(_size.X, _titleSize),
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
            var inputRect = new Rectangle(0, 0, (int)(_size.X - closeButton.Size.X - 2), _titleSize);
            input.InputArea = inputRect;
            input.OnMove = (Vector2 pos, Vector2 change) =>
            {
                if (input.ButtonHeld)
                {
                    Position += change;
                    input.InputArea = Rectangle.Empty;  // Set input area to capture mouse even if it moves outside
                }
                else input.InputArea = inputRect;
            };
            titleBar.AddComponent(input);

            // Add window background
            var b = 60;
            AddChild(new Rect()
            {
                Size = _size,
                Color = new Color(b, b, b)
            });
        }
    }
}