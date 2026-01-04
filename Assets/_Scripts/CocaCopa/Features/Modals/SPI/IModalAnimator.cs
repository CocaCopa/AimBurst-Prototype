using System.Threading.Tasks;
using CocaCopa.Modal.Contracts;

namespace CocaCopa.Modal.SPI {
    internal interface IModalAnimator {
        void PlayShow(ModalAnimOptions input, ModalAnimOptions vk);
        void PlayHide(ModalAnimOptions input, ModalAnimOptions vk);
        void PlayShow();
        void PlayHide();
        bool IsVisible { get; }
    }
}
