using Necs;
using System;
using System.Diagnostics;

namespace Atlas
{
    public struct Transform
    {
        public Vector2 Position;
        public Vector2 LastPos;
        public Vector2 ScenePos;
        public Vector2 Size;
        public Rectangle Bounds;
        internal bool UpdateBounds;
        internal bool UpdatedInitial;
    }

    public class TransformSystem : IComponentSystem<UpdateContext>
    {
        Stopwatch stopWatch = new Stopwatch();

        public void Process(UpdateContext context, EcsContext ecs)
        {
            ecs.Query<Transform>((ref Transform c, ref Transform parent, bool hasParent) =>
            {
                c.LastPos = c.ScenePos;
                c.UpdateBounds = false;

                if (!c.UpdatedInitial)
                {
                    c.Bounds = c.Size.ToRectangle();
                    c.UpdatedInitial = true;
                }

                if (hasParent)
                {
                    c.ScenePos = parent.ScenePos + c.Position;
                    if (c.LastPos - parent.ScenePos != c.Position)
                    {
                        parent.UpdateBounds = true;
                        parent.Bounds = parent.Size.ToRectangle();
                    }
                    if (parent.UpdateBounds) c.UpdateBounds = true;
                }
                else c.ScenePos = c.Position;
            });

            ecs.Query<Transform>((ref Transform c, ref Transform parent, bool hasParent) =>
            {
                if (hasParent && parent.UpdateBounds)
                {
                    var b = c.Bounds;
                    b.Offset(c.Position);
                    parent.Bounds = Rectangle.Union(parent.Bounds, b);
                }
            },
            reverse: true);
        }
    }
}