namespace CocaCopa.SaveSystem.API {
    /// <summary>
    /// Abstraction over save/load operations.
    /// Implementations decide how and where data is stored.
    /// </summary>
    public interface ISaveStorage {
        void Save<T>(T data, string filePath);
        bool TryLoad<T>(string filePath, out T result);
    }
}
