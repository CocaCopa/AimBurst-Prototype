namespace CocaCopa.Modal.Runtime.Internal {
    internal readonly struct NumpadState {
        internal readonly string Text { get; }
        internal readonly int Caret { get; }
        internal readonly bool OnDecimals { get; }
        internal readonly int MaxDecimals { get; }
        internal NumpadState(string text, int caret, bool onDecimals, int maxDecimals) {
            Text = text; Caret = caret; OnDecimals = onDecimals; MaxDecimals = maxDecimals;
        }
        internal static NumpadState EmptyState(int decimals) => new NumpadState(string.Empty, 0, false, decimals);
    }
}
