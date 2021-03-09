using System;
using System.Collections.Generic;

namespace Atlas
{
    public abstract class BaseComponentSystem<T> : IComponentSystem where T : IComponent
    {
        protected List<T> _components = new List<T>();
        private Queue<Action> _actionQueue = new Queue<Action>();
        private bool _sort = false;
        private bool _update = false;

        public event Action<T>? ComponentAdded;
        public event Action<T>? ComponentRemoved;

        protected bool UpdateEveryTick { get; set; } = true;

        public bool HandlesComponent(IComponent component) => component is T;

        public void AddComponent(IComponent component)
        {
            T c = TryCast(component);

            _actionQueue.Enqueue(() =>
            {
                if (_components.Contains(c)) throw new ArgumentException("Component already added to system");
                _components.Add(c);
                _sort = true;
                _update = true;
                ComponentAdded?.Invoke(c);
            });
        }

        public void RemoveComponent(IComponent component)
        {
            T c = TryCast(component);

            _actionQueue.Enqueue(() =>
            {
                if (!_components.Contains(c)) throw new ArgumentException($"Component {component} not added to system");
                _components.Remove(c);
                _sort = true;
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

        public void SortComponents() => _sort = true;

        protected virtual int SortMethod(T a, T b) => b.Priority.CompareTo(a.Priority);

        public void UpdateComponents(Scene scene, float elapsed)
        {
            ProcessChanges();
            if (_sort)
            {
                _components.Sort(SortMethod);
                _sort = false;
            }
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