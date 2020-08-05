
namespace Industropolis.Engine.UI
{
    public static class Markup
    {
        public static Button Button(string label) => new Button(label);
        public static Text Text(string content = "") => new Text() { Content = content };
        public static VStackBox VBox(params Node[] children)
        {
            var box = new VStackBox();
            box.AddChildren(children);
            return box;
        }

        public static HStackBox HBox(params Node[] children)
        {
            var box = new HStackBox();
            box.AddChildren(children);
            return box;
        }
    }
}