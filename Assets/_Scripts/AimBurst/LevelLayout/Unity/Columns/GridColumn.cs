using System.Collections;
using CocaCopa.Core.Animation;
using UnityEngine;

namespace AimBurst.LevelLayout.Unity.Columns {
    internal sealed class GridColumn : MonoBehaviour {
        private float moveDelay;
        private float moveOffset;
        private ValueAnimator moveAnim;
        private int maxAvailableCubes;
        private Vector3 lastTargetPos;
        private Coroutine moveRoutine;

        internal void Init(float moveDelay, AnimationCurve moveCurve, float moveDuration, float moveOffset) {
            this.moveDelay = moveDelay;
            this.moveOffset = moveOffset;
            moveAnim = ValueAnimator.ByDuration(0f, 1f, moveDuration, new Ease(moveCurve));
            lastTargetPos = transform.position;
        }

        internal void SetMaxAvailable(int maxCubes) => maxAvailableCubes = maxCubes;

        public void MoveDown() {
            if (moveRoutine != null) { StopCoroutine(moveRoutine); moveDelay = 0f; }
            moveRoutine = StartCoroutine(MoveRoutine());
        }

        private IEnumerator MoveRoutine() {
            var initPos = transform.position;
            var targetPos = lastTargetPos + (moveOffset * Vector3.forward);
            lastTargetPos = targetPos;
            moveAnim.ResetAnimator();

            yield return new WaitForSeconds(moveDelay);
            while (!moveAnim.IsComplete) {
                float t = moveAnim.EvaluateUnclamped(Time.deltaTime);
                transform.position = Vector3.LerpUnclamped(initPos, targetPos, t);
                yield return null;
            }
            moveRoutine = null;
        }

        private class Ease : IEasing {
            private readonly AnimationCurve curve;
            public Ease(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }
    }
}
