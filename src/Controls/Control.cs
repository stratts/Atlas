using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public abstract class Control : Node
    {
        private Vector2 _size;

        public override Vector2 Size
        {
            get => _size;
            set { _size = value; OnSizeChanged(_size); }
        }

        protected abstract void OnSizeChanged(Vector2 size);
    }
}