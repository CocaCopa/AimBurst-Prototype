using System;
using System.Runtime.CompilerServices;

namespace CocaCopa.Primitives {
    /// <summary>
    /// Unity-free 2D vector.
    /// </summary>
    public readonly struct C_Vector2 : IEquatable<C_Vector2> {
        public readonly float X;
        public readonly float Y;

        public static readonly C_Vector2 Zero = new(0f, 0f);
        public static readonly C_Vector2 One = new(1f, 1f);
        public static readonly C_Vector2 Up = new(0f, 1f);
        public static readonly C_Vector2 Down = new(0f, -1f);
        public static readonly C_Vector2 Left = new(-1f, 0f);
        public static readonly C_Vector2 Right = new(1f, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public C_Vector2(float x, float y) { X = x; Y = y; }

        // Basic ops
        public static C_Vector2 operator +(C_Vector2 a, C_Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static C_Vector2 operator -(C_Vector2 a, C_Vector2 b) => new(a.X - b.X, a.Y - b.Y);
        public static C_Vector2 operator -(C_Vector2 v) => new(-v.X, -v.Y);
        public static C_Vector2 operator *(C_Vector2 v, float s) => new(v.X * s, v.Y * s);
        public static C_Vector2 operator *(float s, C_Vector2 v) => new(v.X * s, v.Y * s);
        public static C_Vector2 operator /(C_Vector2 v, float s) => new(v.X / s, v.Y / s);

        public float Length() => MathF.Sqrt(X * X + Y * Y);
        public float LengthSquared() => X * X + Y * Y;

        public static float Dot(C_Vector2 a, C_Vector2 b) => a.X * b.X + a.Y * b.Y;
        public static float Cross(C_Vector2 a, C_Vector2 b) => a.X * b.Y - a.Y * b.X; // 2D pseudo-cross

        public C_Vector2 Normalized() {
            var lsq = LengthSquared();
            if (lsq <= 0f) return Zero;
            var inv = 1f / MathF.Sqrt(lsq);
            return new C_Vector2(X * inv, Y * inv);
        }

        public static C_Vector2 Lerp(C_Vector2 a, C_Vector2 b, float t) {
            t = t < 0f ? 0f : (t > 1f ? 1f : t);
            return new C_Vector2(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }

        public static C_Vector2 ClampMagnitude(C_Vector2 v, float max) {
            var lsq = v.LengthSquared();
            if (lsq <= max * max) return v;
            var inv = max / MathF.Sqrt(lsq);
            return new C_Vector2(v.X * inv, v.Y * inv);
        }

        public bool Equals(C_Vector2 other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is C_Vector2 v && Equals(v);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X}, {Y})";

        public C_Vector2 WithX(float x) => new(x, Y);
        public C_Vector2 WithY(float y) => new(X, y);
    }
}
