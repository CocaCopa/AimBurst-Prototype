using System;

namespace CocaCopa.Primitives {
    /// <summary>
    /// Represents a scaled integer: Value / Scale == original decimal.
    /// </summary>
    public readonly struct ScaledInt {
        public readonly int Value;
        public readonly int Scale;
        public readonly bool Success;

        public ScaledInt(int value, int scale, bool success) {
            Value = value;
            Scale = scale;
            Success = success;
        }

        public override string ToString() => Success ? $"{Value}/{Scale}" : "Invalid";
    }
}
