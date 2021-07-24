using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Atlas
{
    public struct IntVector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static readonly IntVector Zero = new IntVector(0, 0);

        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static IntVector operator +(IntVector a, IntVector b)
        {
            return new IntVector(a.X + b.X, a.Y + b.Y);
        }

        public static bool operator ==(IntVector a, IntVector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(IntVector a, IntVector b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static IntVector operator -(IntVector a, IntVector b)
        {
            return new IntVector(a.X - b.X, a.Y - b.Y);
        }

        public static IntVector operator -(IntVector a)
        {
            return new IntVector(-a.X, -a.Y);
        }

        public static IntVector operator *(IntVector a, int b)
        {
            return new IntVector(a.X * b, a.Y * b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is IntVector p)
            {
                return this == p;
            }
            else return false;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static IntVector Parse(ReadOnlySpan<char> input)
        {
            Span<char> trimChars = stackalloc char[] { '(', ')' };
            var trimmed = input.Trim(trimChars);
            var sep = trimmed.IndexOf(',');
            var x = trimmed.Slice(0, sep).Trim();
            var y = trimmed.Slice(sep + 1).Trim();
            return (int.Parse(x), int.Parse(y));
        }

        public bool WithinBounds(int width, int height)
        {
            return (X >= 0 && Y >= 0 && X < width && Y < height);
        }

        public IEnumerable<IntVector> GetNeighbours(bool fourWay = false)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    if (fourWay && Math.Abs(x) == Math.Abs(y)) continue;
                    yield return new IntVector(X + x, Y + y);
                }
            }
        }

        public IEnumerable<IntVector> GetPointsBetween(IntVector dest)
        {
            var pos = new Vector2(X, Y);
            var diff = new Vector2(dest.X - X, dest.Y - Y);
            var len = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
            var norm = diff / len;
            for (int i = 0; i <= len; i++)
            {
                yield return new IntVector((int)Math.Round(pos.X), (int)Math.Round(pos.Y));
                pos += norm;
            }
        }

        public Vector2 Direction(IntVector dest) => Vector2.Normalize((dest - this).ToVector2()).Round(2);

        public IntVector Direction8(IntVector dest)
        {
            var dir = Direction(dest).Rounded();
            return new IntVector((int)dir.X, (int)dir.Y);
        }

        public float Distance(IntVector dest) => (dest - this).ToVector2().Length();

        public float DistanceSquared(IntVector dest) => (dest - this).ToVector2().LengthSquared();

        public bool IsParallelTo(IntVector vector) => this.ToVector2().IsParallelTo(vector.ToVector2());

        public Vector2 ToVector2() => new Vector2(X, Y);

        public static implicit operator IntVector((int x, int y) t) => new IntVector(t.x, t.y);

        public void Deconstruct(out int x, out int y) => (x, y) = (X, Y);

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }
    }
}
