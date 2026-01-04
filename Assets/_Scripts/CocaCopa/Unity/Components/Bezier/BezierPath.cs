using System;
using CocaCopa.Core;
using CocaCopa.Unity.Components.Bezier;
using UnityEngine;

namespace CocaCopa.Unity.Components {
    public class BezierPath : MonoBehaviour {
        // Reference Points
        [Tooltip("Start transform of the bezier path. The curve originates from this position.")]
        [SerializeField] private Transform start;
        [Tooltip("End transform of the bezier path. The curve terminates at this position.")]
        [SerializeField] private Transform end;

        // Movement
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Vector3 followOffset;
        [Tooltip("Which delta time source to use for motion updates\n\nNormal = Time.deltaTime\nUnscaled = Time.unscaledDeltaTime\nFixed = Time.fixedDeltaTime.")]
        [SerializeField] private DeltaTime deltaTime = DeltaTime.Normal;
        [Tooltip("Determines how movement along the curve is evaluated: fixed duration (normalized time) or constant world-space speed.")]
        [SerializeField] private MovementMode movementMode = MovementMode.SpeedBased;
        [Tooltip("Time in seconds required to traverse the entire curve when using DurationBased movement.")]
        [SerializeField, Min(0.01f)] private float duration = 2f;
        [Tooltip("Movement speed in world units per second when using SpeedBased movement.")]
        [SerializeField, Min(0.01f)] private float speed = 3f;
        [SerializeField] private AnimationMode animationMode = AnimationMode.AutoPlay;
        [Tooltip("Loop behavior once the end is reached.")]
        [SerializeField] private LoopMode loopMode = LoopMode.Repeat;
        [SerializeField] private bool lookAtPath = true;

        // Control Point Placement
        [Tooltip("Reference direction used to determine the perpendicular plane for control point offsets.")]
        [SerializeField] private Vector3 upHint = Vector3.up;
        [Tooltip("Normalized position (0-1) along the line from start to end where the first control point is placed.")]
        [Range(0f, 1f)]
        [SerializeField] private float control1Along = 0.2f;
        [Tooltip("Perpendicular offset applied to the first control point relative to the start-end direction.")]
        [SerializeField] private float control1Offset = 2f;
        [Tooltip("Normalized position (0-1) along the line from start to end where the second control point is placed.")]
        [Range(0f, 1f)]
        [SerializeField] private float control2Along = 0.8f;
        [Tooltip("Perpendicular offset applied to the second control point relative to the start-end direction.")]
        [SerializeField] private float control2Offset = 2f;

        public enum MovementMode { DurationBased, SpeedBased }
        private enum DeltaTime { Normal, Unscaled, Fixed }
        private enum LoopMode { None, Repeat, PingPong }
        private enum AnimationMode { AutoPlay, Script }

        private BezierArcLengthSolver arcSolver;
        private BezierPoints points;

        internal BezierArcLengthSolver ArcSolver => arcSolver;
        internal Transform Start => start;
        internal Transform End => end;

        private float elapsed;
        private float traveledDistance;
        private int direction = +1;

        public void SetTarget(Transform target) => targetTransform = target;
        public void Play() { enabled = targetTransform != null; }
        public void Stop() { enabled = false; }
        public void Reset() { elapsed = 0f; traveledDistance = 0f; }
        public bool IsActive => enabled;

        private void Awake() {
            arcSolver = BuildArcSolver(out points);
            enabled = targetTransform != null && animationMode == AnimationMode.AutoPlay;
        }

        private void Update() {
            if (deltaTime == DeltaTime.Fixed) { return; }
            float dt = deltaTime == DeltaTime.Normal ? Time.deltaTime : Time.unscaledDeltaTime;
            MoveTransform(dt);
            LookAtPath();
        }

        private void FixedUpdate() {
            if (deltaTime != DeltaTime.Fixed) { return; }
            MoveTransform(Time.fixedDeltaTime);
        }

        private void MoveTransform(float deltaTime) {
            if (!start || !end || !targetTransform) { return; }

            if (RebuildSolver()) { arcSolver = BuildArcSolver(out points); }

            float time = movementMode == MovementMode.DurationBased
            ? DB_Time(deltaTime) : SB_Time(deltaTime);

            Vector3 newPos = BezierPathMath.CubicBezier(points.A, points.B, points.C, points.D, time);
            newPos += followOffset;
            targetTransform.position = newPos;

        }

        /// <summary>
        /// Duration Based
        /// </summary>
        private float DB_Time(float deltaTime) {
            float deltaT = deltaTime / Mathf.Max(0.0001f, duration);
            switch (loopMode) {
                case LoopMode.None:
                    elapsed += deltaTime;
                    if (elapsed >= duration) {
                        elapsed = duration;
                        enabled = false;
                        return 1f;
                    }
                    return Mathf.Clamp01(elapsed / duration);

                case LoopMode.Repeat:
                    elapsed += deltaTime;
                    float t = elapsed / duration;
                    return Mathf.Repeat(t, 1f);

                case LoopMode.PingPong:
                    float normalized = elapsed / duration;
                    normalized += deltaT * direction;
                    normalized = MathUtils.PingPong01(normalized, ref direction);
                    elapsed = normalized * duration;
                    return normalized;

                default: return 0f;
            }
        }

        /// <summary>
        /// Speed Based
        /// </summary>
        private float SB_Time(float deltaTime) {
            float maxLen = arcSolver.TotalLength;
            switch (loopMode) {
                case LoopMode.None:
                    traveledDistance += speed * deltaTime;
                    traveledDistance = Mathf.Min(traveledDistance, maxLen);
                    if (traveledDistance >= maxLen) {
                        traveledDistance = maxLen;
                        enabled = false;
                        return 1f;
                    }
                    return arcSolver.DistanceToT(traveledDistance);

                case LoopMode.Repeat:
                    traveledDistance += speed * deltaTime;
                    if (maxLen > 0f) traveledDistance %= maxLen;
                    return arcSolver.DistanceToT(traveledDistance);

                case LoopMode.PingPong:
                    traveledDistance += speed * deltaTime * direction;
                    traveledDistance = MathUtils.PingPong(traveledDistance, 0f, maxLen, ref direction);
                    return arcSolver.DistanceToT(traveledDistance);

                default: return 0f;
            }
        }

        private Vector3 prevTargetPos;

        private void LookAtPath() {
            if (!lookAtPath) { return; }
            Vector3 delta = targetTransform.position - prevTargetPos;
            if (delta.sqrMagnitude < 0.000001f) { return; }
            targetTransform.forward = delta;
            prevTargetPos = targetTransform.position;
        }

        internal BezierPoints GetBezierPoints() {
            return new BezierPoints(
                A: start.position,
                B: BezierPathMath.BuildControlPoint(start.position, end.position, control1Along, control1Offset, upHint),
                C: BezierPathMath.BuildControlPoint(start.position, end.position, control2Along, control2Offset, upHint),
                D: end.position
            );
        }

        private BezierArcLengthSolver BuildArcSolver(out BezierPoints bezierPoints) {
            bezierPoints = GetBezierPoints();
            return new BezierArcLengthSolver(bezierPoints);
        }

        private float control1AlongPrev;
        private float control1OffsetPrev;
        private float control2AlongPrev;
        private float control2OffsetPrev;
        private Vector3 upHintPrev;
        private Vector3 startPosPrev;
        private Vector3 endPosPrev;

        private bool RebuildSolver() {
            bool changed =
                startPosPrev != start.position ||
                endPosPrev != end.position ||
                control1AlongPrev != control1Along ||
                control1OffsetPrev != control1Offset ||
                control2AlongPrev != control2Along ||
                control2OffsetPrev != control2Offset ||
                upHintPrev != upHint;

            if (!changed) return false;

            startPosPrev = start.position;
            endPosPrev = end.position;
            control1AlongPrev = control1Along;
            control1OffsetPrev = control1Offset;
            control2AlongPrev = control2Along;
            control2OffsetPrev = control2Offset;
            upHintPrev = upHint;

            return true;
        }
    }
}
