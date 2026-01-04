using System;
using System.Collections;
using System.Threading.Tasks;
using AimBurst.ShootersLayout.Runtime.Abstractions;
using AimBurst.ShootersLayout.Unity.Actor;
using CocaCopa.Core.Animation;
using CocaCopa.Unity.Extensions;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity.SLots {
    internal sealed class SlotsAnimator : MonoBehaviour, ISlotsAnimator {
        [Header("Merge")]
        [SerializeField] private MergeAnim mergeConfig;

        private ValueAnimator heightAnim;
        private ValueAnimator unifyAnim;
        private ValueAnimator groundAnim;

        private void Awake() {
            heightAnim = ValueAnimator.ByDuration(0f, 1f, mergeConfig.heightDuration, new Ease(mergeConfig.heightCurve));
            unifyAnim = ValueAnimator.ByDuration(0f, 1f, mergeConfig.unifyDuration, new Ease(mergeConfig.unifyCurve));
            groundAnim = ValueAnimator.ByDuration(0f, 1f, mergeConfig.groundDuration, new Ease(mergeConfig.groundCurve));
        }

        public async Task<IShooterCombat> MergeShootersAsync(IShooterCombat[] shootersToMerge) {
            IShooterCombat mergedShooter = null;
            await MergeShootersRoutine(shootersToMerge, (newShooter) => {
                mergedShooter = newShooter;
            }).AsTask(this);
            return mergedShooter;
        }

        private IEnumerator MergeShootersRoutine(IShooterCombat[] shootersToMerge, Action<IShooterCombat> onComplete) {
            Shooter[] shooters = ShortMergeShooters(shootersToMerge);

            yield return Merge_Lift(shooters);
            yield return Merge_Unify(shooters);
            yield return Merge_Ground(shooters[1]);
            onComplete?.Invoke(shooters[1]);
        }

        private IEnumerator Merge_Lift(Shooter[] shooters) {
            heightAnim.ResetAnimator();
            yield return new WaitForSeconds(mergeConfig.liftDelay);

            Vector3[] startPos = new Vector3[shooters.Length];
            Vector3[] heightTargetPos = new Vector3[shooters.Length];

            for (int i = 0; i < shooters.Length; i++) {
                startPos[i] = shooters[i].transform.position;
                heightTargetPos[i] = startPos[i] + Vector3.up * mergeConfig.offset;
            }

            while (!heightAnim.IsComplete) {
                float t = heightAnim.Evaluate(Time.deltaTime);
                for (int i = 0; i < shooters.Length; i++) {
                    shooters[i].transform.position = Vector3.Lerp(startPos[i], heightTargetPos[i], t);
                }
                yield return null;
            }

            for (int i = 0; i < shooters.Length; i++) {
                shooters[i].transform.position = heightTargetPos[i];
            }
        }

        private IEnumerator Merge_Unify(Shooter[] shooters) {
            unifyAnim.ResetAnimator();
            Vector3 leftStart = shooters[0].transform.position;
            Vector3 midPos = shooters[1].transform.position;
            Vector3 rightStart = shooters[2].transform.position;

            while (!unifyAnim.IsComplete) {
                float t = unifyAnim.Evaluate(Time.deltaTime);
                shooters[0].transform.position = Vector3.Lerp(leftStart, midPos, t);
                shooters[2].transform.position = Vector3.Lerp(rightStart, midPos, t);
                yield return null;
            }

            shooters[0].transform.position = midPos;
            shooters[2].transform.position = midPos;

            shooters[0].Kill();
            shooters[2].Kill();
        }

        private IEnumerator Merge_Ground(Shooter middleShooter) {
            groundAnim.ResetAnimator();
            yield return new WaitForSeconds(mergeConfig.groundDelay);

            Vector3 heightPos = middleShooter.transform.position;
            Vector3 groundPos = heightPos - Vector3.up * mergeConfig.offset;

            while (!groundAnim.IsComplete) {
                float t = groundAnim.Evaluate(Time.deltaTime);
                middleShooter.transform.position = Vector3.Lerp(heightPos, groundPos, t);
                yield return null;
            }

            middleShooter.transform.position = groundPos;
        }

        public static Shooter[] ShortMergeShooters(IShooterCombat[] shooters) {
            if (shooters == null) { throw new ArgumentNullException($"<color=white>[{nameof(SlotsAnimator)}]</color> {nameof(shooters)}"); }
            if (shooters.Length != 3) { throw new ArgumentException($"<color=white>[{nameof(SlotsAnimator)}]</color> Expects exactly 3 shooters to merge."); }

            var result = new Shooter[3];
            for (int i = 0; i < 3; i++) {
                if (shooters[i] is not Shooter s) { throw new InvalidCastException($"{nameof(IShooterCombat)} at index {i} is not a {nameof(Shooter)}"); }
                result[i] = s;
            }
            Array.Sort(result, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
            return result;
        }

        private class Ease : IEasing {
            private readonly AnimationCurve curve;
            public Ease(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }

        [Serializable]
        private class MergeAnim {
            public float offset;
            public float liftDelay;
            public AnimationCurve heightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            public float heightDuration;
            public AnimationCurve unifyCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            public float unifyDuration;
            public float groundDelay;
            public AnimationCurve groundCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            public float groundDuration;
        }
    }
}
