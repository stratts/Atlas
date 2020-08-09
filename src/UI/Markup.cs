using System;
using System.Collections.Generic;
using System.Linq;
using Industropolis.Engine.UI.Views;

namespace Industropolis.Engine.UI
{
    public static class Markup
    {
        public static ButtonView Button(string label, Action? onClick) => new ButtonView(label, onClick);
        public static ButtonView Button(View label, Action? onClick) => new ButtonView(label, onClick);
        public static TextView Text(string content = "") => new TextView() { Content = content };
        public static TextView Text(Func<string> updateFunction) =>
            new TextView().WithUpdate(text => text.Content = updateFunction());
        public static VBoxView VBox(params View?[] children) => new VBoxView(children);
        public static HBoxView HBox(params View?[] children) => new HBoxView(children);
        public static NodeView Node(Node node) => new NodeView(node);
        public static ContainerView Container(View view) => new ContainerView(view);
        public static PanelView Panel(View view) => new PanelView(view);
        public static VBoxView? ListView<T>(IEnumerable<T>? enumerator, Func<T, View> func)
        {
            if (enumerator == null) return null;
            else return new VBoxView(enumerator.Select(func).ToArray()).Fill(width: 1);
        }
    }
}