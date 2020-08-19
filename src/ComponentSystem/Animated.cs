using System;
using System.Collections.Generic;

namespace Industropolis.Engine
{
    using Industropolis.Engine.Anim;

    public class Animated : Component
    {
        public Animation? CurrentAnimation { get; set; }
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

    public class AnimationSet<T> where T : notnull
    {
        private Dictionary<T, Animation> _animations = new Dictionary<T, Animation>();

        public Animation this[T key]
        {
            get => Get(key);
            set => Add(key, value);
        }

        public void Add(T id, Animation animation) => _animations.Add(id, animation);

        public Animation Get(T id) => _animations[id];
    }

    public class Animation
    {
        private List<IAnimationTrack> _tracks = new List<IAnimationTrack>();

        public void AddTrack(IAnimationTrack track) => _tracks.Add(track);

        public void Update(float time)
        {
            foreach (var track in _tracks) track.Update(time);
        }
    }

    public interface IAnimationTrack
    {
        void Update(float time);
    }

    public class Track<T> : IAnimationTrack where T : struct
    {
        private Action<T> _setter;
        private List<(float time, T value)> _frames = new List<(float time, T value)>();

        public Track(Action<T> setter)
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
            foreach (var frame in _frames)
            {
                if (frame.time >= time)
                {
                    _setter.Invoke(frame.value);
                    break;
                }
            }
        }
    }
}