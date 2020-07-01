
namespace Industropolis
{
    public interface IUpdateable
    {
        bool Enabled { get; }
        void Update(float elapsed);
    }
}