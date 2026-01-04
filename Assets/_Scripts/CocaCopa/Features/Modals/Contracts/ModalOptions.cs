namespace CocaCopa.Modal.Contracts {
    [System.Serializable]
    public struct ModalOptions {
        public CachedInputValue cachedInputValue;
        public ModalAnimOptions inputAnimOpt;
        public ModalAnimOptions vkAnimOpt;
        public ModalOptions(CachedInputValue cachedInputValue, ModalAnimOptions inputAnimOpt, ModalAnimOptions vkAnimOpt) {
            this.cachedInputValue = cachedInputValue;
            this.inputAnimOpt = inputAnimOpt;
            this.vkAnimOpt = vkAnimOpt;
        }
    }

    public enum CachedInputValue {
        Erase, Keep
    }
}
