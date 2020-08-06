using System;

namespace Industropolis.Engine.UI
{
    public abstract class View
    {
        protected abstract Node Node { get; }

        public Node GetNode()
        {
            if (Node.GetComponent<Layout>() is null) Node.AddComponent<Layout>();
            return Node;
        }

        protected static T Modify<T>(T obj, Action func) where T : View
        {
            func?.Invoke();
            return obj;
        }
    }
}