using System;
using CocaCopa.Core.Animation;
using CocaCopa.Unity.Components.Panel;
using UnityEngine;

namespace CocaCopa.Unity.Components {
    [RequireComponent(typeof(RectTransform))]
    public class PanelAnimator : MonoBehaviour {
        [Header("General")]
        [SerializeField] private bool startHidden = true;
        [SerializeField] private AnimOptions animOptions;
        [SerializeField] private HideOffsets hideOffsets;

        [Header("Animation")]
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float moveSpeed = 1.5f;

        private RectTransform panel;
        private RectPositions hiddenPositions;
        private IEasing easing;
        private ValueAnimator valueAnimator;
        private Vector3 from;
        private Vector3 to;

        public bool IsAnimating => valueAnimator != null && !valueAnimator.IsComplete;
        public void OverrideAnimOptions(AnimOptions opt) => animOptions = opt;

        private void Awake() {
            easing = new CurveEasing(moveCurve);
            valueAnimator = ValueAnimator.BySpeed(0f, 1f, moveSpeed, easing);
            panel = GetComponent<RectTransform>();
        }

        private void Start() {
            Canvas.ForceUpdateCanvases(); // Makes sure that width/height are calced correctly.
            CalcPositions();
            if (startHidden) {
                Hide();
                valueAnimator.SetProgress(1f);
            }
        }

        private void CalcPositions() {
            var visible = panel.anchoredPosition;

            var panelHeight = panel.rect.height;
            var panelWidth = panel.rect.width;

            var hiddenTop = visible + Vector2.up * (panelHeight + hideOffsets.top);
            var hiddenBottom = visible + Vector2.down * (panelHeight + hideOffsets.bottom);
            var hiddenLeft = visible + Vector2.left * (panelWidth + hideOffsets.left);
            var hiddenRight = visible + Vector2.right * (panelWidth + hideOffsets.right);

            hiddenPositions = new RectPositions(visible, hiddenTop, hiddenBottom, hiddenLeft, hiddenRight);
        }

        private void Update() {
            AnimatePanel();
            DisableWhenComplete();
        }

        private void AnimatePanel() {
            float t = valueAnimator.EvaluateUnclamped(Time.unscaledDeltaTime);
            panel.anchoredPosition = Vector2.LerpUnclamped(from, to, t);
        }

        private void DisableWhenComplete() {
            if (valueAnimator.IsComplete) {
                enabled = false;
            }
        }

        public void Show() {
            valueAnimator.ResetAnimator();
            from = GetHiddenPosition(hiddenPositions, isAppearing: true);
            to = hiddenPositions.visible;
            enabled = true;
        }

        public void Hide() {
            valueAnimator.ResetAnimator();
            from = hiddenPositions.visible;
            to = GetHiddenPosition(hiddenPositions, isAppearing: false);
            enabled = true;
        }

        private Vector2 GetHiddenPosition(RectPositions positions, bool isAppearing) {
            if (isAppearing) {
                return animOptions.appear switch {
                    UIAppear.Left => positions.hiddenLeft,
                    UIAppear.Right => positions.hiddenRight,
                    UIAppear.Top => positions.hiddenTop,
                    UIAppear.Bottom => positions.hiddenBottom,
                    _ => positions.hiddenLeft
                };
            }
            else {
                return animOptions.disappear switch {
                    UIDisappear.Left => positions.hiddenLeft,
                    UIDisappear.Right => positions.hiddenRight,
                    UIDisappear.Top => positions.hiddenTop,
                    UIDisappear.Bottom => positions.hiddenBottom,
                    _ => positions.hiddenLeft
                };
            }
        }

        internal sealed class CurveEasing : IEasing {
            private readonly AnimationCurve curve;
            public CurveEasing(AnimationCurve curve) => this.curve = curve;
            public float Evaluate(float t) => curve.Evaluate(t);
        }

        [Serializable]
        private struct HideOffsets {
            public float top;
            public float bottom;
            public float left;
            public float right;

            public HideOffsets(float top, float bottom, float left, float right) {
                this.top = top;
                this.bottom = bottom;
                this.left = left;
                this.right = right;
            }
        }
    }
}
