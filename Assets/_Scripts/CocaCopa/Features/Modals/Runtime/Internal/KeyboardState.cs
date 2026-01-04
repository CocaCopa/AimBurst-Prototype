namespace CocaCopa.Modal.Runtime.Internal {
    internal struct KeyboardState {
        public string text;
        public bool shiftActive;
        public bool shiftLocked;

        public KeyboardState(string text, bool shiftActive, bool shiftLocked) {
            this.text = text;
            this.shiftActive = shiftActive;
            this.shiftLocked = shiftLocked;
        }
    }
}
