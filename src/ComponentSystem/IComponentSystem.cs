using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public interface IComponentSystem
    {
        bool HandlesComponent(IComponent component);
        void AddComponent(IComponent component);
        void RemoveComponent(IComponent component);
        void UpdateComponents(Scene scene, float elapsed);
        void SortComponent(IComponent component);
    }

    public interface IRenderSystem : IComponentSystem
    {
        void Draw(Scene scene, SpriteBatch spriteBatch);
    }
}