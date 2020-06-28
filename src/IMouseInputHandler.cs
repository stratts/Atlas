using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Industropolis
{
    public interface IMouseInputHandler
    {
        Vector2 GlobalPosition { get; }
        Rectangle InputArea { get; }
        void OnClick(MouseState state, Vector2 pos);
    }
}