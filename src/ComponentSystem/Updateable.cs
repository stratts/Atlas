using System;
using System.Collections.Generic;

namespace Industropolis.Engine
{
    public class Updateable : Component
    {
        public Action<float>? UpdateMethod { get; set; }
    }

    public class UpdateSystem : BaseComponentSystem<Updateable>
    {
        public override void UpdateComponents(Scene scene, IReadOnlyList<Updateable> components, float elapsed)
        {
            foreach (var c in components) c.UpdateMethod?.Invoke(elapsed);
        }
    }
}