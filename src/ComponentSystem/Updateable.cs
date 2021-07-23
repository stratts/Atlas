using System;
using System.Collections.Generic;

namespace Atlas
{
    public interface ILogicComponent : IComponent
    {
        void Update(Scene scene, float elapsed);
    }

    public class Updateable : Component, ILogicComponent
    {
        public Action<Scene, float>? UpdateMethod { get; set; }

        public Updateable() { }

        public Updateable(Action<Scene, float> updateMethod) => UpdateMethod = updateMethod;

        void ILogicComponent.Update(Scene scene, float elapsed) => UpdateMethod?.Invoke(scene, elapsed);
    }

    public class UpdateSystem : BaseComponentSystem<ILogicComponent>
    {
        public override void UpdateComponents(Scene scene, IReadOnlyList<ILogicComponent> components, float elapsed)
        {
            foreach (var c in components)
            {
                if (c.Enabled) c.Update(scene, elapsed);
            }
        }
    }
}