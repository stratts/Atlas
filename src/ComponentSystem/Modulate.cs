using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Industropolis.Engine
{
    public class Modulate : Component
    {
        public Modulate? InheritFrom { get; set; } = null;
        public float Opacity { get; set; } = 1f;
        public Color Tint { get; set; } = Color.White;
        public float Brightness = 0f;

        public Color ModulateColor(Color color)
        {
            if (!Enabled) return color;
            var opacity = InheritFrom != null ? Opacity * InheritFrom.Opacity : Opacity;
            var tint = InheritFrom != null && InheritFrom.Tint != Color.White ? InheritFrom.Tint : Tint;
            var c = color;
            if (tint != Color.White)
            {
                var tHsv = tint.ToHsv();
                var cHsv = c.ToHsv();
                cHsv.Hue = tHsv.Hue;
                cHsv.Saturation = tHsv.Saturation;
                cHsv.Value += Brightness;
                c = cHsv.ToRgb();
            }
            return c * opacity;
        }
    }

    public class ModulateSystem : BaseComponentSystem<Modulate>
    {
        public ModulateSystem()
        {
            UpdateEveryTick = false;
        }

        public override void UpdateComponents(Scene scene, IReadOnlyList<Modulate> components, float elapsed)
        {
            foreach (var c in components)
            {
                if (c.Enabled == false)
                {
                    c.InheritFrom = null;
                    continue;
                }

                foreach (var child in c.Parent.Children) Inherit(child, c);
            }
        }

        private void Inherit(Node node, Modulate component)
        {
            if (node.GetComponent<Modulate>() is Modulate m) m.InheritFrom = component;
            foreach (var child in node.Children) Inherit(child, component);
        }
    }
}

