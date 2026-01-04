using System;
using AimBurst.Core.Contracts;
using UnityEngine;

namespace AimBurst.Input {
    [DisallowMultipleComponent]
    internal sealed class GameInputAdapter : MonoBehaviour, IInputPublisher {
        public event Action<IClickable> OnLeftClick;

        [Header("Raycast")]
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private float maxDistance = 500f;

        private Camera Cam => Camera.main;

        private void Awake() {
            GameInputAPI.OverrideImplementation(this);
        }

        private void Update() {
            if (UnityEngine.Input.GetMouseButtonDown(0)) {
                CaptureLeftClick();
            }
        }

        private void CaptureLeftClick() {
            Ray ray = Cam.ScreenPointToRay(UnityEngine.Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitMask, QueryTriggerInteraction.Ignore)) {
                IClickable clickable = hit.collider.GetComponentInParent<IClickable>();
                if (clickable == null) { return; }
                OnLeftClick?.Invoke(clickable);
                clickable.HandleClick();
            }
        }
    }
}
