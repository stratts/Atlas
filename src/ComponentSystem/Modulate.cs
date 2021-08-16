using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Necs;

namespace Atlas
{
    public class Modulate
    {
        public static Tag Disable = Tag.New();

        public bool Enabled = true;
        public Modulate? InheritFrom { get; set; } = null;
        public float Opacity { get; set; } = 1f;
        public Color Tint { get; set; } = Colors.White;
        public float Brightness = 0f;

        public Color ModulateColor(Color color)
        {
            if (!Enabled) return color;
            var opacity = InheritFrom != null ? Opacity * InheritFrom.Opacity : Opacity;
            var tint = InheritFrom != null && InheritFrom.Tint != Colors.White ? InheritFrom.Tint : Tint;
            var brightness = InheritFrom != null ? InheritFrom.Brightness + Brightness : Brightness;
            var c = color;
            var cHsv = c.ToHsv();
            cHsv.Value += brightness;
            if (tint != Colors.White) cHsv.Saturation = 0;
            c = cHsv.ToRgb();
            c.A = color.A;
            return c.Multiply(tint) * opacity;
        }
    }

    public class ModulateSystem : IComponentSystem<UpdateContext>
    {
        public void Process(UpdateContext context, IEcsContext ecs)
        {
            ecs.Query((ref Modulate a, ref Modulate? parent, bool hasParent) =>
            {
                if (parent != null) a.InheritFrom = parent;
            });

            ecs.Query((ref Drawable a, ref Modulate b) =>
            {
                if (a.Modulate != b) a.Modulate = b;
            });
        }
    }
}

