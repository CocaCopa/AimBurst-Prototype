using System;
using CocaCopa.Core;
using CocaCopa.Modal.SPI;
using UnityEngine;

namespace CocaCopa.Modal.Unity {
    internal abstract class VirtualKeyboardBase : MonoBehaviour, IVirtualKeyboard {
        public event Action<Enum> OnVirtualKeyPressed;
        public KeyboardType KeyboardType => VKType;
        protected abstract KeyboardType VKType { get; }

        protected virtual void Awake() {
            AddListeners();
        }

        protected abstract void AddListeners();

        protected void InvokeOnKeyPressed<T>(T input) where T : Enum {
            OnVirtualKeyPressed?.SafeInvoke(input, nameof(OnVirtualKeyPressed));
        }

        public virtual void EngageShift(bool engage, bool locked) {

        }
    }
}
