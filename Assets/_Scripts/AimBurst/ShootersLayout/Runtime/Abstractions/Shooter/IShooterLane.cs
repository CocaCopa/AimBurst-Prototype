using System.Threading.Tasks;

namespace AimBurst.ShootersLayout.Runtime.Abstractions {
    internal interface IShooterLane : IShooterState {
        Task Advance(float distance);
        void SetPositioning(ShooterPositioning positioning);
    }
}