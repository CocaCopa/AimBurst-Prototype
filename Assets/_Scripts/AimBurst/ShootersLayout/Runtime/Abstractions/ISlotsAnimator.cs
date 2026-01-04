using System.Threading.Tasks;

namespace AimBurst.ShootersLayout.Runtime.Abstractions {
    internal interface ISlotsAnimator {
        Task<IShooterCombat> MergeShootersAsync(IShooterCombat[] shooters);
    }
}
