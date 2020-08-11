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
        private List<IComponentSystem> _systems = new List<IComponentSystem>();
        protected Camera _camera = new Camera(Industropolis.UI.GameScreen.Width, Industropolis.UI.GameScreen.Height);

        private Dictionary<int, uint> _topLevelCount = new Dictionary<int, uint>();

        public Camera Camera => _camera;
        public IReadOnlyList<Node> Nodes => _nodes;

        public Scene()
        {
            AddNode(_camera);
            AddSystem(new DrawableSystem());
            AddSystem(new ModulateSystem());
            AddSystem(new UpdateSystem());
            AddSystem(new MouseInputSystem());
            AddSystem(new LayoutSystem());
            AddSystem(new ScissorSystem());
        }

        public void AddSystem(IComponentSystem system)
        {
            _systems.Add(system);
        }

        public void AddNode(Node node) => AddNode(node, null);

        public void AddNode(Node node, int? layer)
        {
            if (layer.HasValue) node.Layer = layer.Value;

            _nodes.Add(node);
            SetNodeSort(node);

            foreach (var component in node.Components) AddComponent(component);

            node.ChildAdded += AddNode;
            node.ChildRemoved += RemoveNode;
            node.ComponentAdded += AddComponent;
            node.ComponentRemoved += RemoveComponent;
            node.Deleted += RemoveNode;
            node.BroughtToFront += BringNodeToFront;

            foreach (var child in node.Children) AddNode(child);
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

            foreach (var component in node.Components) RemoveComponent(component);
            foreach (var child in node.Children) RemoveNode(child);
        }

        public IEnumerable<T> GetNodes<T>() where T : Node
        {
            foreach (var node in Nodes)
            {
                if (node is T n) yield return n;
            }
        }

        private void BringNodeToFront(Node node) => SetNodeSort(node, true);

        private void SetNodeSort(Node node, bool recurse = false)
        {
            if (node.Parent == null)
            {
                var layer = node.Layer;
                if (!_topLevelCount.ContainsKey(layer)) _topLevelCount[layer] = 0;
                node.SceneSort = ushort.MaxValue * layer + _topLevelCount[layer];
                _topLevelCount[layer]++;
            }
            else
            {
                int index;
                if (node.Layer != -1) index = 20 + 20 * node.Layer + node.Parent.Children.IndexOf(node);
                else index = node.Parent.Children.IndexOf(node) + 1;
                node.SceneSort = node.Parent.SceneSort + (double)index / Math.Pow(10, node.Depth);
            }

            foreach (var component in node.Components)
            {
                component.Priority = node.SceneSort;
                GetSystem(component)?.SortComponents();
            }

            if (recurse)
            {
                foreach (var child in node.Children) SetNodeSort(child, true);
            }
        }

        private IComponentSystem? GetSystem(Component component)
        {
            foreach (var system in _systems)
            {
                if (system.HandlesComponent(component)) return system;
            }
            return null;
        }

        private void AddComponent(Component component)
        {
            component.Priority = component.Parent.SceneSort;
            GetSystem(component)?.AddComponent(component);
        }

        private void RemoveComponent(Component component)
        {
            GetSystem(component)?.RemoveComponent(component);
        }

        public virtual void Update(float elapsed)
        {
            foreach (var system in _systems) system.UpdateComponents(this, elapsed);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var system in _systems)
            {
                if (system is IRenderSystem r) r.Draw(this, spriteBatch);
            }
        }

        public Vector2 ScreenToScene(Vector2 screenPos)
        {
            return (screenPos / Camera.Zoom) + Camera.Viewport.Location.ToVector2();
        }
    }
}
