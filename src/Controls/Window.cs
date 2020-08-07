using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Window : Container
    {
        private int _titleSize;
        private Rect _titleBar;
        private Button _closeButton;
        private Rect _resize;

        public event Action? Closed;

        public Window(string title, Vector2 size) : base(size, new LayoutBorder(10))
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
            };
            _titleBar.AddChild(titleText);
            AddChild(_titleBar);
            _titleBar.AddComponent(new Layout()
            {
                Fill = new Vector2(1, 0),
                Offset = new Vector2(0, -_titleSize),
                IgnorePadding = true
            });
            _titleBar.Size = new Vector2(0, _titleSize);

            // Add close button
            _closeButton = new Button("  x  ", 2);
            _titleBar.AddChild(_closeButton);
            _closeButton.OnPressed += () => this.Enabled = false;
            _closeButton.OnPressed += () => Closed?.Invoke();
            _closeButton.AddComponent(new Layout()
            {
                HAlign = HAlign.Right,
                VAlign = VAlign.Top,
                Margin = new LayoutBorder(right: 2, top: 4)
            });

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

            // Add panel
            var panel = new Panel();
            panel.AddComponent(new Layout() { Fill = new Vector2(1), IgnorePadding = true });
            AddChild(panel);

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
            _resize.AddComponent(new Layout()
            {
                HAlign = HAlign.Right,
                VAlign = VAlign.Bottom,
                IgnorePadding = true
            });
            AddChild(_resize);

            Size = size;
        }
    }
}