using Necs;
using System;

namespace Atlas
{
    public struct Transform
    {
        public Vector2 Position;
        public Vector2 ScenePos;
        public Vector2 Size;
    }

    public class TransformSystem : IComponentParentSystem<UpdateContext, Transform>
    {
        public void Process(UpdateContext context, ref Transform c, ref Transform parent, bool hasParent)
        {
            if (hasParent)
            {
                c.ScenePos = parent.ScenePos + c.Position;
            }
            else c.ScenePos = c.Position;
        }
    }
}