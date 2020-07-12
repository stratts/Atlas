using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Industropolis.Engine
{
    public interface IScene
    {
        void Update(float elapsed);
        void Draw(SpriteBatch spriteBatch);
    }

    public class CompoundScene : IScene
    {
        private Scene[] _scenes;

        public CompoundScene(int layers)
        {
            _scenes = new Scene[layers];
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = _scenes.Length - 1; i >= 0; i--) _scenes[i]?.Draw(spriteBatch);
        }

        public virtual void Update(float elapsed)
        {
            foreach (var scene in _scenes) scene?.Update(elapsed);
        }

        public Scene GetScene(int layer) => _scenes[layer];

        public void SetScene(int layer, Scene scene) => _scenes[layer] = scene;
    }

    public class Scene : IScene
    {
        private List<Node> _nodes = new List<Node>();
        private List<IDrawable>[] _drawable;
        private List<IUpdateable> _updateable = new List<IUpdateable>();
        private List<IComponentSystem> _systems = new List<IComponentSystem>();
        protected Camera _camera = new Camera(UI.GameScreen.Width, UI.GameScreen.Height);

        private int _topLevelCount = 0;
        private bool[] _resortLayer;

        public Camera Camera => _camera;
        public IReadOnlyList<Node> Nodes => _nodes;

        public Scene(int layers)
        {
            if (layers <= 0) layers = 1;
            _drawable = new List<IDrawable>[layers];
            _resortLayer = new bool[layers];
            for (int i = 0; i < layers; i++) _drawable[i] = new List<IDrawable>();
            AddNode(_camera);
            AddSystem(new MouseInputSystem());
        }

        public void AddSystem(IComponentSystem system)
        {
            _systems.Add(system);
        }

        public void AddNode(Node node) => AddNode(node, 0);

        public void AddNode(Node node, int layer)
        {
            _nodes.Add(node);
            _resortLayer[layer] = true;

            if (node is IDrawable d) _drawable[layer].Add(d);
            if (node is IUpdateable u) _updateable.Add(u);

            if (node.Parent == null)
            {
                node.SceneSort = _topLevelCount;
                _topLevelCount++;
            }
            else node.SceneSort = node.Parent.SceneSort + (float)node.Sort / (float)Math.Pow(10, node.Depth);

            foreach (var component in node.Components) AddComponent(component);

            node.ChildAdded += AddNode;
            node.ChildRemoved += RemoveNode;
            node.ComponentAdded += AddComponent;
            node.ComponentRemoved += RemoveComponent;
            node.Deleted += RemoveNode;
            node.BroughtToFront += BringNodeToFront;

            foreach (var child in node.Children) AddNode(child, layer);
        }

        public void RemoveNode(Node node)
        {
            _nodes.Remove(node);

            node.ChildAdded -= AddNode;
            node.ChildRemoved -= RemoveNode;
            node.ComponentAdded -= AddComponent;
            node.ComponentRemoved -= RemoveComponent;
            node.Deleted -= RemoveNode;
            node.BroughtToFront -= BringNodeToFront;

            if (node is IDrawable d)
            {
                int l = 0;
                foreach (var layer in _drawable)
                {
                    if (layer.Contains(d))
                    {
                        _resortLayer[l] = true;
                        layer.Remove(d);
                    }
                    l++;
                }
            }
            if (node is IUpdateable u) _updateable.Remove(u);

            foreach (var component in node.Components) RemoveComponent(component);
            foreach (var child in node.Children) RemoveNode(child);
        }

        private void BringNodeToFront(Node node)
        {
            RemoveNode(node);
            AddNode(node);
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
            for (int i = 0; i < _drawable.Length; i++)
            {
                if (_resortLayer[i])
                {
                    var layer = _drawable[i];
                    layer.Sort((a, b) => a.SceneSort.CompareTo(b.SceneSort));
                    _resortLayer[i] = false;
                }
            }

            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(Camera.Zoom));

            foreach (var layer in _drawable)
            {
                foreach (var d in layer)
                {
                    if (!d.Enabled) continue;
                    var pos = d.ScenePosition.Floor();
                    var bounds = d.DrawBounds;
                    bounds.Location += pos.ToPoint();

                    if (bounds.Intersects(Camera.Viewport) || bounds == Rectangle.Empty)
                    {
                        d.Draw(spriteBatch, pos - _camera.Position.Floor());
                    }
                }
            }

            spriteBatch.End();
        }

        public Vector2 ScreenToScene(Vector2 screenPos)
        {
            return (screenPos / Camera.Zoom) + Camera.Viewport.Location.ToVector2();
        }
    }
}
