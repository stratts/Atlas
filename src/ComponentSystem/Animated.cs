using System;
using System.Collections.Generic;
using Atlas.Anim;
using Necs;

namespace Atlas
{
    public class Animated
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

        public Animated() { }

        public Animated(IAnimation animation) => CurrentAnimation = animation;

        public void Reset() => _currentAnimation?.Reset();
    }

    public class AnimationSystem : IComponentSystem<UpdateContext>
    {
        public void Process(UpdateContext context, EcsContext ecs)
        {
            ecs.Query((ref Animated c) =>
            {
                c.CurrentTime += context.ElapsedTime;
                c.CurrentAnimation?.Update(c.CurrentTime);
            });
        }
    }
}