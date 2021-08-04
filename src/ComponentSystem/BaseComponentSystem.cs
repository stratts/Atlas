using System;
using System.Collections.Generic;

namespace Atlas
{
    public abstract class BaseComponentSystem<T> : IComponentSystem, IComparer<T> where T : IComponent
    {
        protected List<T> _components = new List<T>();
        private Queue<Action> _actionQueue = new Queue<Action>();
        private HashSet<uint> _toSort = new();

        private bool _update = false;

        public event Action<T>? ComponentAdded;
        public event Action<T>? ComponentRemoved;

        protected bool UpdateEveryTick { get; set; } = true;
        protected bool ReverseSort { get; set; } = false;

        public bool HandlesComponent(IComponent component) => component is T;

        public void AddComponent(IComponent component)
        {
            T c = TryCast(component);

            _actionQueue.Enqueue(() =>
            {
                if (_components.Contains(c)) throw new ArgumentException($"Component already added to system ({component} of {component.Parent})");
                var idx = _components.FindIndex(t => Compare(t, c) >= 0);
                _components.Insert(idx != -1 ? idx : _components.Count, c);
                _update = true;
                ComponentAdded?.Invoke(c);
            });
        }

        public void RemoveComponent(IComponent component)
        {
            T c = TryCast(component);

            _actionQueue.Enqueue(() =>
            {
                if (!_components.Contains(c)) return;
                _components.Remove(c);
                _update = true;
                ComponentRemoved?.Invoke(c);
            });
        }

        private T TryCast(IComponent component)
        {
            if (component is T c) return c;

            throw new ArgumentException(
                $"System only handles components with type {typeof(T).Name}, not {component.GetType().Name}"
            );
        }

        private uint GetLayer(IComponent component) => (uint)(component.Priority / Scene.LayerSize);

        public void SortComponent(IComponent component) => _toSort.Add(GetLayer(component));

        public void SortComponents()
        {
            foreach (var layer in _toSort)
            {
                var start = _components.FindIndex(c => GetLayer(c) == layer);
                var end = _components.FindLastIndex(c => GetLayer(c) == layer);
                _components.Sort(start, end - start + 1, this);
            }

            _toSort.Clear();
        }

        public int Compare(T? a, T? b)
        {
            if (a == null || b == null) return 0;
            var res = b.Priority.CompareTo(a.Priority);
            if (res == 0) res = b.AltPriority.CompareTo(a.AltPriority);
            return ReverseSort ? -res : res;
        }

        public void UpdateComponents(Scene scene, float elapsed)
        {
            ProcessChanges();
            SortComponents();
            if (_update || UpdateEveryTick)
            {
                UpdateComponents(scene, _components, elapsed);
                _update = false;
            }
        }

        protected void ProcessChanges()
        {
            while (_actionQueue.TryDequeue(out var action)) action();
        }

        public abstract void UpdateComponents(Scene scene, IReadOnlyList<T> components, float elapsed);
    }
}