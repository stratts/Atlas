using System;
using System.Collections;
using System.Collections.Generic;

namespace Industropolis.Engine
{
    using Industropolis.Engine.Anim;

    public class Animated : Component
    {
        private IAnimation? _currentAnimation;
        public IAnimation? CurrentAnimation
        {
            get => _currentAnimation;
            set
            {
                if (_currentAnimation == value) return;
                _currentAnimation = value;
                CurrentTime = 0;
            }
        }
        public float CurrentTime { get; set; } = 0;
    }

    public class AnimationSystem : BaseComponentSystem<Animated>
    {
        public override void UpdateComponents(Scene scene, IReadOnlyList<Animated> components, float elapsed)
        {
            foreach (var c in components)
            {
                if (!c.Enabled) continue;
                c.CurrentTime += elapsed;
                c.CurrentAnimation?.Update(c.CurrentTime);
            }
        }
    }
}

namespace Industropolis.Engine.Anim
{
    public class AnimationSet : AnimationSet<string> { }

    public class AnimationSet<T> : IEnumerable where T : notnull
    {
        private Dictionary<T, IAnimation> _animations = new Dictionary<T, IAnimation>();

        public IAnimation this[T key]
        {
            get => Get(key);
            set => Add(key, value);
        }

        public void Add(T id, IAnimation animation) => _animations.Add(id, animation);

        public IAnimation Get(T id) => _animations[id];

        public IEnumerator GetEnumerator() => ((IEnumerable)_animations).GetEnumerator();
    }

    public interface IAnimation
    {
        void Update(float time);
    }

    public class CompoundAnimation : IAnimation
    {
        private List<IAnimation> _animations = new List<IAnimation>();

        public void AddAnimation(IAnimation track) => _animations.Add(track);

        public void Update(float time)
        {
            foreach (var track in _animations) track.Update(time);
        }
    }

    public class Animation<T> : IAnimation where T : struct
    {
        private Action<T> _setter;
        private List<(float time, T value)> _frames = new List<(float time, T value)>();

        public bool Loop { get; set; } = false;
        public float? Length { get; set; }

        public Animation(Action<T> setter)
        {
            _setter = setter;
        }

        public void AddFrame(float time, T value)
        {
            _frames.Add((time, value));
            _frames.Sort((a, b) => a.time.CompareTo(b.time));
        }

        public void AddFrames(params (float time, T value)[] frames)
        {
            foreach (var frame in frames) AddFrame(frame.time, frame.value);
        }

        public void Update(float time)
        {
            float length = Length.HasValue ? Length.Value : _frames[_frames.Count - 1].time;
            if (Loop && length > 0) time = time % length;

            for (int i = 0; i < _frames.Count; i++)
            {
                var frame = _frames[i];
                float current = frame.time;
                float next = i < _frames.Count - 1 ? _frames[i + 1].time : length;

                if (time >= current && time < next)
                {
                    _setter.Invoke(frame.value);
                    break;
                }
            }
        }
    }

    public static class AnimationExtensions
    {
        public static Animation<int> AnimateFrame(this Sprite sprite, int start, int end, float interval, bool loop = true)
        {
            var animation = new Animation<int>(value => sprite.CurrentFrame = value);
            animation.Length = interval * (end - start + 1);
            animation.Loop = loop;
            for (int i = start; i <= end; i++) animation.AddFrame(interval * (i - start), i);
            return animation;
        }
    }
}