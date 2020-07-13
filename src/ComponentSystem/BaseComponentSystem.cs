using System;
using System.Collections.Generic;

namespace Industropolis.Engine
{
    public abstract class BaseComponentSystem<T> : IComponentSystem where T : Component
    {
        protected List<T> _components = new List<T>();
        private Queue<Action> _actionQueue = new Queue<Action>();
        private bool _sort = false;

        public bool HandlesComponent(Component component) => component is T;

        public void AddComponent(Component component)
        {
            T c = TryCast(component);

            _actionQueue.Enqueue(() =>
            {
                if (_components.Contains(c)) throw new ArgumentException("Component already added to system");
                _components.Add(c);
                _sort = true;
            });
        }

        public void RemoveComponent(Component component)
        {
            T c = TryCast(component);

            _actionQueue.Enqueue(() =>
            {
                if (!_components.Contains(c)) throw new ArgumentException("Component not added to system");
                _components.Remove(c);
                _sort = true;
            });
        }

        private T TryCast(Component component)
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
            UpdateComponents(scene, _components, elapsed);
        }

        protected void ProcessChanges()
        {
            while (_actionQueue.TryDequeue(out var action)) action();
        }

        public abstract void UpdateComponents(Scene scene, IReadOnlyList<T> components, float elapsed);
    }
}