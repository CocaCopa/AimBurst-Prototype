using System.Threading.Tasks;

namespace AimBurst.ShootersLayout.Runtime.Abstractions {
    internal interface ISlotManager {
        bool TryReserveFor(IShooterCombat shooter);
        bool TryReserveFor(IShooterCombat shooter, IShooterCombat attachedShooter);
        Task AcceptShooterAsync(IShooterCombat shooter);
    }
}
