using System;

namespace CocaCopa.Primitives {
    /// <summary>
    /// Parsing helpers for ScaledInt. Keeps Core.Numerics self-contained.
    /// </summary>
    public static class ScaledIntParser {
        /// <summary>
        /// Parses a decimal string using the provided decimal separator into (value, scale).
        /// Returns Success=false on invalid input or overflow; otherwise Success=true.
        /// </summary>
        public static ScaledInt TryParseScaledInt(string text, char decimalChar) {
            if (string.IsNullOrWhiteSpace(text))
                return new ScaledInt(0, 1, success: false);

            return TryParseScaledInt(text.AsSpan().Trim(), decimalChar);
        }

        /// <summary>
        /// Span-based overload to avoid extra string allocations.
        /// </summary>
        public static ScaledInt TryParseScaledInt(ReadOnlySpan<char> s, char decimalChar) {
            if (s.IsEmpty)
                return new ScaledInt(0, 1, false);

            int sign = 1;
            int i = 0;

            if (s[0] == '+' || s[0] == '-') {
                if (s[0] == '-') sign = -1;
                i++;
            }

            if (i >= s.Length) return new ScaledInt(0, 1, false);

            int value = 0;
            int scale = 1;
            bool seenDecimal = false;
            int decimals = 0;

            for (; i < s.Length; i++) {
                char c = s[i];

                if (c == decimalChar) {
                    if (seenDecimal) return new ScaledInt(0, 1, false); // second decimal
                    seenDecimal = true;
                    continue;
                }

                if (c < '0' || c > '9')
                    return new ScaledInt(0, 1, false);

                int digit = c - '0';

                // value = value * 10 + digit; checked to avoid overflow
                try {
                    checked { value = value * 10 + digit; }
                }
                catch (OverflowException) {
                    return new ScaledInt(0, 1, false);
                }

                if (seenDecimal) {
                    // Bound decimals to something sane (e.g., 9)
                    if (decimals == 9) return new ScaledInt(0, 1, false);
                    scale *= 10;
                    decimals++;
                }
            }

            // Edge cases like "." or "-." → invalid
            if (value == 0 && !ContainsAnyDigit(s, decimalChar))
                return new ScaledInt(0, 1, false);

            return new ScaledInt(sign * value, scale, true);
        }

        private static bool ContainsAnyDigit(ReadOnlySpan<char> s, char decimalChar) {
            for (int j = 0; j < s.Length; j++) {
                char ch = s[j];
                if (ch != '+' && ch != '-' && ch != decimalChar && ch >= '0' && ch <= '9')
                    return true;
            }
            return false;
        }
    }
}
