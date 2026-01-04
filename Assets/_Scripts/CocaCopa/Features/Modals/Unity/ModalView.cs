using System;
using System.Text.RegularExpressions;
using CocaCopa.Core;
using CocaCopa.Modal.SPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocaCopa.Modal.Unity {
    internal class ModalView : MonoBehaviour, IModalView {
        [Header("References")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public event Action OnConfirmIntent;
        public event Action OnCancelIntent;

        private void Awake() {
            inputField.DeactivateInputField();
            inputField.richText = true;
            confirmButton.interactable = false;

            confirmButton.onClick.AddListener(OnConfirmClicked);
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        private void OnConfirmClicked() => OnConfirmIntent?.SafeInvoke(nameof(OnConfirmIntent));
        private void OnCancelClicked() => OnCancelIntent?.SafeInvoke(nameof(OnCancelIntent));
        public void EnableConfirm(bool enabled, bool interactable) {
            confirmButton.enabled = enabled;
            confirmButton.interactable = interactable;
        }
        public void SetInputFieldStr(string txt) => inputField.text = txt;
        public string GetInputFieldStr() => Regex.Replace(inputField.text, "<.*?>", string.Empty);
    }
}
