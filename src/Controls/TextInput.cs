using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Industropolis.Engine
{
    public class TextInput : CustomDrawingNode
    {
        private Text _text;
        private int _pointer = 0;
        private KeyboardState prevState;
        private (int, int)? _selected;

        public bool EnableInput { get; set; } = true;

        public string Content
        {
            get => _text.Content;
            set
            {
                _text.Content = value;
                _pointer = value.Length;
                _selected = (0, value.Length);
            }
        }

        public Color Color { get => _text.Color; set => _text.Color = value; }

        public TextInput()
        {
            _text = new Text();
            AddChild(_text);

            if (Config.CurrentWindow != null)
            {
                Config.CurrentWindow.TextInput += (obj, args) =>
                {
                    HandleInput(args.Character, args.Key);
                };
            }

            AddComponent(new Updateable() { UpdateMethod = Update });

            var input = new MouseInput();
            input.OnClick = pos =>
            {
                _selected = null;
                _pointer = _text.IndexAt(pos.X);
            };

            input.OnMove = (pos, _) => HandleMove(input, pos);
            AddComponent(input);
            Size = _text.Size;
        }

        public override void Draw()
        {
            if (!EnableInput) return;
            if (_selected.HasValue)
            {
                var start = GetPositionAt(_selected.Value.Item1);
                var end = GetPositionAt(_selected.Value.Item2);
                DrawRect(new Vector2(start, 0), new Vector2(end - start, _text.LineHeight), Color.OrangeRed * 0.8f);
            }
            else
            {
                var pointerPos = GetPositionAt(_pointer);
                DrawLine(new Vector2(pointerPos, 0), new Vector2(pointerPos, _text.LineHeight), 2, Color.White);
            }
        }

        private float GetPositionAt(int index)
        {
            var pointerText = _text.Content.AsSpan().Slice(0, index);
            return _text.MeasureString(pointerText).X;
        }

        private void Update(float elapsed)
        {
            if (!EnableInput) return;
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left) && !prevState.IsKeyDown(Keys.Left) && _pointer > 0)
            {
                _selected = null;
                _pointer--;
            }
            if (state.IsKeyDown(Keys.Right) && !prevState.IsKeyDown(Keys.Right) && _pointer < _text.Content.Length)
            {
                _selected = null;
                _pointer++;
            }
            prevState = state;
        }

        private void HandleMove(MouseInput input, Vector2 pos)
        {
            if (input.ButtonHeld)
            {
                input.CaptureGlobal = true;
                var selectTo = _text.IndexAt(pos.X);
                _selected = (Math.Min(_pointer, selectTo), Math.Max(_pointer, selectTo));
            }
            else
            {
                input.CaptureGlobal = false;
            }
        }

        private void ClearSelected()
        {
            if (!_selected.HasValue) return;
            var (start, end) = _selected.Value;
            _text.Content = _text.Content.Remove(start, end - start);
            _pointer = start;
            _selected = null;
        }

        private void HandleInput(char character, Keys key)
        {
            if (!EnableInput) return;
            if (Char.IsControl(character))
            {
                if (key == Keys.Back)
                {
                    if (_selected.HasValue) ClearSelected();
                    else if (_pointer > 0)
                    {
                        _text.Content = _text.Content.Remove(_pointer - 1, 1);
                        _pointer--;
                    }
                }
            }
            else
            {
                ClearSelected();
                _text.Content = _text.Content.Insert(_pointer, character.ToString());
                _pointer++;
            }
            Size = new Vector2(Math.Min(Size.X, _text.Size.X), _text.Size.Y);
        }
    }
}