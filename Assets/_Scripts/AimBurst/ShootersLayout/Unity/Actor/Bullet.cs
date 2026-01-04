using System.Collections.Generic;
using AimBurst.LevelLayout.API;
using AimBurst.LevelLayout.Contracts;
using CocaCopa.Core.Animation;
using CocaCopa.Unity.Numerics;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity.Actor {
    internal sealed class Bullet : MonoBehaviour {
        [SerializeField] private BulletVisuals visuals;

        [Header("Collision")]
        [SerializeField] private float destroyAfterSec;
        [SerializeField] private float hitboxRadius = 0.35f;

        [Header("Movement")]
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float speed;

        private static readonly Collider[] overlapBuffer = new Collider[4];
        private readonly List<ICube> cachedTargets = new List<ICube>();

        private ValueAnimator animator;

        private Vector3 from;
        private ICube target;

        private float initialDistance;

        private void Awake() {
            animator = ValueAnimator.BySpeed(0f, 1f, speed, new MoveEase(moveCurve));
            enabled = false;
        }

        public void Fire(Vector3 from, ICube target) {
            this.from = from;
            this.target = target;
            transform.position = from;
            transform.forward = (target.Position.ToUnity() - from).normalized;

            Vector3 targetPos = target.Position.ToUnity();
            initialDistance = Vector3.Distance(from, targetPos);

            if (initialDistance <= Mathf.Epsilon) {
                enabled = false;
                return;
            }

            animator.ResetAnimator();
            enabled = true;
        }

        private void Update() {
            if (target == null) { enabled = false; return; }
            Move();
            SearchTarget();
        }

        private void Move() {
            Vector3 targetPos = target.Position.ToUnity();
            float t = animator.Evaluate(Time.deltaTime);
            transform.position = Vector3.Lerp(from, targetPos, t);
            if (animator.IsComplete) {
                enabled = false;
            }
        }

        private void SearchTarget() {
            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, hitboxRadius, overlapBuffer);
            for (int i = 0; i < hitCount; i++) {
                var collider = overlapBuffer[i];
                if (collider.transform == transform) { continue; }
                if (collider.TryGetComponent<ICube>(out var cube)) {
                    if (cachedTargets.Contains(cube)) { continue; }
                    cachedTargets.Add(cube);
                    LevelLayoutAPI.ReportHit(cube, target, transform.forward.ToCore());
                    if (ReferenceEquals(cube, target)) {
                        visuals.Impact();
                        Destroy(gameObject, destroyAfterSec);
                        return;
                    }
                }
            }
        }

        private sealed class MoveEase : IEasing {
            private readonly AnimationCurve curve;
            public MoveEase(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }
    }
}
