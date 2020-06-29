
namespace Industropolis
{
    public interface IComponentSystem
    {
        bool HandlesComponent(Component component);
        void AddComponent(Component component);
        void RemoveComponent(Component component);
        void UpdateComponents(Scene scene, float elapsed);
    }
}