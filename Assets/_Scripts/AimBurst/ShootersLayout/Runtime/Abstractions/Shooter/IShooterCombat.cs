using System.Threading.Tasks;
using AimBurst.LevelLayout.Contracts;

namespace AimBurst.ShootersLayout.Runtime.Abstractions {
    internal interface IShooterCombat : IShooterState {
        Task MoveToSlot(int toSlot);
        Task MoveOut(int slot, bool curved);
        void Shoot(ICube targetCube);
        void Merge(int newAmmo);
        void Kill();
    }
}
