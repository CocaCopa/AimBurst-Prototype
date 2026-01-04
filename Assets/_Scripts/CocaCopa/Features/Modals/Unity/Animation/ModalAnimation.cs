using System.Collections;
using CocaCopa.Modal.Contracts;
using CocaCopa.Modal.SPI;
using CocaCopa.Unity.Components;
using CocaCopa.Unity.Components.Panel;
using UnityEngine;

namespace CocaCopa.Modal.Unity.Animation {
    [RequireComponent(typeof(CanvasGroup))]
    internal class ModalAnimation : MonoBehaviour, IModalAnimator {
        [Header("References")]
        [SerializeField] private PanelAnimator inputAnimator;
        [SerializeField] private PanelAnimator vkAnimator;

        [Header("General")]
        [SerializeField] private AnimateFirst animateFirst;
        [SerializeField] private float delayTime;

        private PanelAnimator primaryObj;
        private PanelAnimator secondaryObj;

        public bool IsVisible { get; private set; }

        private void Awake() {
            var cg = GetComponent<CanvasGroup>();
            cg.alpha = 1f;
            SetAnimOrder(animateFirst);
        }

        private void Start() {
            Canvas.ForceUpdateCanvases();
        }

        private void SetAnimOrder(AnimateFirst first) {
            if (first == AnimateFirst.Input) {
                primaryObj = inputAnimator;
                secondaryObj = vkAnimator;
            }
            else {
                primaryObj = vkAnimator;
                secondaryObj = inputAnimator;
            }
        }

        public void PlayShow(ModalAnimOptions input, ModalAnimOptions vk) {
            inputAnimator.OverrideAnimOptions(MapOptions(input));
            vkAnimator.OverrideAnimOptions(MapOptions(vk));
            PlayShow();
        }

        public void PlayHide(ModalAnimOptions input, ModalAnimOptions vk) {
            inputAnimator.OverrideAnimOptions(MapOptions(input));
            vkAnimator.OverrideAnimOptions(MapOptions(vk));
            PlayHide();
        }

        public void PlayShow() => StartCoroutine(TickSequence(reverse: false));
        public void PlayHide() => StartCoroutine(TickSequence(reverse: true));

        private IEnumerator TickSequence(bool reverse) {
            IsVisible = reverse;
            if (reverse) { primaryObj.Hide(); }
            else { primaryObj.Show(); }

            yield return new WaitForSecondsRealtime(delayTime);

            if (reverse) { secondaryObj.Hide(); }
            else { secondaryObj.Show(); }

            while (primaryObj.IsAnimating || secondaryObj.IsAnimating) {
                yield return null;
            }
            IsVisible = !reverse;
        }

        private AnimOptions MapOptions(ModalAnimOptions opt) {
            UIAppear appear = opt.appear switch {
                Appear.Top => UIAppear.Top,
                Appear.Bottom => UIAppear.Bottom,
                Appear.Left => UIAppear.Left,
                Appear.Right => UIAppear.Right,
                _ => throw new System.ArgumentOutOfRangeException()
            };
            UIDisappear disappear = opt.disappear switch {
                Disappear.Top => UIDisappear.Top,
                Disappear.Bottom => UIDisappear.Bottom,
                Disappear.Left => UIDisappear.Left,
                Disappear.Right => UIDisappear.Right,
                _ => throw new System.ArgumentOutOfRangeException()
            };
            return new AnimOptions(appear, disappear);
        }

        private enum AnimateFirst {
            Input,
            VirtualKeyBoard
        }
    }
}
