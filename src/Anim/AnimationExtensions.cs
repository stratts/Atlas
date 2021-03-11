using System;

namespace Atlas.Anim
{
    public class GenerateAnimations : Attribute
    {

    }

    public static class AnimationExtensions
    {
        /// <summary>
        /// Creates an animation that moves between frames of a sprite
        /// </summary>
        /// <param name="sprite">The sprite to animate</param>
        /// <param name="start">The frame to start at</param>
        /// <param name="end">The frame to end at</param>
        /// <param name="interval">Interval between frames, in seconds</param>
        /// <param name="loop">Whether the animation should loop at end</param>
        /// <returns></returns>
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