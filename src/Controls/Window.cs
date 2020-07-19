using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Window : Control
    {
        private int _titleSize;
        private Rect _border;
        private Rect _background;
        private Rect _titleBar;
        private Button _closeButton;
        private Rect _resize;

        public event Action? Closed;

        public Window(string title, Vector2 size)
        {
            // Add title bar
            var titleText = new Text()
            {
                Color = Color.White,
                Content = title,
                Position = new Vector2(6)
            };

            _titleSize = (int)titleText.Size.Y + 12;

            _titleBar = new Rect()
            {
                Color = Color.Black,
                Position = new Vector2(0, -_titleSize)
            };
            _titleBar.AddChild(titleText);
            AddChild(_titleBar);

            // Add close button
            _closeButton = new Button("  x  ", 2);
            _titleBar.AddChild(_closeButton);
            _closeButton.OnPressed += () => this.Enabled = false;
            _closeButton.OnPressed += () => Closed?.Invoke();

            // Add titlebar input component for dragging window around
            var input = new MouseInput();
            input.OnMove = (Vector2 pos, Vector2 change) =>
            {
                if (input.ButtonHeld)
                {
                    Position += change;
                    input.CaptureGlobal = true;  // Set input area to capture mouse even if it moves outside
                    input.HandleConsumed = true;        // Capture mouse even if something has already consumed the input
                }
                else
                {
                    input.CaptureGlobal = false;
                    input.HandleConsumed = false;
                }
            };
            input.OnClick = (_) => BringToFront();
            _titleBar.AddComponent(input);

            // Add window background and border
            _border = new Rect()
            {
                Color = Color.Black
            };
            var b = 60;
            _background = new Rect()
            {
                Position = new Vector2(1),
                Color = new Color(b, b, b)
            };
            _background.AddComponent(new MouseInput());
            AddChild(_border);
            AddChild(_background);

            // Add window resize grabber
            _resize = new Rect()
            {
                Color = Color.Gray,
                Size = new Vector2(10)
            };
            var resizeInput = new MouseInput();
            resizeInput.OnMove = (Vector2 pos, Vector2 change) =>
            {
                if (resizeInput.ButtonHeld)
                {
                    var size = Size;
                    if (change.Y < 0 || (change.Y > 0 && pos.Y > _resize.Size.Y)) size.Y += change.Y;
                    if (change.X < 0 || (change.X > 0 && pos.X > _resize.Size.X)) size.X += change.X;
                    Size = new Vector2(Math.Max(size.X, 70), Math.Max(size.Y, 50));
                    resizeInput.CaptureGlobal = true;  // Set input area to capture mouse even if it moves outside
                    resizeInput.HandleConsumed = true; // Capture mouse even if something has already consumed the input
                }
                else
                {
                    resizeInput.CaptureGlobal = false;
                    resizeInput.HandleConsumed = false;
                }
            };
            resizeInput.InputArea = new Rectangle(Point.Zero, new Point(10));
            _resize.AddComponent(resizeInput);
            AddChild(_resize);

            Size = size;
        }

        protected override void OnSizeChanged(Vector2 size)
        {
            _titleBar.Size = new Vector2(size.X, _titleSize);
            _closeButton.Position = new Vector2(_titleBar.Size.X - _closeButton.Size.X - 2, 4);
            _border.Size = size;
            _background.Size = size - new Vector2(2);
            if (_titleBar.GetComponent<MouseInput>() is MouseInput tInput)
            {
                tInput.InputArea = new Rectangle(Point.Zero, _titleBar.Size.ToPoint());
            }
            if (_background.GetComponent<MouseInput>() is MouseInput m)
            {
                m.InputArea = new Rectangle(Point.Zero, _background.Size.ToPoint());
            }
            _resize.Position = size - _resize.Size;
        }
    }
}