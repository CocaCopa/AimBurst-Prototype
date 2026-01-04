namespace CocaCopa.Modal.Runtime.Internal {
    internal readonly struct QwertyState {
        internal readonly string Text { get; }
        internal readonly int Caret { get; }
        internal readonly int ShiftCounter { get; }

        internal QwertyState(string text, int caret, int shiftCounter) {
            Text = text; Caret = caret; ShiftCounter = shiftCounter;
        }
        internal static QwertyState EmptyState() => new QwertyState(string.Empty, 0, 0);
    }
}
