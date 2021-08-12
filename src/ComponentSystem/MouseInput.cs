using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Necs;

namespace Atlas
{
    public class MouseInput
    {
        public bool ConsumeInput { get; set; } = true;
        public bool HandleConsumed { get; set; } = false;
        public bool CaptureGlobal { get; set; } = false;
        public Rectangle InputArea { get; set; } = Rectangle.Empty;
        public bool ButtonHeld { get; set; } = false;
        public Action<Vector2>? OnClick { get; set; }
        public Action<Vector2, Vector2>? OnMove { get; set; }
        public Action<Vector2, int>? OnScroll { get; set; }
        public Action? OnMouseEnter { get; set; }
        public Action? OnMouseExit { get; set; }
        public Action? OnFocusEnter { get; set; }
        public Action? OnFocusExit { get; set; }
    }

    public class MouseInputSystem : IComponentSystem<UpdateContext, Transform, MouseInput>
    {
        private MouseState _mouseState;
        private MouseState _prevMouseState = Mouse.GetState();
        private Vector2 _mousePos;
        private Vector2 _prevMousePos;
        private HashSet<MouseInput> _mouseEntered = new HashSet<MouseInput>();
        private static MouseInput? _consumedBy;
        private static MouseInput? _focused;

        public static bool InputConsumed { get; set; } = false;

        public void Process(UpdateContext context, ref Transform t, ref MouseInput c)
        {
            if ((!InputConsumed || c.HandleConsumed || _consumedBy == c) && WithinInputArea(context.Scene, _mousePos, t, c))
            {
                // Handle mouse enter
                if (!_mouseEntered.Contains(c))
                {
                    _mouseEntered.Add(c);
                    c.OnMouseEnter?.Invoke();
                }

                if (_mouseState.LeftButton == ButtonState.Pressed) c.ButtonHeld = true;
                else c.ButtonHeld = false;

                // Handle click
                if (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
                {
                    c.OnClick?.Invoke(MouseToAreaPos(context.Scene, _mousePos, t, c));

                    if (_focused != c)
                    {
                        c.OnFocusEnter?.Invoke();
                        _focused?.OnFocusExit?.Invoke();
                        _focused = c;
                    }
                }

                // Handle scroll
                var scroll = _mouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue;
                if (scroll != 0) c.OnScroll?.Invoke(_mousePos, scroll);

                // Handle movement
                if (_mousePos != _prevMousePos)
                {
                    var curAreaPos = MouseToAreaPos(context.Scene, _mousePos, t, c);
                    var prevAreaPos = MouseToAreaPos(context.Scene, _prevMousePos, t, c);
                    c.OnMove?.Invoke(curAreaPos, curAreaPos - prevAreaPos);
                }

                // Mark input as consumed
                if (c.ConsumeInput)
                {
                    _consumedBy = c;
                    InputConsumed = true;
                }
            }
            else if (_mouseEntered.Contains(c))
            {
                c.OnMouseExit?.Invoke();
                _mouseEntered.Remove(c);
            }
        }

        public static Vector2 MouseToScenePos(Scene scene, MouseState state) =>
            scene.ScreenToScene(state.Position.ToAtlasVector2());

        private Vector2 MouseToAreaPos(Scene scene, Vector2 mousePos, Transform t, MouseInput component)
        {
            var nodePos = t.ScenePos;
            var areaPos = new Vector2(component.InputArea.X, component.InputArea.Y);
            return mousePos - nodePos - areaPos;
        }

        private bool WithinInputArea(Scene scene, Vector2 mousePos, Transform t, MouseInput component)
        {
            var area = component.InputArea != Rectangle.Empty ? component.InputArea : t.Size.ToRectangle();
            if (component.CaptureGlobal) return true;
            var areaPos = MouseToAreaPos(scene, mousePos, t, component);
            return (!(areaPos.X < 0 || areaPos.X > area.Width || areaPos.Y < 0 || areaPos.Y > area.Height));
        }


        public void BeforeProcess(UpdateContext context)
        {
            _mouseState = Mouse.GetState();
            _mousePos = MouseToScenePos(context.Scene, _mouseState);
        }

        public void AfterProcess(UpdateContext context)
        {
            _prevMouseState = _mouseState;
            _prevMousePos = _mousePos;
        }
    }
}