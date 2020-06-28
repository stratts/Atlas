using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public class Scene
    {
        private List<IDrawable> _drawable = new List<IDrawable>();
        private List<IMouseInputHandler> _mHandlers = new List<IMouseInputHandler>();
        private List<IComponentSystem> _systems = new List<IComponentSystem>();
        protected Vector2 _camera;

        private MouseState prevMouseState = Mouse.GetState();

        public void AddSystem(IComponentSystem system)
        {
            _systems.Add(system);
        }

        public void AddNode(Node node)
        {
            if (node is IDrawable d) _drawable.Add(d);
            if (node is IMouseInputHandler m) _mHandlers.Add(m);

            foreach (var component in node.Components) AddComponent(component);
            node.ComponentAdded += AddComponent;

            foreach (var child in node.Children) AddNode(child);
        }

        private void AddComponent(Component component)
        {
            foreach (var system in _systems)
            {
                if (system.HandlesComponent(component)) system.AddComponent(component);
            }
        }

        public virtual void Update(float elapsed)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
            {
                foreach (var h in _mHandlers)
                {
                    var screenPos = h.GlobalPosition - _camera;

                    var mX = mouseState.X;
                    var mY = mouseState.Y;

                    var left = screenPos.X + h.InputArea.X;
                    var up = screenPos.Y + h.InputArea.Y;
                    var right = left + h.InputArea.Width;
                    var down = up + h.InputArea.Height;

                    if (!(mX < left || mX > right || mY < up || mY > down))
                    {
                        // Clicked within input area
                        h.OnClick(mouseState, Vector2.Zero);
                    }

                }
            }

            prevMouseState = mouseState;

            foreach (var system in _systems) system.UpdateComponents(elapsed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var d in _drawable)
            {
                var pos = d.GlobalPosition.Floor() - _camera.Floor();

                if (!(pos.Y + d.Size.Y < 0 || pos.X + d.Size.X < 0 || pos.X > 1280 || pos.Y > 720))
                    d.Draw(spriteBatch, pos);
            }
        }
    }
}
