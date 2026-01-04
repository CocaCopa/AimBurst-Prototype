using UnityEngine;

namespace AimBurst.ShootersLayout.Unity.Actor {
    [RequireComponent(typeof(MeshRenderer))]
    internal sealed class BulletVisuals : MonoBehaviour {
        [SerializeField] private GameObject impactVfx;
        [SerializeField] private GameObject trailRendererObj;

        private MeshRenderer meshRenderer;

        private void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        internal void Impact() {
            impactVfx.SetActive(true);
            meshRenderer.enabled = false;
            trailRendererObj.SetActive(false);
        }
    }
}
