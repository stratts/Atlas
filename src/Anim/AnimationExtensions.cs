
namespace Industropolis.Engine.Anim
{
    public static class AnimationExtensions
    {
        public static Animation<int> AnimateFrame(this Sprite sprite, int start, int end, float interval, bool loop = true)
        {
            var animation = new Animation<int>(value => sprite.CurrentFrame = value, true);
            animation.Length = interval * (end - start + 1);
            animation.Loop = loop;
            animation.AddFrame(0, start);
            animation.AddFrame(interval * (end - start), end);
            return animation;
        }
    }
}