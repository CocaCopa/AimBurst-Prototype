using System;
using CocaCopa.Modal.Contracts;
using CocaCopa.Modal.Unity;
using UnityEngine;

namespace CocaCopa.Modal.API {
    [RequireComponent(typeof(ModalInstaller))]
    public sealed class ModalAdapter : MonoBehaviour {
        private InvalidOperationException Exception => new InvalidOperationException(
            "Modal service not initialized yet. Ensure ModalInstaller initializes before accessing the adapter (e.g., access in Start or later), or have ModalInstaller initialize in Awake before dependents."
        );

        private ModalInstaller installer;
        private IModalService service;

        private void Awake() {
            installer = GetComponent<ModalInstaller>();
        }

        public IModalService GetService() {
            if (service != null) return service;

            service = installer.ModalService ?? throw Exception;
            return service;
        }
    }
}
