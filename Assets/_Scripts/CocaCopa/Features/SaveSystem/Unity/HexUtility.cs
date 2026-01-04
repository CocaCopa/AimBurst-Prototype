using System;
using System.Globalization;

namespace CocaCopa.SaveSystem.Unity {
    internal static class HexUtility {
        public static byte[] ParseHexToBytes(string hex, string contextName) {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException($"[{contextName}] Salt hex string must not be empty.");

            hex = hex.Replace(" ", string.Empty);

            if (hex.Length % 2 != 0)
                throw new ArgumentException($"[{contextName}] Salt hex string must have an even length.");

            int len = hex.Length / 2;
            if (len < 8)
                throw new ArgumentException($"[{contextName}] Salt must be at least 8 bytes (16 hex characters).");

            var bytes = new byte[len];

            for (int i = 0; i < len; i++) {
                string b = hex.Substring(i * 2, 2);
                bytes[i] = byte.Parse(b, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return bytes;
        }
    }
}
