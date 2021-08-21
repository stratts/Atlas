using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static Atlas.Collision.Direction;
using Necs;

namespace Atlas
{
    internal static class DirectionExtensions
    {
        public static Collision.Direction Invert(this Collision.Direction direction)
        {
            return direction switch
            {
                Left => Right,
                Right => Left,
                Top => Bottom,
                Bottom => Top,
                _ => direction
            };
        }
    }

    public struct CollisionInfo
    {
        public Collision Component { get; }
        public Collision Source { get; }
        public Entity SourceEntity { get; }
        public Collision.Direction Direction { get; }
        public Vector2 Vector { get; }
        public float Coordinate { get; }

        public CollisionInfo(Entity source, Collision component, Collision collided, Vector2 vector, float coordinate, Collision.Direction direction)
        {
            SourceEntity = source;
            Vector = vector;
            Component = component;
            Source = collided;
            Coordinate = coordinate;
            Direction = direction;
        }
    }

    public class Collision
    {
        [Flags]
        public enum Direction { None = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }
        public Direction? RestrictDirection { get; set; }
        public Rectangle? Area { get; set; }
        public Action<CollisionInfo>? OnCollision { get; set; }
        public Rectangle CollisionBox { get; set; }
        public int? Mask { get; set; }
        public bool CollidingWith(Collision other) => CollisionBox.Intersects(other.CollisionBox);
    }

    public class CollisionSystem : IComponentSystem<UpdateContext>
    {
        private const int _gridSize = 128;
        private Dictionary<(int, int), List<(ulong, Collision)>> _grid = new();

        public void Process(UpdateContext context, EcsContext ecs)
        {
            ecs.Query((ref Collision c, ref Transform t) =>
            {
                var area = c.Area != null ? c.Area.Value : t.Bounds;
                area.Offset(t.ScenePos);
                c.CollisionBox = area;
            });

            var iter = ecs.GetIterator<Collision>();

            foreach (var list in _grid.Values) list.Clear();

            // Assign components to grid cells
            while (iter.MoveNext())
            {
                var c = iter.Component;
                var entity = iter.Entity.Id;
                var box = c.CollisionBox;

                for (int x = box.Left / _gridSize; x < box.Right / _gridSize + 1; x++)
                {
                    for (int y = box.Top / _gridSize; y < box.Bottom / _gridSize + 1; y++)
                    {
                        if (!_grid.TryGetValue((x, y), out var list))
                        {
                            list = new();
                            _grid[(x, y)] = list;
                        }

                        list.Add((entity, c));
                    }
                }
            }

            // Check collisions within each cell
            foreach (var cell in _grid.Values)
            {
                foreach (var (entityA, a) in cell)
                {
                    if (a.OnCollision == null) continue;

                    foreach (var (entityB, b) in cell)
                    {
                        if (a == b || (a.Mask == b.Mask && a.Mask.HasValue && b.Mask.HasValue)) continue;

                        if (a.CollidingWith(b))
                        {
                            var info = GetInfo(a.CollisionBox, b.CollisionBox);
                            if (a.RestrictDirection.HasValue && !a.RestrictDirection.Value.HasFlag(info.dir)) continue;
                            if (b.RestrictDirection.HasValue && !b.RestrictDirection.Value.HasFlag(info.dir.Invert())) continue;
                            var collisionInfo = new CollisionInfo(ecs.GetEntity(entityB), a, b, info.vector, info.coord, info.dir);
                            a.OnCollision?.Invoke(collisionInfo);
                        }
                    }
                }
            }
        }

        private (float coord, Collision.Direction dir, Vector2 vector) GetInfo(Rectangle a, Rectangle b)
        {
            // Find edges of the two boxes that are closest
            Span<(Collision.Direction dir, int dist)> distances = stackalloc[] {
                (Left, Math.Abs(a.Left - b.Right)),
                (Right, Math.Abs(a.Right - b.Left)),
                (Top, Math.Abs(a.Top - b.Bottom)),
                (Bottom, Math.Abs(a.Bottom - b.Top)),
            };

            int? minDist = null;
            var direction = default(Collision.Direction);

            foreach (var d in distances)
            {
                if (!minDist.HasValue || d.dist < minDist)
                {
                    minDist = d.dist;
                    direction = d.dir;
                }
            }

            // Coordinate is located on box that is being collided with
            int coordinate = direction switch
            {
                Left => b.Right,
                Right => b.Left,
                Top => b.Bottom,
                Bottom => b.Top,
                _ => 0
            };

            var vector = a.Center.ToVector2().DirectionTo(Rectangle.Intersect(a, b).Center.ToVector2());

            return (coordinate, direction, vector);
        }
    }
}