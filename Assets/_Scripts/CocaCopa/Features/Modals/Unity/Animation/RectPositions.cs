using UnityEngine;

namespace CocaCopa.Modal.Unity.Animation {
    internal class RectPositions {
        internal Vector2 visible;
        internal Vector2 hiddenLeft;
        internal Vector2 hiddenRight;
        internal Vector2 hiddenBottom;
        internal RectPositions(Vector2 visible, Vector2 hiddenLeft, Vector2 hiddenRight, Vector2 hiddenBottom) {
            this.visible = visible;
            this.hiddenLeft = hiddenLeft;
            this.hiddenRight = hiddenRight;
            this.hiddenBottom = hiddenBottom;
        }
    }
}
