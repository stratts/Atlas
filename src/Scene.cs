using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Atlas
{
    public interface IScene
    {
        void Update(float elapsed);
        void Draw(SpriteBatch spriteBatch);
        bool EnableUpdate { get; }
        bool EnableDraw { get; }
    }

    /// <summary>
    /// A scene that consists of multiple scenes arranged into layers
    /// </summary>
    public class CompoundScene : IScene
    {
        private IScene[] _scenes;

        public bool EnableUpdate { get; set; } = true;
        public bool EnableDraw { get; set; } = true;

        private Queue<int> test = new Queue<int>();

        public CompoundScene(int layers)
        {
            _scenes = new IScene[layers];
            var t = new List<int>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = _scenes.Length - 1; i >= 0; i--)
            {
                var s = _scenes[i];
                if (!s.EnableDraw) continue;
                s?.Draw(spriteBatch);
            }
        }

        public virtual void Update(float elapsed)
        {
            foreach (var scene in _scenes)
            {
                if (!scene.EnableUpdate) continue;
                scene?.Update(elapsed);
            }
        }

        public IScene GetScene(int layer) => _scenes[layer];

        public void SetScene(int layer, IScene scene) => _scenes[layer] = scene;
    }

    /// <summary>
    /// Base scene class, manages nodes, systems, and components
    /// </summary>
    public class Scene : IScene
    {
        private List<Node> _nodes = new List<Node>();
        private List<IComponentSystem> _systems = new List<IComponentSystem>();
        protected Camera _camera = new Camera(Config.ScreenSize.X, Config.ScreenSize.Y);

        private Dictionary<uint, uint> _topLevelCount = new Dictionary<uint, uint>();
        private Dictionary<uint, bool> _depthSort = new Dictionary<uint, bool>();

        public Camera Camera => _camera;
        public IReadOnlyList<Node> Nodes => _nodes;

        public bool EnableUpdate { get; set; } = true;
        public bool EnableDraw { get; set; } = true;

        public bool NearestNeighbour { get; set; } = false;

        public Scene()
        {
            AddNode(_camera);
            AddSystem(new MouseInputSystem());

            AddSystem(new AnimationSystem());
            AddSystem(new UpdateSystem());
            AddSystem(new CollisionSystem());

            AddSystem(new LayoutSystem());
            AddSystem(new ModulateSystem());
            AddSystem(new ScissorSystem());
            AddSystem(new DrawableSystem());
        }

        public void AddSystem(IComponentSystem system)
        {
            _systems.Add(system);
        }

        public void AddNode(Node node) => AddNode(node, null);

        public void AddNode(Node node, uint? layer)
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

        /// <summary>
        /// Gets nodes within the scene that match the given type
        /// </summary>
        public IEnumerable<T> GetNodes<T>() where T : Node
        {
            foreach (var node in Nodes)
            {
                if (node is T n) yield return n;
            }
        }

        /// <summary>
        /// Gets nodes within the scene located at the given position
        /// </summary>
        public IEnumerable<Node> GetNodesAt(Vector2 position)
        {
            foreach (var node in Nodes)
            {
                if (new Rectangle(node.ScenePosition.ToPoint(), node.Size.ToPoint()).Contains(position))
                    yield return node;
            }
        }

        public void SetDepthSort(uint layer, bool enableDepthSort) => _depthSort[layer + 1] = enableDepthSort;

        private void BringNodeToFront(Node node) => SetNodeSort(node, true);

        private void SetNodeSort(Node node, bool recurse = false)
        {
            const ulong maxNodes = uint.MaxValue;
            const ulong maxChildren = ushort.MaxValue;
            const ulong layerSize = maxNodes * maxChildren;

            if (node.Parent == null)
            {
                uint layer = node.Layer.HasValue ? node.Layer.Value + 1 : 0;
                if (!_topLevelCount.ContainsKey(layer)) _topLevelCount[layer] = 0;
                if (!_depthSort.ContainsKey(layer)) _depthSort[layer] = false;
                uint sortKey = _depthSort[layer] ? (uint)((long)int.MaxValue + (long)node.ScenePosition.Y) : _topLevelCount[layer];
                node.SceneSort = (ulong)layer * layerSize + sortKey * maxChildren;
                _topLevelCount[layer]++;
            }
            else if (node.RootNode != null) SetChildSort(node.RootNode, node.RootNode.SceneSort);

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

        private ulong SetChildSort(Node node, ulong current)
        {
            var children = new List<Node>(node.Children);
            children.Sort(
                   (a, b) =>
                   {
                       int layerA = a.Layer.HasValue ? (int)a.Layer.Value : -1;
                       int layerB = b.Layer.HasValue ? (int)b.Layer.Value : -1;
                       return layerA.CompareTo(layerB);
                   });

            node.SceneSort = current;
            foreach (var component in node.Components) component.Priority = node.SceneSort;

            foreach (var child in children) current = SetChildSort(child, current + 1);

            return current;
        }

        private IComponentSystem? GetSystem(IComponent component)
        {
            foreach (var system in _systems)
            {
                if (system.HandlesComponent(component)) return system;
            }
            return null;
        }

        private void AddComponent(IComponent component)
        {
            component.Priority = component.Parent.SceneSort;
            GetSystem(component)?.AddComponent(component);
        }

        private void RemoveComponent(IComponent component)
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

        /// <summary>
        /// Translates an onscreen position to a position relative to the scene
        /// </summary>
        public Vector2 ScreenToScene(Vector2 screenPos)
        {
            return (screenPos / Camera.Zoom) + Camera.Viewport.Location.ToVector2();
        }

        /// <summary>
        /// Translates a position relative to the scene to an onscreen position
        /// </summary>
        public Vector2 SceneToScreen(Vector2 scenePos)
        {
            return (scenePos - Camera.Viewport.Location.ToVector2()) * Camera.Zoom;
        }
    }
}
