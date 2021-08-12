using Microsoft.Xna.Framework;

namespace Atlas.UI
{
    public class ViewNode : Node
    {
        private View? _view;
        private Node? _node;

        public bool HasView => _view != null;

        public Node? Node => _node;

        public ViewNode(bool fill = true)
        {
            if (fill) AddComponent(new Layout() { Fill = new Vector2(1, 1) });
        }

        public ViewNode(View view, bool fill = true) : this(fill) => Update(view);

        public void Update(View newView)
        {
            ClearView();
            _node = newView.GetNode();
            AddChild(_node);
            _view = newView;
            Size = _node.Size;
        }

        public void ClearView()
        {
            if (_view != null) RemoveChild(_view.GetNode());
            _view = null;
        }
    }
}