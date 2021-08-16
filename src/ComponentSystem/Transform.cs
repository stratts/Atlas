using Necs;
using System;

namespace Atlas
{
    public struct Transform
    {
        public Vector2 Position;
        public Vector2 LastPos;
        public Vector2 ScenePos;
        public Vector2 Size;
    }

    public class TransformSystem : IComponentSystem<UpdateContext>
    {
        public void Process(UpdateContext context, IEcsContext ecs)
        {
            ecs.Query<Transform>(Query);
        }

        private void Query(ref Transform c, ref Transform parent, bool hasParent)
        {
            c.LastPos = c.ScenePos;

            if (hasParent)
            {
                c.ScenePos = parent.ScenePos + c.Position;
            }
            else c.ScenePos = c.Position;
        }
    }
}