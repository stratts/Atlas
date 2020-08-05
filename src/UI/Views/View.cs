using System;

namespace Industropolis.Engine.UI
{
    public abstract class View
    {
        protected abstract Node Node { get; }

        public static implicit operator Node(View v)
        {
            var node = v.Node;
            if (node.GetComponent<Layout>() is null) node.AddComponent<Layout>();
            return node;
        }

        protected static T Modify<T>(T obj, Action func) where T : View
        {
            func?.Invoke();
            return obj;
        }
    }
}