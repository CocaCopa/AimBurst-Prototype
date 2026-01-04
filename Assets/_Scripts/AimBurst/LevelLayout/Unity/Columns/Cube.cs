using System;
using AimBurst.Core.Contracts;
using AimBurst.LevelLayout.Contracts;
using CocaCopa.Core.Animation;
using CocaCopa.Primitives;
using CocaCopa.Unity.Numerics;
using UnityEngine;

namespace AimBurst.LevelLayout.Unity.Columns {
    [RequireComponent(typeof(Collider))]
    internal sealed class Cube : MonoBehaviour, ICube {
        [SerializeField] private CubeColor color;
        [Space(10)]
        [SerializeField] private LayerMask environmentMask;
        [SerializeField] private float ceilingCheckPadding;
        [SerializeField] private AnimationCurve resurfaceCurve;
        [SerializeField] private float resurfaceDuration;

        private Collider cubeCollider;
        private ValueAnimator ceilAnim;

        private CubeAnimator animator;
        public CubeColor Color => color;
        public C_Vector3 Position => transform.position.ToCore();
        public int InstanceID => GetInstanceID();

        private Vector3 originalPos;

        private void Awake() {
            FetchComponents();
            ceilAnim = ValueAnimator.ByDuration(0f, 1, resurfaceDuration, new Ease(resurfaceCurve));
        }

        private void Start() {
            originalPos = transform.position;
            enabled = CeilingHit();
        }

        private void FetchComponents() {
            cubeCollider = GetComponent<Collider>();
            animator = GetComponentInChildren<CubeAnimator>();
            if (animator == null) { throw new NullReferenceException($"<color=white>[{nameof(Cube)}]</color> {nameof(CubeAnimator)}"); }
        }

        private void Update() {
            AvoidCeiling();
        }

        private void AvoidCeiling() {
            if (CeilingHit()) {
                Vector3 pos = transform.position;
                pos.y = 0f;
                transform.position = pos;
            }
            else {
                float t = ceilAnim.EvaluateUnclamped(Time.deltaTime);
                Vector3 startPos = transform.position; startPos.y = 0f;
                Vector3 endPos = transform.position; endPos.y = originalPos.y;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                if (ceilAnim.IsComplete) { enabled = false; }
            }
        }

        private bool CeilingHit() {
            var bounds = cubeCollider.bounds;
            Vector3 origin = transform.position; origin.y = 0f;
            float distance = bounds.extents.y + ceilingCheckPadding;

            return Physics.Raycast(origin, Vector3.up, distance, environmentMask, QueryTriggerInteraction.Ignore);
        }

        public void Kill() {
            cubeCollider.enabled = false;
            transform.SetParent(null);
            animator.Kill();
        }

        public void DodgeBullet(C_Vector3 dodgeToward) {
            // cubeCollider.enabled = false;
            Vector3 reflected = Vector3.Reflect(dodgeToward.ToUnity(), Vector3.right).normalized;
            Vector3 cross = Vector3.Cross(Vector3.up, reflected);
            animator.Dodge(cross, reflected);
        }

        private class Ease : IEasing {
            private readonly AnimationCurve curve;
            public Ease(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }
    }
}
