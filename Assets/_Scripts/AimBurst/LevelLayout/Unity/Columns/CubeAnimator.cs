using System;
using System.Collections;
using CocaCopa.Core.Animation;
using UnityEngine;

namespace AimBurst.LevelLayout.Unity.Columns {
    internal sealed class CubeAnimator : MonoBehaviour {
        [Header("Killed")]
        [SerializeField] private Animation killAnim;
        [SerializeField] private PushbackOpt killPushback;
        [Space(20)]

        [Header("Dodge")]
        [SerializeField] private float dodgeAngle = 20f;
        [SerializeField] private Animation dodgeInAnim;
        [SerializeField] private Animation dodgeOutAnim;
        [SerializeField] private PushbackOpt dodgePushback;

        private Coroutine dodgeRoutine;
        private bool killed;

        private void Awake() {
            killAnim.anim = ValueAnimator.ByDuration(0f, 1f, killAnim.duration, new Ease(killAnim.curve));
            dodgeInAnim.anim = ValueAnimator.ByDuration(0f, 1f, dodgeInAnim.duration, new Ease(dodgeInAnim.curve));
            dodgeOutAnim.anim = ValueAnimator.ByDuration(0f, 1f, dodgeOutAnim.duration, new Ease(dodgeOutAnim.curve));
            killed = false;
        }

        internal void Kill() {
            killed = true;
            StartCoroutine(KillRoutine());
        }

        private IEnumerator KillRoutine() {
            Vector3 initScale = transform.localScale;
            killAnim.anim.ResetAnimator();

            if (killPushback.enable) {
                yield return PushbackRoutine(Vector3.forward, killPushback);
            }

            while (!killAnim.anim.IsComplete) {
                float t = killAnim.anim.Evaluate(Time.deltaTime);
                transform.localScale = initScale * (1 - t);
                yield return null;
            }
        }

        public void Dodge(Vector3 toward, Vector3 towardPerpendicular) {
            if (killed) { return; }
            if (dodgeRoutine != null) { StopCoroutine(dodgeRoutine); dodgeRoutine = null; }
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
            dodgeRoutine ??= StartCoroutine(DodgeRoutine(toward, towardPerpendicular));
        }

        private IEnumerator DodgeRoutine(Vector3 toward, Vector3 perp) {
            Quaternion startRot = transform.rotation;
            Quaternion dodgeRot = startRot * Quaternion.AngleAxis(dodgeAngle, toward);
            bool pushbackCompleted = false;

            if (dodgePushback.enable) {
                StartCoroutine(PushbackRoutine(-perp, dodgePushback, () => pushbackCompleted = true));
            }

            dodgeInAnim.anim.ResetAnimator();
            while (!dodgeInAnim.anim.IsComplete) {
                float t = dodgeInAnim.anim.Evaluate(Time.deltaTime);
                transform.rotation = Quaternion.Slerp(startRot, dodgeRot, t);
                yield return null;
            }

            dodgeOutAnim.anim.ResetAnimator();
            while (!dodgeOutAnim.anim.IsComplete) {
                float t = dodgeOutAnim.anim.Evaluate(Time.deltaTime);
                transform.rotation = Quaternion.Slerp(dodgeRot, startRot, t);
                yield return null;
            }

            transform.rotation = startRot;

            while (!pushbackCompleted) { yield return null; }
            dodgeRoutine = null;
        }


        private IEnumerator PushbackRoutine(Vector3 dir, PushbackOpt opt, Action onComplete) {
            yield return PushbackRoutine(dir, opt);
            onComplete?.Invoke();
        }
        private IEnumerator PushbackRoutine(Vector3 dir, PushbackOpt opt) {
            var pushbackAnim = ValueAnimator.ByDuration(0f, 1f, opt.outDuration, new Ease(opt.outCurve));
            var comebackAnim = ValueAnimator.ByDuration(0f, 1f, opt.inDuration, new Ease(opt.inCurve));
            transform.forward = dir;
            Vector3 initPos = transform.localPosition;
            while (!pushbackAnim.IsComplete) {
                float t = pushbackAnim.Evaluate(Time.deltaTime);
                transform.localPosition = initPos + opt.offset * t * dir;
                yield return null;
            }

            while (!comebackAnim.IsComplete) {
                float t = comebackAnim.Evaluate(Time.deltaTime);
                transform.localPosition = initPos + opt.offset * (1 - t) * dir;
                yield return null;
            }
        }

        private sealed class Ease : IEasing {
            private readonly AnimationCurve curve;
            public float Evaluate(float t) => curve.Evaluate(t);
            public Ease(AnimationCurve curve) => this.curve = curve;
        }

        [Serializable]
        private class Animation {
            public AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            [Min(0f)] public float duration;
            public ValueAnimator anim;
        }

        [Serializable]
        private class PushbackOpt {
            public bool enable;
            public float offset;
            public AnimationCurve outCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            public float outDuration;
            public AnimationCurve inCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            public float inDuration;
        }
    }
}
