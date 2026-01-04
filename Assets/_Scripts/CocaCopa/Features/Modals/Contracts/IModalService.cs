using System.Threading;
using System.Threading.Tasks;

namespace CocaCopa.Modal.Contracts {
    public interface IModalService {
        Task<ModalResult> ShowAsync(ModalOptions options, CancellationToken ct);
        Task Hide();
        bool IsActive { get; }
    }
}
