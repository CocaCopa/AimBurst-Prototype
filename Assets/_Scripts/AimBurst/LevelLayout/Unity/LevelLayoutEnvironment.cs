using UnityEngine;

namespace AimBurst.LevelLayout.Unity {
    public class LevelLayoutEnvironment : MonoBehaviour {
        [Header("References")]
        [SerializeField] private MeshRenderer ceiling;
        [SerializeField] private MeshRenderer floor;
        [SerializeField] private MeshRenderer floorGameArea;

        [Header("Normal Level")]
        [SerializeField] private Material normalCeiling;
        [SerializeField] private Material normalFloor;
        [SerializeField] private Material normalFloorInner;
        [SerializeField] private Material normalFloorOuter;

        [Header("Hard Level")]
        [SerializeField] private Material hardCeiling;
        [SerializeField] private Material hardFloor;
        [SerializeField] private Material hardFloorInner;
        [SerializeField] private Material hardFloorOuter;

        public void Init(bool hardLevel) {
            ceiling.sharedMaterial = hardLevel ? hardCeiling : normalCeiling;
            floor.sharedMaterial = hardLevel ? hardFloor : normalFloor;

            var mats = floorGameArea.sharedMaterials;
            mats[0] = hardLevel ? hardFloorOuter : normalFloorOuter;
            mats[1] = hardLevel ? hardFloorInner : normalFloorInner;
            floorGameArea.sharedMaterials = mats;
        }
    }
}
