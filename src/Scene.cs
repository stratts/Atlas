using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public interface IScene
    {
        void Update(float elapsed);
        void Draw(SpriteBatch spriteBatch);
    }

    public class LayeredScene : IScene
    {
        private Scene[] _scenes;

        public LayeredScene(int layers)
        {
            _scenes = new Scene[layers];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = _scenes.Length - 1; i >= 0; i--) _scenes[i]?.Draw(spriteBatch);
        }

        public void Update(float elapsed)
        {
            foreach (var scene in _scenes) scene?.Update(elapsed);
        }

        public Scene GetScene(int layer) => _scenes[layer];

        public void SetScene(int layer, Scene scene) => _scenes[layer] = scene;
    }

    public class Scene : IScene
    {
        private List<IDrawable> _drawable = new List<IDrawable>();
        private List<IComponentSystem> _systems = new List<IComponentSystem>();
        protected Camera _camera = new Camera(1280, 720);

        public Camera Camera => _camera;

        public Scene()
        {
            AddNode(_camera);
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
            node.ComponentRemoved += RemoveComponent;
            node.Deleted += RemoveNode;

            foreach (var child in node.Children) AddNode(child);
        }

        public void RemoveNode(Node node)
        {
            if (node is IDrawable d) _drawable.Remove(d);
            foreach (var component in node.Components) RemoveComponent(component);
            foreach (var child in node.Children) RemoveNode(child);
        }

        private void AddComponent(Component component)
        {
            foreach (var system in _systems)
            {
                if (system.HandlesComponent(component)) system.AddComponent(component);
            }
        }

        private void RemoveComponent(Component component)
        {
            foreach (var system in _systems)
            {
                if (system.HandlesComponent(component)) system.RemoveComponent(component);
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
                var pos = d.ScenePosition.Floor() - _camera.Position.Floor();

                if (!(pos.Y + d.Size.Y < 0 || pos.X + d.Size.X < 0 || pos.X > 1280 || pos.Y > 720))
                    d.Draw(spriteBatch, pos);
            }
        }
    }
}
