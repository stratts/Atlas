using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Industropolis
{
    public class MouseInput : Component
    {
        public bool ConsumeInput { get; set; } = true;
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
                if (!InputConsumed && WithinInputArea(scene, _mousePos, c))
                {
                    // Handle click
                    if (_mouseState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
                    {
                        c.OnClick?.Invoke(MouseToAreaPos(scene, _mousePos, c));
                    }

                    // Handle movement
                    if (_mousePos != _prevMousePos)
                    {
                        c.OnMove?.Invoke(MouseToAreaPos(scene, _mousePos, c));

                        if (!_mouseEntered.Contains(c))
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

        private Vector2 MouseToScenePos(Scene scene, MouseState state)
        {
            return new Vector2(state.X + scene.Camera.Position.X, state.Y + scene.Camera.Position.Y);
        }

        private Vector2 MouseToAreaPos(Scene scene, Vector2 mousePos, MouseInput component)
        {
            var nodePos = component.Parent.ScenePosition;
            var areaPos = new Vector2(component.InputArea.X, component.InputArea.Y);
            return mousePos - nodePos - areaPos;
        }

        private bool WithinInputArea(Scene scene, Vector2 mousePos, MouseInput component)
        {
            return component.InputArea.Contains(MouseToAreaPos(scene, mousePos, component));
        }
    }
}