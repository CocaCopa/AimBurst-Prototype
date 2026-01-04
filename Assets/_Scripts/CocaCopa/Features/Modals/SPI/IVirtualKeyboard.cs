using System;

namespace CocaCopa.Modal.SPI {
    internal interface IVirtualKeyboard {
        KeyboardType KeyboardType { get; }
        event Action<Enum> OnVirtualKeyPressed;
        void EngageShift(bool engage, bool locked);
    }
}
