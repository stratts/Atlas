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
        private List<IUpdateable> _updateable = new List<IUpdateable>();
        private List<IComponentSystem> _systems = new List<IComponentSystem>();
        protected Camera _camera = new Camera(GameScreen.Width, GameScreen.Height);

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
            if (node is IUpdateable u) _updateable.Add(u);

            foreach (var component in node.Components) AddComponent(component);
            node.ComponentAdded += AddComponent;
            node.ComponentRemoved += RemoveComponent;
            node.Deleted += RemoveNode;

            foreach (var child in node.Children) AddNode(child);
        }

        public void RemoveNode(Node node)
        {
            if (node is IDrawable d) _drawable.Remove(d);
            if (node is IUpdateable u) _updateable.Remove(u);

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
            foreach (var u in _updateable)
            {
                if (u.Enabled) u.Update(elapsed);
            }
            foreach (var system in _systems) system.UpdateComponents(this, elapsed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var scale = Matrix.CreateScale(Camera.Zoom);
            var translation = Matrix.CreateTranslation(-Camera.Position.X, -Camera.Position.Y, 0);
            var offset = Matrix.CreateTranslation(Camera.Size.X / 2, Camera.Size.Y / 2, 0);

            spriteBatch.Begin(transformMatrix: translation * scale * offset);
            var viewport = Camera.Viewport;

            foreach (var d in _drawable)
            {
                if (!d.Enabled) continue;
                var pos = d.ScenePosition.Floor();
                var bounds = d.DrawBounds;

                if (!(pos.X + bounds.Right < viewport.Left || pos.Y + bounds.Bottom < viewport.Top
                    || pos.X + bounds.Left > viewport.Right || pos.Y + bounds.Top > viewport.Bottom))
                    d.Draw(spriteBatch, pos - new Vector2(offset.Translation.X, offset.Translation.Y));
            }

            spriteBatch.End();
        }

        public Vector2 ScreenToScene(Vector2 screenPos)
        {
            return (screenPos / Camera.Zoom) + Camera.Viewport.Location.ToVector2();
        }
    }
}
