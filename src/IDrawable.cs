using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public interface IDrawable
    {
        Vector2 Size { get; }
        Vector2 ScenePosition { get; }
        void Draw(SpriteBatch spriteBatch, Vector2 position);
    }
}