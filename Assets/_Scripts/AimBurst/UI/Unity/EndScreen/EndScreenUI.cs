using System;
using System.Collections;
using System.Threading.Tasks;
using AimBurst.UI.Contracts.EndScreen;
using CocaCopa.Core.Animation;
using CocaCopa.Unity.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace AimBurst.UI.Unity.EndScreen {
    internal sealed class EndScreenUI : MonoBehaviour, IEndScreen {
        [Header("References")]
        [SerializeField] private EndScreenSFX endSfx;
        [SerializeField] private GameObject canvasObj;
        [SerializeField] private Image backgroundImg;

        [Header("Victory")]
        [SerializeField] private GameObject victoryHolder;
        [SerializeField] private GameObject victoryBanner;
        [SerializeField] private GameObject nextLevelBtnObj;
        [SerializeField] private Confetti confetti;


        [Header("Defeat")]
        [SerializeField] private GameObject defeatHolder;
        [SerializeField] private GameObject defeatBanner;
        [SerializeField] private GameObject restartBtnObj;

        [Header("Scale Animation")]
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float scaleDuration;

        [Header("Fade Animation")]
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float fadeDuration;

        [Header("Sequence")]
        [SerializeField] private float sequenceDelay;
        [SerializeField] private float enableConfettiDelay;
        [SerializeField] private float scaleStateDelay;
        [SerializeField] private float scaleBtnDelay;

        private ValueAnimator scaleStateAnim;
        private ValueAnimator scaleBtnAnim;
        private ValueAnimator fadeAnim;

        private GameObject bannerStateObj;
        private GameObject btnStateObj;

        private Button nextLevelBtn;
        private Button restartLevelBtn;
        private bool skipConfetti;

        private readonly TaskCompletionSource<bool> triggerTcs = new TaskCompletionSource<bool>();

        private void Awake() {
            nextLevelBtn = nextLevelBtnObj.GetComponent<Button>();
            restartLevelBtn = restartBtnObj.GetComponent<Button>();
            nextLevelBtn.onClick.AddListener(OnNextLevelPressed);
            restartLevelBtn.onClick.AddListener(OnNextLevelPressed);

            scaleStateAnim = ValueAnimator.ByDuration(0f, 1f, scaleDuration, new Ease(scaleCurve));
            scaleBtnAnim = ValueAnimator.ByDuration(0f, 1f, scaleDuration, new Ease(scaleCurve));
            fadeAnim = ValueAnimator.ByDuration(0f, 1f, fadeDuration, new Ease(fadeCurve));

            SetBackgroundAlpha(0f);
            canvasObj.SetActive(false);

            victoryBanner.transform.localScale = Vector3.zero;
            nextLevelBtnObj.transform.localScale = Vector3.zero;
            confetti.particlesObj.SetActive(false);
            confetti.textureObj.SetActive(false);

            defeatBanner.transform.localScale = Vector3.zero;
            restartBtnObj.transform.localScale = Vector3.zero;
        }

        private void OnNextLevelPressed() {
            triggerTcs?.TrySetResult(true);
        }

        public async Task<bool> Trigger(State state, bool hideNextLvlBtn = false) {
            canvasObj.SetActive(true);
            switch (state) {
                case State.Win: await WinState(hideNextLvlBtn); break;
                case State.Lose: await LoseState(); break;
            }

            return await triggerTcs.Task;
        }

        private async Task WinState(bool hideNextLvlBtn) {
            victoryHolder.SetActive(true);

            float elapsed = 0f;
            while (elapsed < sequenceDelay) {
                elapsed += Time.deltaTime;
                await Task.Yield();
            }

            endSfx.PlayWinAudio();
            skipConfetti = false;
            bannerStateObj = victoryBanner;
            btnStateObj = nextLevelBtnObj;

            await StartSequence(hideNextLvlBtn).AsTask(this);
        }

        private async Task LoseState() {
            defeatHolder.SetActive(true);
            endSfx.PlayLoseAudio();
            skipConfetti = true;
            bannerStateObj = defeatBanner;
            btnStateObj = restartBtnObj;
            await StartSequence(false).AsTask(this);
        }

        private IEnumerator StartSequence(bool hideNextLvlBtn) {
            StartCoroutine(FadeRoutine());
            if (!skipConfetti) {
                yield return new WaitForSeconds(enableConfettiDelay);
                confetti.particlesObj.SetActive(true);
                confetti.textureObj.SetActive(true);
            }
            yield return new WaitForSeconds(scaleStateDelay);
            StartCoroutine(ScaleObjRoutine(bannerStateObj, scaleStateAnim));
            if (!hideNextLvlBtn) {
                yield return new WaitForSeconds(scaleBtnDelay);
                StartCoroutine(ScaleObjRoutine(btnStateObj, scaleBtnAnim));
            }
        }

        private IEnumerator FadeRoutine() {
            fadeAnim.ResetAnimator();

            while (!fadeAnim.IsComplete) {
                float t = fadeAnim.EvaluateUnclamped(Time.deltaTime);
                SetBackgroundAlpha(t);
                yield return null;
            }
        }

        private IEnumerator ScaleObjRoutine(GameObject obj, ValueAnimator anim) {
            anim.ResetAnimator();
            while (!anim.IsComplete) {
                float t = anim.EvaluateUnclamped(Time.deltaTime);
                obj.transform.localScale = Vector3.one * t;
                yield return null;
            }

        }

        private void SetBackgroundAlpha(float alpha) {
            Color imgColor = backgroundImg.color;
            imgColor.a = alpha;
            backgroundImg.color = imgColor;
        }

        [Serializable]
        private struct Confetti {
            public GameObject particlesObj;
            public GameObject textureObj;
        }

        private class Ease : IEasing {
            private readonly AnimationCurve curve;
            public Ease(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }
    }
}
