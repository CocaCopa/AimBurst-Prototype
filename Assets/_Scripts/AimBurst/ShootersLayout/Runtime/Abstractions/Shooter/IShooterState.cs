
using AimBurst.Core.Contracts;
using CocaCopa.Primitives;

namespace AimBurst.ShootersLayout.Runtime.Abstractions {
    internal interface IShooterState {
        C_Vector3 Position { get; }
        CubeColor Color { get; }
        ShooterPositioning Positioning { get; }
        IShooterCombat CombatFriend { get; }
        bool OnCooldown { get; }
        bool HasAmmo { get; }
        int RemainingAmmo { get; }
        int LaneIndex { get; }
        bool IsHidden { get; }
    }
}
