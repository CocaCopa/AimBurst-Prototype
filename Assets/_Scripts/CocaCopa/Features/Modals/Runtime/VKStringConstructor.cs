using CocaCopa.Modal.Runtime.Internal;
using CocaCopa.Modal.SPI;

namespace CocaCopa.Modal.Runtime.Domain {
    internal class VKStringConstructor {
        private readonly KeyboardType kType;
        private readonly int MaxDecimal = 2;

        private NumpadState numpadState;
        private QwertyState qwertyState;

        internal int CaretIndex => kType == KeyboardType.Numpad ? numpadState.Caret : qwertyState.Caret;

        internal VKStringConstructor(KeyboardType kt) {
            kType = kt;

            if (kType == KeyboardType.Numpad) {
                numpadState = NumpadState.EmptyState(MaxDecimal);
            }
            else qwertyState = QwertyState.EmptyState();
        }

        internal KeyboardState Apply(System.Enum input) {
            if (input is NumpadInput numpad) {
                numpadState = NumpadRules.Apply(numpadState, numpad);
                string cr = numpadState.Text.Length > 0 ? "€" : string.Empty;
                return new KeyboardState(numpadState.Text + cr, shiftActive: false, shiftLocked: false);
            }
            else if (input is QwertyInput qwerty) {
                qwertyState = QwertyRules.Apply(qwertyState, qwerty);
                int shiftCounter = qwertyState.ShiftCounter;
                return new KeyboardState(qwertyState.Text, shiftCounter != 0, shiftCounter == 2);
            }

            throw new System.Exception("[VKStringConstructor] Could not read the provided input");
        }

        internal void ResetStr() {
            if (kType == KeyboardType.Numpad) {
                numpadState = NumpadState.EmptyState(MaxDecimal);
            }
            else qwertyState = QwertyState.EmptyState();
        }
    }
}
