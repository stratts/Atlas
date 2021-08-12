using System;
using Microsoft.Xna.Framework;

namespace Atlas.UI
{
    public abstract class View
    {
        internal abstract Node Node { get; }

        public Vector2 Size => Node.Size;

        public Node GetNode()
        {
            Node.GetOrAddComponent<Layout>();
            return Node;
        }

        protected static T Modify<T>(T obj, Action func) where T : View
        {
            func?.Invoke();
            return obj;
        }
    }
}