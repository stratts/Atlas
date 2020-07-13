using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public interface IComponentSystem
    {
        bool HandlesComponent(Component component);
        void AddComponent(Component component);
        void RemoveComponent(Component component);
        void UpdateComponents(Scene scene, float elapsed);
        void SortComponents();
    }

    public interface IRenderSystem : IComponentSystem
    {
        void Draw(Scene scene, SpriteBatch spriteBatch);
    }
}