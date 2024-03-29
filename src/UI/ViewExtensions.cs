using System;
using Microsoft.Xna.Framework;
using Atlas.UI.Views;

namespace Atlas.UI
{
    public delegate T Style<T>(T view) where T : View;

    public static class ViewExtensions
    {
        private static T? GetComponent<T>(this View view)
        {
            return view.GetNode().GetComponent<T>();
        }

        private static T AddComponent<T>(this View view) where T : new()
        {
            return view.GetNode().AddComponent<T>();
        }

        private static void AddComponent<T>(this View view, T c)
        {
            view.GetNode().AddComponent(c);
        }

        private static T Modify<T>(T obj, Action<T> func)
        {
            func?.Invoke(obj);
            return obj;
        }

        private static ref Layout GetLayout(this View view)
        {
            return ref view.Node.GetOrAddComponent<Layout>();
        }

        /*public static T InitComponents<T>(this T view, params Component[] components) where T : View
        {
            foreach (var c in components) view.AddComponent(c);
            return view;
        }*/

        public static T WithUpdate<T>(this T view, Action<T> func) where T : View
        {
            func?.Invoke(view);
            view.AddComponent(new Updateable() { UpdateMethod = (_, elapsed) => func?.Invoke(view) });
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

        public static T HAlign<T>(this T view, HAlign align) where T : View { view.GetLayout().HAlign = align; return view; }
        public static T VAlign<T>(this T view, VAlign align) where T : View { view.GetLayout().VAlign = align; return view; }

        public static T Expand<T>(this T view) where T : View => Modify(view, view => view.AddComponent<StackBox.Expand>());

        public static T? ShowIf<T>(this T view, bool condition) where T : View => condition == true ? view : null;

        public static T OnClick<T>(this T view, Action<T> func) where T : View
        {
            AddComponent(view, new MouseInput() { OnClick = _ => func?.Invoke(view) });
            return view;
        }

        public static T OnHover<T>(this T view, Action onEnter, Action onExit) where T : View
        {
            AddComponent(view, new MouseInput()
            {
                OnMouseEnter = onEnter,
                OnMouseExit = onExit
            });
            return view;
        }

        public static T Scale<T>(this T view, float amount) where T : View
        {
            var node = view.GetNode();
            node.Size *= amount;
            return view;
        }
    }
}