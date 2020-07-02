
namespace Industropolis.Engine
{
    public interface IUpdateable
    {
        bool Enabled { get; }
        void Update(float elapsed);
    }
}