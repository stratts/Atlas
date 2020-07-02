
namespace Industropolis.Engine
{
    public abstract class Component
    {
        private bool _enabled = true;

        public Node Parent { get; set; } = null!;
        public bool Enabled
        {
            get => _enabled && Parent != null ? Parent.Enabled : _enabled;
            set => _enabled = value;
        }
    }
}