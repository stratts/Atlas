using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis
{
    public interface IDrawable
    {
        bool Enabled { get; set; }
        Vector2 Size { get; }
        Vector2 ScenePosition { get; }
        float Opacity { get; set; }
        void Draw(SpriteBatch spriteBatch, Vector2 position);
    }
}