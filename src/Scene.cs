using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public class Scene
    {
        private List<IDrawable> _drawable = new List<IDrawable>();
        private List<IComponentSystem> _systems = new List<IComponentSystem>();
        protected Vector2 _camera;

        public Vector2 Camera => _camera;

        public Scene()
        {
            AddSystem(new MouseInputSystem());
        }

        public void AddSystem(IComponentSystem system)
        {
            _systems.Add(system);
        }

        public void AddNode(Node node)
        {
            if (node is IDrawable d) _drawable.Add(d);

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
            foreach (var system in _systems) system.UpdateComponents(this, elapsed);
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
