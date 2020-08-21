using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Industropolis.Engine
{
    public abstract class BaseSplitContainer
    {
        private class Section : IContainer
        {
            public Vector2 Offset;
            public Vector2 Size;
            public float MinSpace { get; set; }
            public float? MaxSpace { get; set; }
            public LayoutBorder Padding { get; set; } = LayoutBorder.None;

            LayoutBorder IContainer.Padding => Padding;
            Vector2 IContainer.Offset => Offset;
            Vector2 IContainer.Size => Size;
        }

        private Vector2 _size;
        private List<Section> _sections = new List<Section>();

        public Vector2 Size { get => _size; set => Resize(value); }
        public int Spacing { get; set; }

        protected abstract ref float Primary(ref Vector2 v);
        protected abstract ref float Secondary(ref Vector2 v);

        public IContainer AddSection(float minSpace = 0, float? maxSpace = null, LayoutBorder padding = default(LayoutBorder))
        {
            var section = new Section();
            section.MinSpace = minSpace;
            section.MaxSpace = maxSpace;
            if (padding != default(LayoutBorder)) section.Padding = padding;
            _sections.Add(section);
            Resize(_size);
            return section;
        }

        public void ClearSections() => _sections.Clear();

        public void Layout()
        {
            float fixedSpace = _sections.Select(c => c.MinSpace).Sum() + (Spacing * (_sections.Count - 1));
            if (Primary(ref _size) < fixedSpace) Primary(ref _size) = fixedSpace;

            Func<Section, float> MaxSpace = (s) =>
            {
                var maxSpace = !s.MaxSpace.HasValue ? Primary(ref _size) : s.MaxSpace.Value;
                return Math.Max(0, maxSpace - s.MinSpace);
            };

            float availableSpace = Primary(ref _size) - fixedSpace;
            float totalRequestedSpace = _sections.Select(c => MaxSpace(c)).Sum();

            foreach (var container in _sections)
            {
                float allocatedSpace = 0;
                if (!container.MaxSpace.HasValue || container.MaxSpace > container.MinSpace)
                {
                    allocatedSpace = MaxSpace(container) / totalRequestedSpace;
                }
                Primary(ref container.Size) = container.MinSpace + availableSpace * allocatedSpace;
                Secondary(ref container.Size) = Secondary(ref _size);
            }

            for (int i = 1; i < _sections.Count; i++)
            {
                var c = _sections[i];
                Primary(ref c.Offset) = Primary(ref _sections[i - 1].Offset) + Primary(ref _sections[i - 1].Size) + Spacing;
                Secondary(ref c.Offset) = 0;
            }
        }

        private void Resize(Vector2 size)
        {
            if (_size == size) return;
            _size = size;
            Layout();
        }
    }

    public class ColumnContainer : BaseSplitContainer
    {
        protected override ref float Primary(ref Vector2 v) => ref v.X;
        protected override ref float Secondary(ref Vector2 v) => ref v.Y;
    }

    public class RowContainer : BaseSplitContainer
    {
        protected override ref float Primary(ref Vector2 v) => ref v.Y;
        protected override ref float Secondary(ref Vector2 v) => ref v.X;
    }
}