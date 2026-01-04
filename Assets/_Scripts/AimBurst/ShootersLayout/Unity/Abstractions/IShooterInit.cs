using AimBurst.ShootersLayout.Unity.Actor;
using AimBurst.ShootersLayout.Unity.Pathing;

namespace AimBurst.ShootersLayout.Unity.Abstractions {
    internal interface IShooterInit {
        void SetOwnerIndex(int index);
        void SetCombatFriend(Shooter shooter);
        void SetAmmo(int amount);
        void SetHidden(bool hidden);
        void WireShootersBezierRef(ShootersBezier shootersBezier);
    }
}
