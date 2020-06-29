using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Industropolis
{
    public class MouseInput : Component
    {
        public Rectangle InputArea { get; set; }
        public Action<Vector2>? OnClick { get; set; }
        public Action<Vector2>? OnMove { get; set; }
        public Action? OnMouseEnter { get; set; }
        public Action? OnMouseExit { get; set; }
    }

    public class MouseInputSystem : BaseComponentSystem<MouseInput>
    {
        private MouseState _mouseState;
        private MouseState _prevMouseState = Mouse.GetState();

        public override void UpdateComponents(Scene scene, IReadOnlyList<MouseInput> components, float elapsed)
        {
            _mouseState = Mouse.GetState();

            if (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
            {
                foreach (var c in components)
                {
                    if (c.OnClick == null) continue;
                    if (WithinInputArea(scene, _mouseState, c))
                    {
                        c.OnClick.Invoke(MouseToAreaPos(scene, _mouseState, c));
                    }
                }
            }

            if (_mouseState.X != _prevMouseState.X || _mouseState.Y != _prevMouseState.Y)
            {
                foreach (var c in components)
                {
                    if (c.OnMove == null && c.OnMouseEnter == null && c.OnMouseExit == null) continue;

                    bool withinCurrent = WithinInputArea(scene, _mouseState, c);
                    bool withinPrev = WithinInputArea(scene, _prevMouseState, c);
                    if (withinCurrent)
                    {
                        c.OnMove?.Invoke(MouseToAreaPos(scene, _mouseState, c));
                        if (!withinPrev) c.OnMouseEnter?.Invoke();
                    }
                    else if (withinPrev) c.OnMouseExit?.Invoke();
                }
            }

            _prevMouseState = _mouseState;
        }

        private Vector2 MouseToAreaPos(Scene scene, MouseState state, MouseInput component)
        {
            var mousePos = new Vector2(state.X + scene.Camera.X, state.Y + scene.Camera.Y);
            var nodePos = component.Parent.GlobalPosition;
            var areaPos = new Vector2(component.InputArea.X, component.InputArea.Y);
            return mousePos - nodePos - areaPos;
        }

        private bool WithinInputArea(Scene scene, MouseState state, MouseInput component)
        {
            return component.InputArea.Contains(MouseToAreaPos(scene, state, component));
        }
    }
}