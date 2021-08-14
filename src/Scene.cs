using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Necs;

namespace Atlas
{
    public class SceneContext
    {
        public Scene Scene { get; }
        public float Elapsed { get; }

        public SceneContext(Scene scene, float elapsed) => (Scene, Elapsed) = (scene, elapsed);
    }

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
        protected EcsContext<UpdateContext, RenderContext> _ecs = new();
        private UpdateContext _updateContext;
        protected Camera _camera = new Camera(Config.ScreenSize.X, Config.ScreenSize.Y);

        private const ulong _maxNodes = uint.MaxValue;
        private const ulong _maxChildren = ushort.MaxValue;
        public const ulong LayerSize = _maxNodes * _maxChildren;

        private Dictionary<uint, uint> _topLevelCount = new Dictionary<uint, uint>();
        private Dictionary<uint, bool> _depthSort = new Dictionary<uint, bool>();

        public Camera Camera => _camera;

        public IUpdateContext UpdateContext => _updateContext;

        public bool EnableUpdate { get; set; } = true;
        public bool EnableDraw { get; set; } = true;

        public bool NearestNeighbour { get; set; } = false;

        public Scene()
        {
            _updateContext = new UpdateContext(this);
            AddNode(_camera);

            _ecs.AddSystem(new LayoutSystem());
            _ecs.AddSystem(new TransformSystem());
            _ecs.AddSystem(new MouseInputSystem());

            _ecs.AddSystem(new AnimationSystem());
            _ecs.AddSystem(new UpdateSystem());
            //AddSystem(new CollisionSystem());

            _ecs.AddSystem(new ModulateSystem());
            _ecs.AddSystem(new DrawableModulateSystem());
            //AddSystem(new ScissorSystem());
            _ecs.AddRenderSystem(new DrawableSystem());
        }

        public void AddNode(Node node) => AddNode(node, null);

        public void AddNode(Node node, uint? layer)
        {
            if (layer.HasValue) node.Layer = layer.Value;

            if (node.Layer != null) _ecs.SetTreePriority(node.Id, (ulong)node.Layer.Value);
            _ecs.AddEntity(node);
            node.Deleted += RemoveNode;
        }

        public void RemoveNode(Node node)
        {
            node.Deleted -= RemoveNode;
            _ecs.RemoveEntity(node);
        }

        /// <summary>
        /// Gets nodes within the scene that match the given type
        /// </summary>
        public IEnumerable<T> GetNodes<T>() where T : Node
        {
            /*foreach (var node in Nodes)
            {
                if (node is T n) yield return n;
            }*/
            yield break;
        }

        /// <summary>
        /// Gets nodes within the scene located at the given position
        /// </summary>
        public IEnumerable<Node> GetNodesAt(Vector2 position)
        {
            yield break;
            /*foreach (var node in Nodes)
            {
                if (node == Camera) continue;
                var bounds = node.Bounds;
                bounds.Offset(node.ScenePosition);
                if (bounds.Contains(position)) yield return node;
            }*/
        }

        public T? GetNodeAt<T>(Vector2 position) where T : Node
        {
            foreach (var node in GetNodesAt(position))
            {
                if (node is T n) return n;
            }
            return null;
        }

        public bool IsDepthSort(uint layer) => _depthSort.GetValueOrDefault(layer + 1);

        public void SetDepthSort(uint layer, bool enableDepthSort) => _depthSort[layer + 1] = enableDepthSort;

        private void BringNodeToFront(Node node) => UpdateNodeSort(node, true);

        private void UpdateNodeSort(Node node, bool recurse = false, bool renderOnly = false)
        {

        }


        public virtual void Update(float elapsed)
        {
            _updateContext.ElapsedTime = elapsed;
            _ecs.Update(_updateContext);

            /*foreach (var node in _nodes)
            {
                if (node.LastPos != node.Position && (node.Parent == null || node.PlaceInScene) && node.SceneLayer != null && IsDepthSort(node.SceneLayer.Value))
                {
                    UpdateNodeSort(node, true, renderOnly: true);
                    node.LastPos = node.Position;
                }
            }
            foreach (var system in _systems) system.UpdateComponents(this, elapsed);*/
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _ecs.Render(new RenderContext(this, spriteBatch));
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
