namespace CocaCopa.SaveSystem.SPI {
    /// <summary>
    /// Abstraction over JSON serialization so Runtime does not depend on Unity or a specific JSON library.
    /// </summary>
    public interface IJsonSerializer {
        string ToJson<T>(T value);
        T FromJson<T>(string json);
    }
}
