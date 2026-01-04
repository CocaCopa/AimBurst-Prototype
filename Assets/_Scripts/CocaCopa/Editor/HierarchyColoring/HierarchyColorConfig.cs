using UnityEngine;

namespace CocaCopa.EditorUtils.HierarchyColoring {
    public class HierarchyColorConfig : ScriptableObject {
        public string targetString = "----";
        public Color fontColor = Color.red;
        public Color backgroundColor = new Color32(56, 56, 56, 255);
        public FontStyle fontStyle = FontStyle.Normal;
    }
}
