using UnityEngine;

namespace CocaCopa.Unity.Components.Panel {
    internal class RectPositions {
        internal Vector2 visible;
        internal Vector2 hiddenTop;
        internal Vector2 hiddenBottom;
        internal Vector2 hiddenLeft;
        internal Vector2 hiddenRight;
        internal RectPositions(Vector2 visible, Vector2 hiddenTop, Vector2 hiddenBottom, Vector2 hiddenLeft, Vector2 hiddenRight) {
            this.visible = visible;
            this.hiddenTop = hiddenTop;
            this.hiddenBottom = hiddenBottom;
            this.hiddenLeft = hiddenLeft;
            this.hiddenRight = hiddenRight;
        }
    }
}
