using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Atlas.Anim
{
    public class AnimationSet : AnimationSet<string> { }

    /// <summary>
    /// A collection of animations accessed using a key of type T
    /// </summary>
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
        bool Complete { get; }
        event Action? Completed;
        void Reset();
    }

    /// <summary>
    /// An animation consisting of multiple animations that are updated simultaneously
    /// </summary>
    public class CompoundAnimation : IAnimation
    {
        private List<IAnimation> _animations = new List<IAnimation>();

        public bool Complete { get; private set; } = false;

        public event Action? Completed;

        public void AddAnimation(IAnimation track) => _animations.Add(track);

        public CompoundAnimation(params IAnimation[] animations)
        {
            foreach (var anim in animations) AddAnimation(anim);
        }

        public void Update(float time)
        {
            foreach (var track in _animations) track.Update(time);

            foreach (var track in _animations)
            {
                if (!track.Complete) return;
            }

            if (!Complete)
            {
                Completed?.Invoke();
                Complete = true;
            }
        }

        public void Reset()
        {
            foreach (var track in _animations) track.Reset();
            Complete = false;
        }
    }

    public interface Interpolator<T>
    {
        T Interpolate(T a, T b, float amount);
    }

    public delegate T Interpolate<T>(T a, T b, float amount);

    internal class DefaultInterpolator : Interpolator<int>, Interpolator<float>, Interpolator<Vector2>, Interpolator<Color>
    {
        public int Interpolate(int a, int b, float amount) => (int)(a + (b - a) * amount);
        public float Interpolate(float a, float b, float amount) => a + (b - a) * amount;
        public Vector2 Interpolate(Vector2 a, Vector2 b, float amount) => a + (b - a) * amount;
        public Color Interpolate(Color a, Color b, float amount) => a.Mix(b, amount);
    }

    public struct AnimationFrame<T>
    {
        public float Time { get; }
        public T Value { get; }
        public EaseType? EaseType { get; }

        public AnimationFrame(float time, T value, EaseType? easeType = null) =>
            (Time, Value, EaseType) = (time, value, easeType);
    }

    /// <summary>
    /// Animates a value of type <typeparamref name="T"/>, using a collection of frames
    /// with optional interpolation and looping
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Animation<T> : IAnimation where T : struct
    {
        private static DefaultInterpolator _defaultInterpolator = new DefaultInterpolator();
        private Interpolate<T>? _interpolateFunc;
        private Action<T> _setter;
        private List<AnimationFrame<T>> _frames = new List<AnimationFrame<T>>();
        private bool _complete = false;

        public bool Loop { get; set; } = false;
        public float? Length { get; set; }
        public EaseType EaseType { get; set; } = EaseType.None;
        public bool Complete
        {
            get => _complete;
            private set
            {
                if (value == true && !_complete) Completed?.Invoke();
                _complete = value;
            }
        }

        public event Action? Completed;

        /// <summary>
        /// Creates an empty animation
        /// </summary>
        /// <param name="setter">The delegate used to set the value</param>
        /// <param name="interpolate">Whether or not to interpolate (or 'tween') values between frames</param>
        public Animation(Action<T> setter, bool interpolate = false)
        {
            if (interpolate && _defaultInterpolator is Interpolator<T> i) _interpolateFunc = i.Interpolate;
            _setter = setter;
        }

        public void AddFrame(float time, T value, EaseType? easeType = null)
        {
            _frames.Add(new AnimationFrame<T>(time, value, easeType));
            _frames.Sort((a, b) => a.Time.CompareTo(b.Time));
        }

        public void AddFrames(params (float time, T value)[] frames)
        {
            foreach (var frame in frames) AddFrame(frame.time, frame.value);
        }

        public void Update(float time)
        {
            float length = Length.HasValue ? Length.Value : _frames[_frames.Count - 1].Time;
            if (Loop && length > 0) time = time % length;
            if (time > length && !Loop)
            {
                if (!Complete) _setter.Invoke(_frames[_frames.Count - 1].Value);
                Complete = true;
                return;
            }

            for (int i = 0; i < _frames.Count; i++)
            {
                var frame = _frames[i];
                float current = frame.Time;
                float next = i < _frames.Count - 1 ? _frames[i + 1].Time : length;

                if (time >= current && time < next)
                {
                    T value;
                    if (_interpolateFunc != null && i < _frames.Count - 1)
                    {
                        if (frame.EaseType != null) EaseType = frame.EaseType.Value;
                        var nextFrame = _frames[i + 1];
                        var interpolateAmount = (time - current) / (next - current);
                        if (EaseType != EaseType.None) interpolateAmount = Easings.Interpolate(interpolateAmount, EaseType);
                        value = _interpolateFunc.Invoke(frame.Value, nextFrame.Value, interpolateAmount);
                    }
                    else value = frame.Value;
                    _setter.Invoke(value);
                    break;
                }
            }
        }

        public void Reset()
        {
            Complete = false;
        }

        public static Animation<T> Create(Action<T> setter, T start, T end, float length, bool loop = false, EaseType easeType = EaseType.Linear, bool pingPong = false)
        {
            var anim = new Animation<T>(setter, true)
            {
                Length = !pingPong ? length : length * 2,
                Loop = loop,
                EaseType = easeType
            };

            anim.AddFrames((0, start), (length, end));
            if (pingPong) anim.AddFrame(length * 2, start);
            return anim;
        }
    }
}