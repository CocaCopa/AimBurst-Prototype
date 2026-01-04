using AimBurst.Core.Contracts;

namespace AimBurst.ShootersLayout.Contracts {
    public interface ILaneState {
        int CurrentShootersCount { get; }
        CubeColor? NextShooterColor { get; }
    }
}
