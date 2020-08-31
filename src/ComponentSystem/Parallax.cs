using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Parallax : Component, ILogicComponent
    {
        private Vector2? _prevCameraPos;

        public float Amount { get; set; } = 1;

        void ILogicComponent.Update(Scene scene, float elapsed)
        {
            if (_prevCameraPos.HasValue)
            {
                Parent.Position -= (scene.Camera.Position - _prevCameraPos.Value) * (-1f + Amount);
            }
            _prevCameraPos = scene.Camera.Position;
        }
    }
}