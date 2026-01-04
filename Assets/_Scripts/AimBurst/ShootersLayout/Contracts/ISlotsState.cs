using System.Threading.Tasks;

namespace AimBurst.ShootersLayout.Contracts {
    public interface ISlotsState {
        int TotalSlots { get; }
        Task<bool> ObserveSlotsInactive();
    }
}
