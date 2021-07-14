using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Atlas
{
    public class Modulate : Component
    {
        public static Tag Disable = Tag.New();

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
            var cHsv = c.ToHsv();
            cHsv.Value += Brightness;
            c = cHsv.ToRgb();
            c.A = color.A;
            return c.Multiply(tint) * opacity;
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

                if (c.InheritFrom != null && !c.InheritFrom.Enabled) c.InheritFrom = null;

                foreach (var child in c.Parent.Children)
                {
                    if (!child.HasTag(Modulate.Disable)) Inherit(child, c);
                }
            }
        }

        private void Inherit(Node node, Modulate component)
        {
            if (node.GetComponent<Modulate>() is Modulate m) m.InheritFrom = component;
            foreach (var child in node.Children)
            {
                if (!child.HasTag(Modulate.Disable)) Inherit(child, component);
            }
        }
    }
}

