using UnityEngine;

namespace CocaCopa.Unity.Components.Bezier {
    /// <summary>
    /// Approximates the arc length of a cubic Bézier curve by sampling it at fixed intervals,
    /// and provides a mapping from traveled distance to curve parameter t.
    /// </summary>
    internal class BezierArcLengthSolver {
        /// <summary>
        /// Number of uniform samples used to approximate the curve length.
        /// Higher values improve accuracy but cost more CPU.
        /// </summary>
        private const int ArcSamples = 50;

        private readonly float[] arcLengths;
        private readonly float totalLength;

        /// <summary>
        /// Gets the total approximate length of the Bézier curve,
        /// calculated by sampling the curve at fixed intervals.
        /// </summary>
        public float TotalLength => totalLength;

        /// <summary>
        /// Builds an arc-length lookup table for the given cubic Bézier curve.
        /// </summary>
        /// <param name="points">The curve control points (A, B, C, D).</param>
        internal BezierArcLengthSolver(BezierPoints points) {
            arcLengths = new float[ArcSamples + 1];
            arcLengths[0] = 0f;

            Vector3 prev = points.A;
            float length = 0f;

            for (int i = 1; i <= ArcSamples; i++) {
                float t = i / (float)ArcSamples;
                Vector3 p = BezierPathMath.CubicBezier(points.A, points.B, points.C, points.D, t);
                length += Vector3.Distance(prev, p);
                arcLengths[i] = length;
                prev = p;
            }

            totalLength = length;
        }

        /// <summary>
        /// Converts a traveled distance along the curve (in world units) to the corresponding
        /// normalized curve parameter t in the range [0, 1], using the precomputed lookup table.
        /// </summary>
        /// <param name="distance">Distance traveled along the curve in world units.</param>
        /// <returns>
        /// The approximate curve parameter t in [0, 1] such that the arc length from t=0 to that t
        /// is approximately equal to <paramref name="distance"/>.
        /// </returns>
        internal float DistanceToT(float distance) {
            if (distance <= 0f) return 0f;
            if (distance >= totalLength) return 1f;

            for (int i = 1; i < arcLengths.Length; i++) {
                if (arcLengths[i] >= distance) {
                    float prevLen = arcLengths[i - 1];
                    float segLen = arcLengths[i] - prevLen;
                    float segT = (distance - prevLen) / segLen;
                    return (i - 1 + segT) / ArcSamples;
                }
            }

            return 1f;
        }
    }
}
