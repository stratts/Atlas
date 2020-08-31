using System;
using System.Collections.Generic;

namespace Industropolis.Engine
{
    public interface ILogicComponent : IComponent
    {
        void Update(Scene scene, float elapsed);
    }

    public class Updateable : Component, ILogicComponent
    {
        public Action<Scene, float>? UpdateMethod { get; set; }

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