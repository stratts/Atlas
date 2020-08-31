
namespace Industropolis.Engine
{
    public interface IComponent
    {
        Node Parent { get; set; }
        bool Enabled { get; set; }
        double Priority { get; set; }
    }

    public abstract class Component : IComponent
    {
        private bool _enabled = true;

        public Node Parent { get; set; } = null!;
        public bool Enabled
        {
            get => _enabled && Parent != null ? Parent.Enabled : _enabled;
            set => _enabled = value;
        }
        public double Priority { get; set; }
    }
}