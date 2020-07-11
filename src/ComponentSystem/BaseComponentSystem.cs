using System;
using System.Collections.Generic;

namespace Industropolis.Engine
{
    public abstract class BaseComponentSystem<T> : IComponentSystem where T : Component
    {
        private List<T> _components = new List<T>();
        private Queue<Action> _actionQueue = new Queue<Action>();

        public bool HandlesComponent(Component component) => component is T;

        public void AddComponent(Component component)
        {
            T c = TryCast(component);
            if (_components.Contains(c)) throw new ArgumentException("Component already added to system");
            _actionQueue.Enqueue(() => _components.Insert(0, c));
        }

        public void RemoveComponent(Component component)
        {
            T c = TryCast(component);
            if (!_components.Contains(c)) throw new ArgumentException("Component not added to system");
            _actionQueue.Enqueue(() => _components.Remove(c));
        }

        private T TryCast(Component component)
        {
            if (component is T c) return c;

            throw new ArgumentException(
                $"System only handles components with type {typeof(T).Name}, not {component.GetType().Name}"
            );
        }

        public void UpdateComponents(Scene scene, float elapsed)
        {
            while (_actionQueue.TryDequeue(out var action)) action();
            UpdateComponents(scene, _components, elapsed);
        }

        public abstract void UpdateComponents(Scene scene, IReadOnlyList<T> components, float elapsed);
    }
}