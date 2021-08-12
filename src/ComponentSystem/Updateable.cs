using System;
using System.Collections.Generic;
using Necs;

namespace Atlas
{
    public class Updateable
    {
        public Action<Scene, float>? UpdateMethod { get; set; }

        public Updateable() { }

        public Updateable(Action<Scene, float> updateMethod) => UpdateMethod = updateMethod;

        public void Update(Scene scene, float elapsed) => UpdateMethod?.Invoke(scene, elapsed);
    }

    public class UpdateSystem : IComponentSystem<UpdateContext, Updateable>
    {
        public void Process(UpdateContext context, ref Updateable component)
        {
            component.Update(context.Scene, context.ElapsedTime);
        }
    }
}