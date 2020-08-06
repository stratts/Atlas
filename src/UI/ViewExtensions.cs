using System;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine.UI
{
    public static class ViewExtensions
    {
        private static T? GetComponent<T>(this View view) where T : Component
        {
            return view.GetNode().GetComponent<T>();
        }

        private static T AddComponent<T>(this View view) where T : Component, new()
        {
            return view.GetNode().AddComponent<T>();
        }

        private static void AddComponent(this View view, Component c)
        {
            view.GetNode().AddComponent(c);
        }

        private static T Modify<T>(T obj, Action<T> func)
        {
            func?.Invoke(obj);
            return obj;
        }

        private static Layout GetLayout(this View view)
        {
            return view.GetComponent<Layout>() is Layout l ? l : view.AddComponent<Layout>();
        }

        public static T InitComponents<T>(this T view, params Component[] components) where T : View
        {
            foreach (var c in components) view.AddComponent(c);
            return view;
        }

        public static T WithUpdate<T>(this T view, Action<T> func) where T : View
        {
            func?.Invoke(view);
            view.AddComponent(new Updateable() { UpdateMethod = (elapsed) => func?.Invoke(view) });
            return view;
        }

        public static T Layout<T>(this T view) where T : View
        {
            view.GetLayout();
            return view;
        }

        public static T Margin<T>(this T view, float left = 0, float right = 0, float top = 0, float bottom = 0) where T : View
        {
            view.GetLayout().Margin = new LayoutBorder(left, right, top, bottom);
            return view;
        }

        public static T Fill<T>(this T view, float width = 0, float height = 0) where T : View
        {
            view.GetLayout().Fill = new Vector2(width, height);
            return view;
        }

        public static T Align<T>(this T view, HAlign h = HAlign.None, VAlign v = VAlign.None) where T : View
        {
            view.GetLayout().HAlign = h;
            view.GetLayout().VAlign = v;
            return view;
        }

        public static T Expand<T>(this T view) where T : View => Modify(view, view => view.AddComponent<StackBox.Expand>());

        public static T? ShowIf<T>(this T view, bool condition) where T : View => condition == true ? view : null;
    }
}