using UnityEngine;

namespace CocaCopa.Unity.Components.Bezier {

    /// <summary>
    /// Math helpers for cubic Bézier evaluation and control point construction.
    /// Intended for internal use by the Bézier component system.
    /// </summary>
    internal class BezierPathMath {

        /// <summary>
        /// Evaluates a cubic Bézier curve at parameter <paramref name="t"/>.
        /// </summary>
        /// <param name="A">Start point (t = 0).</param>
        /// <param name="B">First control point.</param>
        /// <param name="C">Second control point.</param>
        /// <param name="D">End point (t = 1).</param>
        /// <param name="t">Curve parameter, typically in the [0, 1] range.</param>
        /// <returns>The point on the cubic Bézier curve at <paramref name="t"/>.</returns>
        internal static Vector3 CubicBezier(Vector3 A, Vector3 B, Vector3 C, Vector3 D, float t) {
            float u = 1f - t;
            return u * u * u * A
            + 3f * u * u * t * B
            + 3f * u * t * t * C
            + t * t * t * D;
        }

        /// <summary>
        /// Builds a control point between <paramref name="start"/> and <paramref name="end"/> by placing it
        /// at a normalized position along the segment and offsetting it perpendicular to the segment direction.
        /// </summary>
        /// <param name="start">Segment start position.</param>
        /// <param name="end">Segment end position.</param>
        /// <param name="along01">Normalized position along the segment [0, 1] where the control point base is placed.</param>
        /// <param name="offset">Signed perpendicular offset distance applied from the base point.</param>
        /// <param name="upHint">Preferred "up" vector used to define the perpendicular direction via a cross product. If nearly parallel to the segment direction, a fallback axis is used.</param>
        /// <returns>The computed control point position.</returns>
        internal static Vector3 BuildControlPoint(Vector3 start, Vector3 end, float along01, float offset, Vector3 upHint) {
            Vector3 dir = end - start;
            if (dir.sqrMagnitude < 0.0001f) return start;

            dir.Normalize();
            Vector3 perp = Vector3.Cross(dir, upHint);
            if (perp.sqrMagnitude < 0.00001f)
                perp = Vector3.Cross(dir, Vector3.right);

            perp.Normalize();
            return Vector3.Lerp(start, end, along01) + perp * offset;
        }
    }
}
