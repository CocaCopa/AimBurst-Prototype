using AimBurst.ShootersLayout.Runtime.Abstractions;

namespace AimBurst.ShootersLayout.Runtime {
    internal interface IShootersLayoutFlow {
        void HandleClick(ILaneController clickedLane);
    }
}
