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
    }

    public class MouseInputSystem : BaseComponentSystem<MouseInput>
    {
        private MouseState prevMouseState = Mouse.GetState();

        public override void UpdateComponents(Scene scene, IReadOnlyList<MouseInput> components, float elapsed)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                foreach (var c in components)
                {
                    var screenPos = c.Parent.GlobalPosition - scene.Camera;

                    var mX = mouseState.X;
                    var mY = mouseState.Y;

                    var left = screenPos.X + c.InputArea.X;
                    var up = screenPos.Y + c.InputArea.Y;
                    var right = left + c.InputArea.Width;
                    var down = up + c.InputArea.Height;

                    if (!(mX < left || mX > right || mY < up || mY > down))
                    {
                        // Clicked within input area
                        c.OnClick?.Invoke(Vector2.Zero);
                    }

                }
            }

            prevMouseState = mouseState;
        }
    }
}