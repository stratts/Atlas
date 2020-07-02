using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public interface IDrawable
    {
        bool Enabled { get; set; }
        Vector2 Size { get; }
        Rectangle DrawBounds { get { return new Rectangle(0, 0, (int)Size.X, (int)Size.Y); } }
        Vector2 ScenePosition { get; }
        float Opacity { get; set; }
        void Draw(SpriteBatch spriteBatch, Vector2 position);
    }
}