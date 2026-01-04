using System;

namespace CocaCopa.Primitives {
    [Serializable]
    public struct C_Vector3 {
        public float x, y, z;

        public C_Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

        public static C_Vector3 zero => new(0f, 0f, 0f);
        public static C_Vector3 one => new(1f, 1f, 1f);

        public static C_Vector3 up => new(0f, 1f, 0f);
        public static C_Vector3 down => new(0f, -1f, 0f);
        public static C_Vector3 left => new(-1f, 0f, 0f);
        public static C_Vector3 right => new(1f, 0f, 0f);
        public static C_Vector3 forward => new(0f, 0f, 1f);
        public static C_Vector3 back => new(0f, 0f, -1f);

        public float SqrMagnitude => x * x + y * y + z * z;
        public float Magnitude => (float)Math.Sqrt(SqrMagnitude);

        public C_Vector3 Normalized {
            get {
                float mag = Magnitude;
                return mag > 1e-8f ? this / mag : zero;
            }
        }

        public static float Dot(C_Vector3 a, C_Vector3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        public static C_Vector3 Cross(C_Vector3 a, C_Vector3 b) =>
            new(
                a.y * b.z - a.z * b.y,
                a.z * b.x - a.x * b.z,
                a.x * b.y - a.y * b.x
            );

        public static C_Vector3 Lerp(C_Vector3 a, C_Vector3 b, float t) => LerpUnclamped(a, b, Clamp01(t));

        public static C_Vector3 LerpUnclamped(C_Vector3 a, C_Vector3 b, float t) =>
            new(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t
            );

        public static float Distance(C_Vector3 a, C_Vector3 b) => (a - b).Magnitude;

        public static C_Vector3 MoveTowards(C_Vector3 current, C_Vector3 target, float maxDelta) {
            C_Vector3 delta = target - current;
            float sqrDist = delta.SqrMagnitude;
            if (sqrDist == 0f || (maxDelta >= 0f && sqrDist <= maxDelta * maxDelta))
                return target;

            float dist = (float)Math.Sqrt(sqrDist);
            return current + delta / dist * maxDelta;
        }

        public static C_Vector3 ClampMagnitude(C_Vector3 v, float maxLength) {
            float sqrMag = v.SqrMagnitude;
            float maxSqr = maxLength * maxLength;
            if (sqrMag > maxSqr && sqrMag > 0f) {
                float mag = (float)Math.Sqrt(sqrMag);
                return v / mag * maxLength;
            }
            return v;
        }

        public static C_Vector3 Project(C_Vector3 v, C_Vector3 onNormal) {
            float denom = Dot(onNormal, onNormal);
            if (denom < 1e-8f) return zero;
            return onNormal * (Dot(v, onNormal) / denom);
        }

        public static C_Vector3 Reflect(C_Vector3 inDirection, C_Vector3 inNormal) =>
            inDirection - 2f * Dot(inDirection, inNormal) * inNormal;

        public static float Angle(C_Vector3 from, C_Vector3 to) {
            float denom = (float)Math.Sqrt(from.SqrMagnitude * to.SqrMagnitude);
            if (denom < 1e-8f) return 0f;

            float cos = Dot(from, to) / denom;
            cos = Clamp(cos, -1f, 1f);
            return (float)(Math.Acos(cos) * (180.0 / Math.PI));
        }

        public static bool Approximately(C_Vector3 a, C_Vector3 b, float epsilon = 1e-5f) =>
            (a - b).SqrMagnitude <= epsilon * epsilon;

        public static C_Vector3 operator +(C_Vector3 a, C_Vector3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
        public static C_Vector3 operator -(C_Vector3 a, C_Vector3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
        public static C_Vector3 operator *(C_Vector3 v, float s) => new(v.x * s, v.y * s, v.z * s);
        public static C_Vector3 operator *(float s, C_Vector3 v) => v * s;
        public static C_Vector3 operator /(C_Vector3 v, float s) => new(v.x / s, v.y / s, v.z / s);

        // Keep == exact to match .NET structs; use Approximately for float logic.
        public static bool operator ==(C_Vector3 a, C_Vector3 b) => a.x == b.x && a.y == b.y && a.z == b.z;
        public static bool operator !=(C_Vector3 a, C_Vector3 b) => !(a == b);

        public override readonly bool Equals(object obj) => obj is C_Vector3 v && this == v;
        public override readonly int GetHashCode() => HashCode.Combine(x, y, z);
        public override readonly string ToString() => $"({x}, {y}, {z})";

        private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);
        private static float Clamp(float v, float min, float max) => v < min ? min : (v > max ? max : v);
    }
}
