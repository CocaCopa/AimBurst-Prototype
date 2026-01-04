namespace CocaCopa.Modal.Contracts {
    public readonly struct ModalResult {
        public bool Confirmed { get; }
        public string Text { get; }
        private ModalResult(bool confirmed, string text) {
            Confirmed = confirmed;
            Text = text;
        }
        public static ModalResult Cancel() => new ModalResult(false, string.Empty);
        public static ModalResult Confirm(string text) => new ModalResult(true, text);
    }
}