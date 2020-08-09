using System;
using Microsoft.Xna.Framework;
using Industropolis.Engine.UI.Views;

namespace Industropolis.Engine.UI
{
    public delegate T Style<T>(T view) where T : View;

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

        public static T Margin<T>(this T view, float margin) where T : View => Margin(view, margin, margin, margin, margin);
        public static T Margin<T>(this T view, float horizontal = 0, float vertical = 0) where T : View => Margin(view, horizontal, horizontal, vertical, vertical);
        public static T Margin<T>(this T view, float left = 0, float right = 0, float top = 0, float bottom = 0) where T : View
        {
            view.GetLayout().Margin = new LayoutBorder(left, right, top, bottom);
            return view;
        }

        public static T Padding<T>(this T view, float margin) where T : View, IPaddableView => Padding(view, margin, margin, margin, margin);
        public static T Padding<T>(this T view, float horizontal = 0, float vertical = 0) where T : View, IPaddableView => Padding(view, horizontal, horizontal, vertical, vertical);
        public static T Padding<T>(this T view, float left = 0, float right = 0, float top = 0, float bottom = 0) where T : View, IPaddableView
        {
            view.SetPadding(new LayoutBorder(left, right, top, bottom));
            return view;
        }

        public static T Fill<T>(this T view, float width = 0, float height = 0) where T : View
        {
            view.GetLayout().Fill = new Vector2(width, height);
            return view;
        }

        public static T Style<T>(this T view, Style<T> style) where T : View => style(view);

        public static T Align<T>(this T view, HAlign h = HAlign.None, VAlign v = VAlign.None) where T : View
        {
            view.GetLayout().HAlign = h;
            view.GetLayout().VAlign = v;
            return view;
        }
        public static T _HAlign<T>(this T view, HAlign align) where T : View => Modify(view, view => view.GetLayout().HAlign = align);
        public static T _VAlign<T>(this T view, VAlign align) where T : View => Modify(view, view => view.GetLayout().VAlign = align);

        public static T Expand<T>(this T view) where T : View => Modify(view, view => view.AddComponent<StackBox.Expand>());

        public static T? ShowIf<T>(this T view, bool condition) where T : View => condition == true ? view : null;
    }
}