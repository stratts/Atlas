
namespace Industropolis.Engine.UI
{
    public static class Markup
    {
        public static ButtonView Button(string label) => new ButtonView(label);
        public static TextView Text(string content = "") => new TextView() { Content = content };
        public static VBoxView VBox(params View[] children) => new VBoxView(children);
        public static HBoxView HBox(params View[] children) => new HBoxView(children);
        public static NodeView Node(Node node) => new NodeView(node);
    }
}