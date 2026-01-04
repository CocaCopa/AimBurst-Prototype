using System;

namespace CocaCopa.Modal.SPI {
    internal interface IModalView {
        public event Action OnConfirmIntent;
        public event Action OnCancelIntent;
        void EnableConfirm(bool enabled, bool interactable);
        string GetInputFieldStr();
        void SetInputFieldStr(string txt);
    }
}
