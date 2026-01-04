using CocaCopa.Modal.Runtime.Internal;

namespace CocaCopa.Modal.Runtime.Domain {
    internal class NumpadRules {
        internal static NumpadState Apply(NumpadState currentState, NumpadInput input) {
            return input switch {
                NumpadInput.DecimalPoint => Input_Decimal(currentState),
                NumpadInput.Backspace => Input_Backspace(currentState),
                _ => Input_Digit(currentState, input)
            };
        }

        private static NumpadState Input_Decimal(NumpadState s) {
            if (s.Caret == s.Text.Length && s.OnDecimals) {
                return s;
            }
            else if (s.Text.Length == 0) {
                string newStr = "0.00";
                return new NumpadState(newStr, 2, true, s.MaxDecimals);
            }
            else if (!s.OnDecimals) {
                return new NumpadState(s.Text, s.Caret + 1, true, s.MaxDecimals);
            }
            else /* if (OnDecimals) */ {
                return s;
            }
        }

        internal static NumpadState Input_Backspace(NumpadState s) {
            if (s.Caret == 0) { return s; }
            int caretIndex = s.Caret - 1;
            bool onDecimals = s.OnDecimals;
            string currStr = s.Text;

            if (caretIndex == s.Text.Length - 1 - s.MaxDecimals) {
                caretIndex--;
                onDecimals = false;
            }

            if (onDecimals) {
                var rmCount = currStr.Length - caretIndex;
                currStr = currStr[..^rmCount];
                for (int i = 0; i < rmCount; i++) {
                    currStr += '0';
                }
            }
            else {
                string[] parts = currStr.Split('.');
                if (parts[0].Length > 1) {
                    currStr = parts[0][..^1];
                    currStr += $".{parts[1]}";
                }
                else { currStr = string.Empty; }
            }
            return new NumpadState(currStr, caretIndex, onDecimals, s.MaxDecimals);
        }

        private static NumpadState Input_Digit(NumpadState s, NumpadInput input) {
            if ((s.Text.Length > 6 && !s.OnDecimals) || (s.Caret == s.Text.Length && s.OnDecimals)) {
                return s;
            }

            string strInput = input.ToString();
            if (!strInput.Contains("Digit")) { throw new System.Exception("[NumpadRules] Could not read input"); }
            strInput = strInput.Replace("Digit", "");
            string newStr = "";

            if (s.Text.Length == 0 && input == NumpadInput.Digit0) {
                newStr = "0.00";
                return new NumpadState(newStr, 2, true, s.MaxDecimals);
            }

            if (s.Text.Length == 0) {
                newStr = $"{strInput}.00";
                return new NumpadState(newStr, s.Caret + 1, s.OnDecimals, s.MaxDecimals);
            }

            for (int i = 0; i < s.Text.Length; i++) {
                newStr += s.Text[i];
                if (s.Caret == i + 1) {
                    newStr += strInput;
                }
            }
            if (s.OnDecimals) { newStr = newStr[..^1]; }

            return new NumpadState(newStr, s.Caret + 1, s.OnDecimals, s.MaxDecimals);
        }
    }
}
