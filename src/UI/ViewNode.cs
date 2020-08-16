using Microsoft.Xna.Framework;

namespace Industropolis.Engine.UI
{
    public class ViewNode : Node
    {
        private View? _view;

        public bool HasView => _view != null;

        public ViewNode(bool fill = true)
        {
            if (fill) AddComponent(new Layout() { Fill = new Vector2(1, 1) });
        }

        public ViewNode(View view, bool fill = true) : this(fill) => Update(view);

        public void Update(View newView)
        {
            ClearView();
            var node = newView.GetNode();
            AddChild(node);
            Size = node.Size;
            _view = newView;
        }

        public void ClearView()
        {
            if (_view != null) RemoveChild(_view.GetNode());
            _view = null;
        }
    }
}