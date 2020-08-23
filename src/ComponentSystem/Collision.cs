using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using static Industropolis.Engine.Collision.Direction;

namespace Industropolis.Engine
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
        public Collision.Direction Direction { get; }
        public Vector2 Vector { get; }
        public float Coordinate { get; }

        public CollisionInfo(Collision component, Collision collided, Vector2 vector, float coordinate, Collision.Direction direction)
        {
            Vector = vector;
            Component = component;
            Source = collided;
            Coordinate = coordinate;
            Direction = direction;
        }
    }

    public class Collision : Component
    {
        [Flags]
        public enum Direction { Left = 1, Right = 2, Top = 4, Bottom = 8 }
        public Direction? RestrictDirection { get; set; }
        public Rectangle? Area { get; set; }
        public Action<CollisionInfo>? OnCollision { get; set; }
        public Rectangle CollisionBox
        {
            get
            {
                var area = Area.HasValue ? Area.Value : new Rectangle(Point.Zero, Parent.Size.ToPoint());
                return new Rectangle(area.Location + Parent.ScenePosition.ToPoint(), area.Size);
            }
        }
        public bool CollidingWith(Collision other) => CollisionBox.Intersects(other.CollisionBox);
    }

    public class CollisionSystem : BaseComponentSystem<Collision>
    {
        public override void UpdateComponents(Scene scene, IReadOnlyList<Collision> components, float elapsed)
        {
            foreach (var a in components)
            {
                foreach (var b in components)
                {
                    if (a == b) continue;

                    if (a.CollidingWith(b))
                    {
                        var info = GetInfo(b.Parent, a.CollisionBox, b.CollisionBox);
                        var collisionInfo = new CollisionInfo(a, b, info.vector, info.coord, info.dir);
                        if (a.RestrictDirection.HasValue && !a.RestrictDirection.Value.HasFlag(info.dir)) continue;
                        if (b.RestrictDirection.HasValue && !b.RestrictDirection.Value.HasFlag(info.dir.Invert())) continue;
                        a.OnCollision?.Invoke(collisionInfo);
                    }
                }
            }
        }

        private (float coord, Collision.Direction dir, Vector2 vector) GetInfo(Node source, Rectangle a, Rectangle b)
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