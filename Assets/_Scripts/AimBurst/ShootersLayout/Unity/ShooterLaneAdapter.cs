using System;
using AimBurst.Core.Contracts;
using AimBurst.ShootersLayout.Runtime;
using AimBurst.ShootersLayout.Runtime.Abstractions;
using UnityEngine;

namespace AimBurst.ShootersLayout.Unity {
    public sealed class ShooterLaneAdapter : MonoBehaviour, IClickable {
        private IShootersLayoutFlow flow;
        private ILaneController controller;

        int mainThreadId;

        void Awake() => mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

        void AssertMainThread() {
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != mainThreadId)
                UnityEngine.Debug.LogError("NOT MAIN THREAD");
        }


        internal void Wire(IShootersLayoutFlow flow, ILaneController controller) {
            this.flow ??= flow ?? throw new NullReferenceException($"<color=white>[{nameof(ShooterLaneAdapter)}]</color> {nameof(IShootersLayoutFlow)}");
            this.controller ??= controller ?? throw new NullReferenceException($"<color=white>[{nameof(ShooterLaneAdapter)}]</color> {nameof(ILaneController)}");
        }
        public void HandleClick() { flow.HandleClick(controller); }
    }
}
