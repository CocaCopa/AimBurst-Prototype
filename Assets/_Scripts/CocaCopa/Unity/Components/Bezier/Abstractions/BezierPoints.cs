using UnityEngine;

namespace CocaCopa.Unity.Components.Bezier {
    /// <summary>
    /// Container for the four control points of a cubic Bézier curve.
    /// </summary>
    internal struct BezierPoints {
        /// <summary>
        /// Start point of the curve (t = 0).
        /// </summary>
        public Vector3 A;
        /// <summary>
        /// First control point influencing the curve near the start.
        /// </summary>
        public Vector3 B;
        /// <summary>
        /// Second control point influencing the curve near the end.
        /// </summary>
        public Vector3 C;
        /// <summary>
        /// End point of the curve (t = 1).
        /// </summary>
        public Vector3 D;

        /// <summary>
        /// Initializes a new set of cubic Bézier control points.
        /// </summary>
        /// <param name="A">Start point (t = 0).</param>
        /// <param name="B">First control point.</param>
        /// <param name="C">Second control point.</param>
        /// <param name="D">End point (t = 1).</param>
        public BezierPoints(Vector3 A, Vector3 B, Vector3 C, Vector3 D) {
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = D;
        }
    }
}
