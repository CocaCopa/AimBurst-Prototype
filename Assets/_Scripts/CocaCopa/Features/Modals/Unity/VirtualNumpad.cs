using CocaCopa.Modal.Runtime.Internal;
using CocaCopa.Modal.SPI;
using UnityEngine;
using UnityEngine.UI;

namespace CocaCopa.Modal.Unity {
    internal class VirtualNumpad : VirtualKeyboardBase {
        [Header("Digit Rows")]
        [SerializeField] private Button numpad1;
        [SerializeField] private Button numpad2;
        [SerializeField] private Button numpad3;
        [SerializeField] private Button numpad4;
        [SerializeField] private Button numpad5;
        [SerializeField] private Button numpad6;
        [SerializeField] private Button numpad7;
        [SerializeField] private Button numpad8;
        [SerializeField] private Button numpad9;

        [Header("Side Buttons")]
        [SerializeField] private Button numpadBackspace;
        [SerializeField] private Button numpadDot;
        [SerializeField] private Button numpad0;

        protected override KeyboardType VKType => KeyboardType.Numpad;

        protected override void AddListeners() {
            numpad1.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit1));
            numpad2.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit2));
            numpad3.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit3));
            numpad4.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit4));
            numpad5.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit5));
            numpad6.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit6));
            numpad7.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit7));
            numpad8.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit8));
            numpad9.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit9));
            numpad0.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Digit0));
            numpadDot.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.DecimalPoint));
            numpadBackspace.onClick.AddListener(() => InvokeOnKeyPressed(NumpadInput.Backspace));
        }
    }
}
