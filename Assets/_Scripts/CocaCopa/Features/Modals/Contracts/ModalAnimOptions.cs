namespace CocaCopa.Modal.Contracts {
    [System.Serializable]
    public sealed class ModalAnimOptions {
        public Appear appear;
        public Disappear disappear;
        public ModalAnimOptions(Appear appear, Disappear disappear) {
            this.appear = appear;
            this.disappear = disappear;
        }
        public static ModalAnimOptions Left => new ModalAnimOptions(Appear.Left, Disappear.Left);
        public static ModalAnimOptions Right => new ModalAnimOptions(Appear.Right, Disappear.Right);
        public static ModalAnimOptions Bottom => new ModalAnimOptions(Appear.Bottom, Disappear.Bottom);
        public static ModalAnimOptions Top => new ModalAnimOptions(Appear.Top, Disappear.Top);
    }

    public enum Appear {
        Top, Bottom, Left, Right
    }

    public enum Disappear {
        Top, Bottom, Left, Right
    }
}
