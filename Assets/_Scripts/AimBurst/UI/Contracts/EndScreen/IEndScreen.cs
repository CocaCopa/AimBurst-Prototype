using System.Threading.Tasks;

namespace AimBurst.UI.Contracts.EndScreen {
    public interface IEndScreen {
        Task<bool> Trigger(State state, bool hideNextLvlBtn);
    }
}
