namespace CocaCopa.Unity.Components.Panel {
    [System.Serializable]
    public sealed class AnimOptions {
        public UIAppear appear;
        public UIDisappear disappear;
        public AnimOptions(UIAppear appear, UIDisappear disappear) {
            this.appear = appear;
            this.disappear = disappear;
        }
        public static AnimOptions Left => new AnimOptions(UIAppear.Left, UIDisappear.Left);
        public static AnimOptions Right => new AnimOptions(UIAppear.Right, UIDisappear.Right);
        public static AnimOptions Bottom => new AnimOptions(UIAppear.Bottom, UIDisappear.Bottom);
        public static AnimOptions Top => new AnimOptions(UIAppear.Top, UIDisappear.Top);
    }

    public enum UIAppear {
        Top, Bottom, Left, Right
    }

    public enum UIDisappear {
        Top, Bottom, Left, Right
    }
}
