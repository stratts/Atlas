using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Industropolis.Engine
{
    public class MouseInput : Component
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
    }

    public class MouseInputSystem : BaseComponentSystem<MouseInput>
    {
        private MouseState _mouseState;
        private MouseState _prevMouseState = Mouse.GetState();
        private Vector2 _mousePos;
        private Vector2 _prevMousePos;
        private HashSet<MouseInput> _mouseEntered = new HashSet<MouseInput>();

        public static bool InputConsumed { get; set; } = false;

        public override void UpdateComponents(Scene scene, IReadOnlyList<MouseInput> components, float elapsed)
        {
            _mouseState = Mouse.GetState();
            _mousePos = MouseToScenePos(scene, _mouseState);

            foreach (var c in components)
            {
                if (c.Enabled && (!InputConsumed || c.HandleConsumed) && WithinInputArea(scene, _mousePos, c))
                {
                    if (_mouseState.LeftButton == ButtonState.Pressed) c.ButtonHeld = true;
                    else c.ButtonHeld = false;
                    // Handle click
                    if (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
                    {
                        c.OnClick?.Invoke(MouseToAreaPos(scene, _mousePos, c));
                    }

                    // Handle scroll
                    var scroll = _mouseState.ScrollWheelValue - _prevMouseState.ScrollWheelValue;
                    if (scroll != 0) c.OnScroll?.Invoke(_mousePos, scroll);

                    // Handle movement
                    if (_mousePos != _prevMousePos)
                    {
                        var curAreaPos = MouseToAreaPos(scene, _mousePos, c);
                        var prevAreaPos = MouseToAreaPos(scene, _prevMousePos, c);
                        c.OnMove?.Invoke(curAreaPos, curAreaPos - prevAreaPos);

                        if (c.InputArea != Rectangle.Empty && !_mouseEntered.Contains(c))
                        {
                            _mouseEntered.Add(c);
                            c.OnMouseEnter?.Invoke();
                        }
                    }

                    // Mark input as consumed
                    if (c.ConsumeInput) InputConsumed = true;
                }
                else if (_mouseEntered.Contains(c))
                {
                    c.OnMouseExit?.Invoke();
                    _mouseEntered.Remove(c);
                }
            }

            _prevMouseState = _mouseState;
            _prevMousePos = _mousePos;
        }

        public static Vector2 MouseToScenePos(Scene scene, MouseState state) => scene.ScreenToScene(state.Position.ToVector2());

        private Vector2 MouseToAreaPos(Scene scene, Vector2 mousePos, MouseInput component)
        {
            var nodePos = component.Parent.ScenePosition;
            var areaPos = new Vector2(component.InputArea.X, component.InputArea.Y);
            return mousePos - nodePos - areaPos;
        }

        private bool WithinInputArea(Scene scene, Vector2 mousePos, MouseInput component)
        {
            var area = component.InputArea != Rectangle.Empty ? component.InputArea : component.Parent.Size.ToRectangle();
            if (component.CaptureGlobal) return true;
            var areaPos = MouseToAreaPos(scene, mousePos, component);
            return (!(areaPos.X < 0 || areaPos.X > area.Width || areaPos.Y < 0 || areaPos.Y > area.Height));
        }
    }
}