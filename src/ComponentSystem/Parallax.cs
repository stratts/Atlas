using System.Collections.Generic;
using Industropolis.Engine;
using Microsoft.Xna.Framework;

namespace SimplePlatformer
{
    public class Parallax : Updateable
    {
        private Vector2? _prevCameraPos;

        public float Amount { get; set; } = 1;

        public Parallax() => UpdateMethod = Update;

        private void Update(Scene scene, float elapsed)
        {
            if (_prevCameraPos.HasValue)
            {
                Parent.Position -= (scene.Camera.Position - _prevCameraPos.Value) * (-1f + Amount);
            }
            _prevCameraPos = scene.Camera.Position;
        }
    }
}