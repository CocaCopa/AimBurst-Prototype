using CocaCopa.Modal.Runtime.Internal;

namespace CocaCopa.Modal.Runtime.Domain {
    internal class QwertyRules {
        internal static QwertyState Apply(QwertyState currentState, QwertyInput input) {
            return input switch {
                QwertyInput.Shift => Input_Shift(currentState),
                QwertyInput.Backspace => Input_BackSpace(currentState),
                QwertyInput.Spacebar => Input_Spacebar(currentState),
                _ => Input_Character(currentState, input)
            };
        }

        private static QwertyState Input_Shift(QwertyState s) {
            int shiftCounter = s.ShiftCounter;
            if (shiftCounter == 2) { shiftCounter = 0; }
            else shiftCounter++;
            return new QwertyState(s.Text, s.Caret, shiftCounter);
        }

        private static QwertyState Input_BackSpace(QwertyState s) {
            string text = s.Text.Length > 1 ? s.Text[..^1] : string.Empty;
            int caret = s.Caret - 1;
            return new QwertyState(text, caret, s.ShiftCounter);
        }

        private static QwertyState Input_Spacebar(QwertyState s) {
            string text = s.Text + ' ';
            int caret = s.Caret + 1;
            return new QwertyState(text, caret, s.ShiftCounter);
        }

        private static QwertyState Input_Character(QwertyState s, QwertyInput input) {
            string inputString = input.ToString();
            int shiftCounter = s.ShiftCounter;
            string newString;
            if (inputString.Contains("Alpha")) {
                string str = inputString.Replace("Alpha", string.Empty);
                newString = s.Text + str;
            }
            else if (inputString.Length == 1) {
                string str = shiftCounter != 0 ? inputString.ToUpper() : inputString.ToLower();
                if (shiftCounter == 1) shiftCounter = 0;
                newString = s.Text + str;
            }
            else throw new System.Exception("[QwertyRules] Could not read input");

            return new QwertyState(newString, s.Caret + 1, shiftCounter);
        }
    }
}
