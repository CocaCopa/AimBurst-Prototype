using UnityEngine;

namespace CocaCopa.Unity.Extensions {
    /// <summary>
    /// UI coordinate conversion and positioning utilities for RectTransforms.
    /// Provides clean conversions between world, anchored, and parent-local coordinates,
    /// as well as pivot- and anchor-aware helpers for precise UI placement.
    /// </summary>
    public static class UiCoords {
        // =====================================================
        // === WORLD ↔ ANCHORED CONVERSIONS =====================
        // =====================================================

        /// <summary>
        /// Converts a world position (usually representing the object's pivot)
        /// into the anchoredPosition required for <paramref name="rect"/> to be
        /// positioned under the specified <paramref name="targetParent"/>.
        /// Works correctly in any Canvas render mode.
        /// </summary>
        public static Vector2 WorldToAnchored(this Vector3 worldPos, RectTransform rect, RectTransform targetParent) {
            // world -> targetParent local
            Vector2 parentLocal = targetParent.InverseTransformPoint(worldPos);

            // anchor reference point based on rect's anchors and targetParent's pivot
            Vector2 parentSize = targetParent.rect.size;
            Vector2 anchorCenter = (rect.anchorMin + rect.anchorMax) * 0.5f;
            Vector2 anchorRef = new Vector2(
                (anchorCenter.x - targetParent.pivot.x) * parentSize.x,
                (anchorCenter.y - targetParent.pivot.y) * parentSize.y
            );

            return parentLocal - anchorRef;
        }

        /// <summary>
        /// Converts a world position (of the object's pivot) into the anchoredPosition
        /// needed for <paramref name="rect"/> under its current parent.
        /// Works in all Canvas render modes, including Screen Space - Overlay.
        /// </summary>
        public static Vector2 WorldToAnchored(this Vector3 worldPos, RectTransform rect) {
            var parent = (RectTransform)rect.parent;
            Vector2 parentLocal = parent.InverseTransformPoint(worldPos);

            Vector2 parentSize = parent.rect.size;
            Vector2 anchorCenter = (rect.anchorMin + rect.anchorMax) * 0.5f;
            Vector2 anchorRef = new Vector2(
                (anchorCenter.x - parent.pivot.x) * parentSize.x,
                (anchorCenter.y - parent.pivot.y) * parentSize.y
            );

            return parentLocal - anchorRef;
        }

        /// <summary>
        /// Converts an anchoredPosition (relative to a RectTransform's parent)
        /// into a world-space position.
        /// </summary>
        public static Vector3 AnchoredToWorld(this Vector2 anchored, RectTransform rect) {
            var parent = (RectTransform)rect.parent;
            Vector2 parentSize = parent.rect.size;
            Vector2 anchorCenter = (rect.anchorMin + rect.anchorMax) * 0.5f;
            Vector2 anchorRef = new Vector2(
                (anchorCenter.x - parent.pivot.x) * parentSize.x,
                (anchorCenter.y - parent.pivot.y) * parentSize.y
            );

            Vector3 parentLocal = (Vector3)(anchorRef + anchored);
            return parent.TransformPoint(parentLocal);
        }

        /// <summary>
        /// Converts a point expressed as an anchoredPosition relative to
        /// <paramref name="fromParent"/> into an anchoredPosition relative to
        /// <paramref name="toParent"/>.  
        /// Works in any Canvas mode without requiring a camera reference.
        /// </summary>
        public static Vector2 ConvertAnchoredBetweenParents(this Vector2 fromAnchored, RectTransform fromParent, RectTransform toParent) {
            Vector3 world = fromParent.TransformPoint(new Vector3(fromAnchored.x, fromAnchored.y, 0f));
            Vector2 toLocal = toParent.InverseTransformPoint(world);
            return toLocal;
        }

        // =====================================================
        // === PIVOT / ANCHOR MATH HELPERS =====================
        // =====================================================

        /// <summary>
        /// Returns the anchor reference point of this RectTransform (in parent-local units).  
        /// Formula: (anchorCenter - parentPivot) * parentSize
        /// </summary>
        public static Vector2 AnchorRef(this RectTransform rt) {
            var parent = (RectTransform)rt.parent;
            Vector2 parentSize = parent.rect.size;
            Vector2 anchorCenter = (rt.anchorMin + rt.anchorMax) * 0.5f;

            return new Vector2(
                (anchorCenter.x - parent.pivot.x) * parentSize.x,
                (anchorCenter.y - parent.pivot.y) * parentSize.y
            );
        }

        /// <summary>
        /// Returns the anchoredPosition that would place the child’s pivot
        /// exactly at its parent’s pivot (no mutation).
        /// </summary>
        public static Vector2 AnchoredAtParentPivot(this RectTransform child) {
            // anchoredPosition = childPivot - anchorRef → set to -anchorRef to land on parent pivot
            return -child.AnchorRef();
        }

        /// <summary>
        /// Sets the child’s anchors (anchorMin/anchorMax) to match its parent’s pivot.  
        /// After calling this, anchoredPosition == Vector2.zero will land exactly
        /// on the parent’s pivot.  
        /// ⚠️ Mutates anchors; call only during initialization or prefab setup.
        /// </summary>
        public static void SetAnchorsToParentPivot(this RectTransform child) {
            var parent = (RectTransform)child.parent;
            Vector2 p = parent.pivot;
            child.anchorMin = p;
            child.anchorMax = p;
        }

        /// <summary>
        /// Returns the coordinates (in parent-local space) of this RectTransform’s pivot.
        /// </summary>
        public static Vector2 PivotInParent(this RectTransform rt) {
            // anchoredPosition = pivot - AnchorRef  =>  pivot = anchoredPosition + AnchorRef
            return rt.anchoredPosition + rt.AnchorRef();
        }

        /// <summary>
        /// Returns the anchoredPosition needed for this RectTransform’s pivot
        /// to land at the specified parent-local point.
        /// </summary>
        public static Vector2 AnchoredAtParentPoint(this RectTransform rt, Vector2 parentLocalPoint) {
            return parentLocalPoint - rt.AnchorRef();
        }

        /// <summary>
        /// Returns the anchoredPosition that would place this RectTransform’s pivot
        /// exactly on another RectTransform’s pivot.  
        /// Assumes both share the same parent.
        /// </summary>
        public static Vector2 AnchoredAtSiblingPivot(this RectTransform rt, RectTransform other) {
            if (rt.parent != other.parent) {
                Debug.LogWarning("[UiCoords] AnchoredAtSiblingPivot: different parents; use AnchoredAtOtherPivot_CrossParent instead.");
            }

            Vector2 pivotPoint = other.PivotInParent();
            return rt.AnchoredAtParentPoint(pivotPoint);
        }

        /// <summary>
        /// Cross-parent version of <see cref="AnchoredAtSiblingPivot"/>.  
        /// Returns the anchoredPosition that would place this RectTransform’s pivot
        /// exactly on another RectTransform’s pivot, even if they have different parents.
        /// </summary>
        public static Vector2 AnchoredAtOtherPivot_CrossParent(this RectTransform rt, RectTransform other) {
            Vector2 otherPivotParent = other.PivotInParent();
            Vector3 world = other.parent.TransformPoint(otherPivotParent);

            var myParent = (RectTransform)rt.parent;
            Vector2 myParentLocal = myParent.InverseTransformPoint(world);

            return rt.AnchoredAtParentPoint(myParentLocal);
        }

        public static Vector2 GlobalToLocalDir(this Vector3 globalDir, RectTransform localSpace) {
            return localSpace.InverseTransformDirection(globalDir);
        }
    }
}
