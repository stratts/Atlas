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
        public Collision.Direction Direction { get; }
        public float Coordinate { get; }

        public CollisionInfo(float coordinate, Collision.Direction direction)
        {
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
    }

    public class CollisionSystem : BaseComponentSystem<Collision>
    {
        public override void UpdateComponents(Scene scene, IReadOnlyList<Collision> components, float elapsed)
        {
            foreach (var a in components)
            {
                var areaA = a.Area.HasValue ? a.Area.Value : new Rectangle(Point.Zero, a.Parent.Size.ToPoint());
                var boxA = new Rectangle(areaA.Location + a.Parent.ScenePosition.ToPoint(), areaA.Size);

                foreach (var b in components)
                {
                    if (a == b) continue;
                    var areaB = b.Area.HasValue ? b.Area.Value : new Rectangle(Point.Zero, b.Parent.Size.ToPoint());
                    var boxB = new Rectangle(areaB.Location + b.Parent.ScenePosition.ToPoint(), areaB.Size);

                    if (boxA.Intersects(boxB))
                    {
                        var info = GetInfo(boxA, boxB);
                        if (a.RestrictDirection.HasValue && !a.RestrictDirection.Value.HasFlag(info.Direction)) continue;
                        if (b.RestrictDirection.HasValue && !b.RestrictDirection.Value.HasFlag(info.Direction.Invert())) continue;
                        a.OnCollision?.Invoke(info);
                    }
                }
            }
        }

        private CollisionInfo GetInfo(Rectangle a, Rectangle b)
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

            return new CollisionInfo(coordinate, direction);
        }
    }
}