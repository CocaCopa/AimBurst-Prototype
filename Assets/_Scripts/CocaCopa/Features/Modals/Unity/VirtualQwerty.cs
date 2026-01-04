using System.Collections.Generic;
using CocaCopa.Modal.Runtime.Internal;
using CocaCopa.Modal.SPI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CocaCopa.Modal.Unity {
    internal class VirtualQwerty : VirtualKeyboardBase {
        [Header("All keys in order (must match KeyOrder)")]
        [SerializeField] private Button[] virtualKeys;

        [Header("Special: Shift")]
        [SerializeField] private Sprite engagedShiftSprite;
        [SerializeField] private Color lockedShiftColor = Color.white;

        protected override KeyboardType VKType => KeyboardType.QWERTY;
        private Sprite defaultShiftSprite;
        private Color defaultShiftColor;

        private readonly Dictionary<Button, QwertyInput> map = new Dictionary<Button, QwertyInput>();
        private readonly Dictionary<Button, MonoBehaviour> buttonTexts = new Dictionary<Button, MonoBehaviour>();

        // Expected key order for the serialized array
        private static readonly QwertyInput[] KeyOrder = {
            // Specials
            QwertyInput.Shift, QwertyInput.Backspace, QwertyInput.Spacebar,

            // Row 0 (numbers)
            QwertyInput.Alpha1, QwertyInput.Alpha2, QwertyInput.Alpha3, QwertyInput.Alpha4, QwertyInput.Alpha5,
            QwertyInput.Alpha6, QwertyInput.Alpha7, QwertyInput.Alpha8, QwertyInput.Alpha9, QwertyInput.Alpha0,

            // Row 1
            QwertyInput.Q, QwertyInput.W, QwertyInput.E, QwertyInput.R, QwertyInput.T,
            QwertyInput.Y, QwertyInput.U, QwertyInput.I, QwertyInput.O, QwertyInput.P,

            // Row 2
            QwertyInput.A, QwertyInput.S, QwertyInput.D, QwertyInput.F, QwertyInput.G,
            QwertyInput.H, QwertyInput.J, QwertyInput.K, QwertyInput.L,

            // Row 3
            QwertyInput.Z, QwertyInput.X, QwertyInput.C, QwertyInput.V,
            QwertyInput.B, QwertyInput.N, QwertyInput.M,
        };

        protected override void Awake() {
            base.Awake();
            MapCompToVKs();
            defaultShiftSprite = (buttonTexts[virtualKeys[0]] as Image).sprite;
            defaultShiftColor = (buttonTexts[virtualKeys[0]] as Image).color;
        }

        private void MapCompToVKs() {
            for (int i = 0; i < virtualKeys.Length; i++) {
                Button btn = virtualKeys[i];
                TextMeshProUGUI txtComp = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (txtComp == null) {
                    Image imgComp = btn.transform.GetChild(0).GetComponent<Image>();
                    buttonTexts[btn] = imgComp;
                }
                else buttonTexts[btn] = txtComp;
            }
        }

        protected override void AddListeners() {
            if (virtualKeys == null || virtualKeys.Length == 0) {
                Debug.LogError($"{nameof(VirtualQwerty)}: No elements assigned.");
                return;
            }
            if (virtualKeys.Length != KeyOrder.Length) {
                Debug.LogError($"{nameof(VirtualQwerty)}: elements.Length ({virtualKeys.Length}) != KeyOrder.Length ({KeyOrder.Length}). Fix the Inspector order/size.");
                return;
            }

            map.Clear();

            for (int i = 0; i < virtualKeys.Length; i++) {
                var btn = virtualKeys[i];
                if (btn == null) {
                    Debug.LogError($"{nameof(VirtualQwerty)}: elements[{i}] is null. Fix your references.");
                    continue;
                }

                var key = KeyOrder[i];
                if (map.ContainsKey(btn)) {
                    Debug.LogWarning($"{nameof(VirtualQwerty)}: Duplicate Button at index {i} ignored.");
                    continue;
                }

                map.Add(btn, key);
            }

            foreach (var pair in map) {
                var button = pair.Key;
                var input = pair.Value;
                button.onClick.AddListener(() => InvokeOnKeyPressed(input));
            }
        }

        public override void EngageShift(bool engage, bool locked) {
            MonoBehaviour shiftImg = buttonTexts[virtualKeys[0]];
            if (shiftImg is Image shift) {
                shift.sprite = engage ? engagedShiftSprite : defaultShiftSprite;
                shift.color = locked ? lockedShiftColor : defaultShiftColor;
            }
            else Debug.LogError($"{nameof(VirtualQwerty)}: Shift key not found. Please make sure the inspector order is correct.");
            for (int i = 0; i < buttonTexts.Values.Count; i++) {
                buttonTexts.TryGetValue(virtualKeys[i], out var textComp);
                if (textComp is TextMeshProUGUI tmpTxt) {
                    tmpTxt.SetText(
                        engage ? tmpTxt.text.ToUpper() : tmpTxt.text.ToLower()
                    );
                }
            }
        }
    }
}
