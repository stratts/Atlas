
namespace Atlas
{
    public interface IComponent
    {
        string Name { get; set; }
        Node Parent { get; set; }
        bool Enabled { get; set; }
        double Priority { get; set; }
    }

    public abstract class Component : IComponent
    {
        private bool _enabled = true;

        public string Name { get; set; } = "";

        public Node Parent { get; set; } = null!;

        public bool Enabled
        {
            get => _enabled && Parent != null ? Parent.Enabled : _enabled;
            set => _enabled = value;
        }

        public double Priority { get; set; }
    }
}