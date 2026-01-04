using System;
using System.Threading;
using System.Threading.Tasks;
using CocaCopa.Core;
using CocaCopa.Modal.Contracts;
using CocaCopa.Modal.Runtime.Domain;
using CocaCopa.Modal.Runtime.Internal;
using CocaCopa.Modal.SPI;

namespace CocaCopa.Modal.Runtime {
    internal class ModalFlow : IModalService {
        private TaskCompletionSource<ModalResult> tcs;
        private CancellationTokenRegistration ctorCtr;
        private CancellationTokenRegistration showCtr;

        private readonly ConfirmOptions confirmOpt;
        private readonly IModalView modalView;
        private readonly IVirtualKeyboard vk;
        private readonly IModalAnimator modalAnimator;
        private readonly VKStringConstructor strCtor;
        private readonly VirtualCaret vCaret;
        private readonly float caretOnDuration;
        private readonly float caretOffDuration;

        private string inputValue;
        private string normalTxt;
        private string colorizedTxt;

        private CaretState caretState;
        private float caretTimer;
        private bool caretIsOn;

        internal string CurrentValue => inputValue;

        public bool IsActive { get; private set; }

        internal ModalFlow(Layout layout, CaretOptions caretOpt, ConfirmOptions confirmOpt, CancellationToken ct) {
            if (ct.CanBeCanceled) ctorCtr = ct.Register(() => { if (tcs != null) Complete(ModalResult.Cancel()); });
            modalAnimator = layout.ModalAnimator;
            modalView = layout.ModalView;
            vk = layout.VirtualKeyboard;
            this.confirmOpt = confirmOpt;
            strCtor = new VKStringConstructor(vk.KeyboardType);
            vCaret = VirtualCaret.NumpadCaret(vk.KeyboardType, caretOpt.HtmlStrRGBA);

            caretOnDuration = caretOpt.OnDuration;
            caretOffDuration = caretOpt.OffDuration;
            caretState = CaretState.OnTimer;
            caretIsOn = false;
            normalTxt = string.Empty;
            colorizedTxt = string.Empty;
            inputValue = string.Empty;
        }

        private void View_OnConfirmIntent() {
            if (!IsActive) { return; }
            Complete(ModalResult.Confirm(CurrentValue));
        }

        private void View_OnCancelIntent() {
            if (!IsActive) { return; }
            Complete(ModalResult.Cancel());
        }

        internal void ResetInput() {
            strCtor.ResetStr();
            modalView.SetInputFieldStr(string.Empty);
            normalTxt = string.Empty;
            colorizedTxt = string.Empty;
        }

        internal void OnVirtualKeyPressed(Enum input) {
            KeyboardState state = strCtor.Apply(input);
            inputValue = state.text;
            vk.EngageShift(state.shiftActive, state.shiftLocked);
            normalTxt = inputValue;
            colorizedTxt = vCaret.ApplyCaret(normalTxt, strCtor.CaretIndex);

            caretIsOn = false;
            caretState = CaretState.ValidateState;

            modalView.SetInputFieldStr(colorizedTxt);
            modalView.EnableConfirm(enabled: true, interactable: AllowedStrValue());
        }

        private bool AllowedStrValue() {
            string currentInput = modalView.GetInputFieldStr();

            if (currentInput.Equals(string.Empty)) {
                return confirmOpt.AllowEmptyString;
            }
            if (currentInput.Length < confirmOpt.MinWidth) {
                return false;
            }
            if (confirmOpt.InvalidStrings != null && confirmOpt.InvalidStrings.Length > 0) {
                for (int i = 0; i < confirmOpt.InvalidStrings.Length; i++) {
                    string invStr = confirmOpt.InvalidStrings[i];
                    if (currentInput.Equals(invStr)) {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void TickCaret(float deltaTime) {
            switch (caretState) {
                case CaretState.OnTimer:
                    caretTimer -= deltaTime;
                    caretTimer = MathUtils.Max(0f, caretTimer);
                    if (caretTimer == 0f) {
                        caretState = CaretState.ValidateState;
                    }
                    break;

                case CaretState.ValidateState:
                    string textToShow = caretIsOn ? normalTxt : colorizedTxt;
                    float duration = caretIsOn ? caretOnDuration : caretOffDuration;
                    caretTimer = duration;
                    caretIsOn = !caretIsOn;
                    caretState = CaretState.OnTimer;
                    modalView.SetInputFieldStr(textToShow);
                    break;
            }
        }

        public Task<ModalResult> ShowAsync(ModalOptions options, CancellationToken ct = default) {
            if (IsActive) { throw new InvalidOperationException("Modal already active"); }
            modalView.OnConfirmIntent += View_OnConfirmIntent;
            modalView.OnCancelIntent += View_OnCancelIntent;
            vk.OnVirtualKeyPressed += OnVirtualKeyPressed;
            if (options.cachedInputValue == CachedInputValue.Erase) {
                ResetInput();
            }
            IsActive = true;
            tcs = new TaskCompletionSource<ModalResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            if (!confirmOpt.AllowEmptyString && modalView.GetInputFieldStr().Equals(string.Empty)) {
                modalView.EnableConfirm(enabled: true, interactable: false);
            }

            modalAnimator.PlayShow(options.inputAnimOpt, options.vkAnimOpt);

            if (ct.CanBeCanceled) {
                showCtr = ct.Register(() => { Complete(ModalResult.Cancel()); });
            }

            return tcs.Task;
        }

        public async Task Hide() {
            if (IsActive) { throw new Exception("Cannot hide modal before result"); }
            modalAnimator.PlayHide();
            while (modalAnimator.IsVisible) {
                await Task.Yield();
            }
        }

        private void Complete(ModalResult result) {
            modalView.OnConfirmIntent -= View_OnConfirmIntent;
            modalView.OnCancelIntent -= View_OnCancelIntent;
            vk.OnVirtualKeyPressed -= OnVirtualKeyPressed;
            IsActive = false;
            showCtr.Dispose();
            ctorCtr.Dispose();
            var tmp_tcs = tcs;
            tcs = null;
            tmp_tcs.TrySetResult(result);
        }

        #region Class Data
        private enum CaretState {
            OnTimer, ValidateState
        };

        internal readonly struct CaretOptions {
            public readonly string HtmlStrRGBA;
            public readonly float OnDuration;
            public readonly float OffDuration;
            public CaretOptions(string htmlStrRGBA, float onDuration, float offDuration) {
                HtmlStrRGBA = htmlStrRGBA;
                OnDuration = onDuration;
                OffDuration = offDuration;
            }
        }

        internal readonly struct Layout {
            public readonly IModalView ModalView;
            public readonly IVirtualKeyboard VirtualKeyboard;
            public readonly IModalAnimator ModalAnimator;
            public Layout(IModalView modalView, IVirtualKeyboard virtualKeyboard, IModalAnimator modalAnimator) {
                ModalView = modalView;
                VirtualKeyboard = virtualKeyboard;
                ModalAnimator = modalAnimator;
            }
        }

        internal readonly struct ConfirmOptions {
            public readonly bool AllowEmptyString;
            public readonly int MinWidth;
            public readonly string[] InvalidStrings;
            public ConfirmOptions(bool allowEmptyString, int minWidth, params string[] invalidStrings) {
                AllowEmptyString = allowEmptyString;
                MinWidth = minWidth;
                InvalidStrings = invalidStrings;
            }
        }
        #endregion
    }
}
