using System.Threading.Tasks;

namespace AimBurst.ShootersLayout.Runtime.Abstractions {
    internal interface ILaneController {
        Task AdvanceLaneAsync();
        IShooterLane Peek();
        IShooterLane Dequeue();
    }
}
