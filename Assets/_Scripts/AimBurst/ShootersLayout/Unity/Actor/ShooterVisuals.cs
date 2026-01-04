using System.Collections;
using AimBurst.ShootersLayout.Runtime.Abstractions;
using Chris.Outline.Unity;
using CocaCopa.Core.Animation;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity.Actor {
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Outline))]
    internal sealed class ShooterVisuals : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Shooter shooter;

        [Header("Combat Friend")]
        [SerializeField] private LineRenderer mainLr;
        [SerializeField] private LineRenderer outlineLr;
        [SerializeField] private float connectionWidth;
        [SerializeField, Range(0f, 1f)] private float connectionOutlineWidth;

        [Header("Particles")]
        [SerializeField] private GameObject[] footstepsVfx;
        [SerializeField] private ParticleSystem muzzleVfx;

        [Header("Hidden")]
        [SerializeField] private Material hiddenMaterial;
        [SerializeField] private Color hiddenOutlineColor;
        [SerializeField] private float hiddenOutlineWidth;

        [Header("Slot")]
        [SerializeField] private Color slotOutlineColor = Color.white;
        [SerializeField] private float slotOutlineWidth = 0.5f;

        [Header("Highlight")]
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private AnimationCurve highlightCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float highlightDuration;

        [Header("Recoil")]
        [SerializeField] private AnimationCurve recoilCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float recoilDuration;
        [SerializeField] private float offset;

        [Header("Compress")]
        [SerializeField] private Vector3 compressScale;
        [SerializeField] private AnimationCurve compressCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float compressDuration;

        private ValueAnimator highlightAnim;
        private ValueAnimator compressAnim;
        private ValueAnimator recoilAnim;

        private MeshRenderer meshRenderer;
        private Material defaultMaterial;
        private Color defaultMaterialColor;
        private Outline outline;

        private Color defaultOutlineColor;
        private float defaultOutlineWidth;

        private Coroutine highlightRoutine;
        private Coroutine compressRoutine;

        private Vector3 defaultScale;

        private void Awake() {
            shooter.OnPositioningChanged += Shooter_OnPositioningChanged;
            highlightAnim = ValueAnimator.ByDuration(0f, 1f, highlightDuration, new Ease(highlightCurve));
            compressAnim = ValueAnimator.ByDuration(0f, 1f, compressDuration, new Ease(compressCurve));
            recoilAnim = ValueAnimator.ByDuration(0f, 1f, recoilDuration / 2f, new Ease(recoilCurve));
            meshRenderer = GetComponent<MeshRenderer>();
            outline = GetComponent<Outline>();
            canvas.worldCamera = Camera.main;
            defaultMaterial = meshRenderer.material;
            defaultMaterialColor = defaultMaterial.color;
            defaultOutlineColor = outline.OutlineColor;
            defaultOutlineWidth = outline.OutlineWidth;
            mainLr.gameObject.SetActive(false);
            outlineLr.gameObject.SetActive(false);
        }

        private void Start() {
            defaultScale = transform.localScale;
        }

        private void Shooter_OnPositioningChanged() {
            if (shooter.IsHidden) { return; }

            if (shooter.Positioning == ShooterPositioning.Front) {
                SetBlackOutline();
            }
            else { SetColoredOutline(); }
        }

        internal void Shoot() {
            muzzleVfx.Play();
        }

        internal void EnableFootsteps(bool enable) {
            for (int i = 0; i < footstepsVfx.Length; i++) {
                footstepsVfx[i].SetActive(enable);
            }
        }

        internal void EnableFriendConnection(Color connectionColor) {
            mainLr.gameObject.SetActive(true);
            outlineLr.gameObject.SetActive(true);

            mainLr.positionCount = 2;
            mainLr.useWorldSpace = true;
            mainLr.startWidth = connectionWidth;
            mainLr.endWidth = connectionWidth;

            outlineLr.positionCount = 2;
            outlineLr.useWorldSpace = true;
            outlineLr.startWidth = mainLr.startWidth + mainLr.startWidth * connectionOutlineWidth;
            outlineLr.endWidth = mainLr.endWidth + mainLr.endWidth * connectionOutlineWidth;
            mainLr.material.color = connectionColor;
        }

        internal void DisableFriendConnection() {
            mainLr.gameObject.SetActive(false);
            outlineLr.gameObject.SetActive(false);
        }

        internal void FriendConnectionPosition(Vector3 startPos, Vector3 endPos) {
            mainLr.SetPosition(0, startPos);
            mainLr.SetPosition(1, endPos);

            Vector3 depthOffset = Camera.main.transform.forward * 0.01f;

            outlineLr.SetPosition(0, startPos + depthOffset);
            outlineLr.SetPosition(1, endPos + depthOffset);
        }

        internal void Highlight() {
            if (highlightRoutine != null) { StopCoroutine(highlightRoutine); }
            highlightRoutine = StartCoroutine(HighlightRoutine());
        }
        internal void Lock() {
            meshRenderer.material = hiddenMaterial;
            SetHiddenOutline();
        }
        internal void Unlock() {
            meshRenderer.material = defaultMaterial;
            SetBlackOutline();
        }

        internal IEnumerator HighlightRoutine() {
            highlightAnim.ResetAnimator();
            Color originalColor = defaultMaterialColor;
            while (!highlightAnim.IsComplete) {
                float t = highlightAnim.Evaluate(Time.deltaTime);
                defaultMaterial.color = Color.Lerp(highlightColor, originalColor, t);
                yield return null;
            }
        }

        private void SetColoredOutline() {
            outline.OutlineColor = slotOutlineColor;
            outline.OutlineWidth = slotOutlineWidth;
        }

        private void SetBlackOutline() {
            outline.OutlineColor = defaultOutlineColor;
            outline.OutlineWidth = defaultOutlineWidth;
        }

        private void SetHiddenOutline() {
            outline.OutlineColor = hiddenOutlineColor;
            outline.OutlineWidth = hiddenOutlineWidth;
        }

        internal void Disable() {
            meshRenderer.enabled = false;
            canvas.enabled = false;
        }

        internal void RecoilEffect() {
            StartCoroutine(RecoilRoutine());
        }

        private IEnumerator RecoilRoutine() {
            Vector3 initPos = Vector3.zero;
            Vector3 recoilPos = transform.forward * -offset;
            recoilAnim.ResetAnimator();
            while (!recoilAnim.IsComplete) {
                float t = recoilAnim.Evaluate(Time.deltaTime);
                transform.localPosition = Vector3.Lerp(initPos, recoilPos, t);
                yield return null;
            }
            recoilAnim.ResetAnimator();
            while (!recoilAnim.IsComplete) {
                float t = recoilAnim.Evaluate(Time.deltaTime);
                transform.localPosition = Vector3.Lerp(recoilPos, initPos, t);
                yield return null;
            }
        }

        internal void Compress() {
            if (compressRoutine != null) { StopCoroutine(compressRoutine); compressRoutine = null; }
            compressRoutine ??= StartCoroutine(CompressRoutine());
        }

        private IEnumerator CompressRoutine() {
            compressAnim.ResetAnimator();
            Vector3 currScale = defaultScale;
            Vector3 targetScale = currScale + compressScale;
            while (!compressAnim.IsComplete) {
                float t = compressAnim.Evaluate(Time.deltaTime);
                transform.localScale = Vector3.Lerp(currScale, targetScale, t);
                yield return null;
            }
            compressAnim.ResetAnimator();
            while (!compressAnim.IsComplete) {
                float t = compressAnim.Evaluate(Time.deltaTime);
                transform.localScale = Vector3.Lerp(targetScale, currScale, t);
                yield return null;
            }
            compressRoutine = null;
        }

        private class Ease : IEasing {
            private readonly AnimationCurve curve;
            public Ease(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }
    }
}
