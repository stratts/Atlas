using System;
using System.Collections.Generic;
using Atlas.Anim;

namespace Atlas
{
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