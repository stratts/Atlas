using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine.UI
{
    public static class NodeExtensions
    {
        private static T Modify<T>(T obj, Action<T> func)
        {
            func?.Invoke(obj);
            return obj;
        }

        private static Layout GetLayout(this Node node)
        {
            return node.GetComponent<Layout>() is Layout l ? l : node.AddComponent<Layout>();
        }

        public static T InitComponents<T>(this T node, params Component[] components) where T : Node
        {
            foreach (var c in components) node.AddComponent(c);
            return node;
        }

        public static T WithUpdate<T>(this T node, Action<T> func) where T : Node
        {
            func?.Invoke(node);
            node.AddComponent(new Updateable() { UpdateMethod = (elapsed) => func?.Invoke(node) });
            return node;
        }

        public static T Layout<T>(this T node) where T : Node
        {
            node.GetLayout();
            return node;
        }

        public static T Margin<T>(this T node, float left = 0, float right = 0, float top = 0, float bottom = 0) where T : Node
        {
            node.GetLayout().Margin = new LayoutBorder(left, right, top, bottom);
            return node;
        }

        public static T Fill<T>(this T node, float width = 0, float height = 0) where T : Node
        {
            node.GetLayout().Fill = new Vector2(width, height);
            return node;
        }

        public static T Align<T>(this T node, HAlign h = HAlign.None, VAlign v = VAlign.None) where T : Node
        {
            node.GetLayout().HAlign = h;
            node.GetLayout().VAlign = v;
            return node;
        }

        public static T Expand<T>(this T node) where T : Node => Modify(node, node => node.AddComponent<StackBox.Expand>());

        public static Button OnClicked(this Button button, Action onClick) =>
            Modify(button, button => button.OnClick = onClick);

        public static Text Color(this Text text, Color color) =>
            Modify(text, text => text.Color = color);
    }
}