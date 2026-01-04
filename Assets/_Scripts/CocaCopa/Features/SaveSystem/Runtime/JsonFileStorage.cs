using System.IO;
using System.Text;
using CocaCopa.SaveSystem.Runtime.Encryption;
using CocaCopa.SaveSystem.SPI;

namespace CocaCopa.SaveSystem.Runtime {
    internal sealed class JsonFileStorage {
        private readonly IEncryptionTransform _encryption;
        private readonly IJsonSerializer _json;

        public JsonFileStorage(IJsonSerializer jsonSerializer, IEncryptionTransform encryption = null) {
            _json = jsonSerializer ?? throw new System.ArgumentNullException(nameof(jsonSerializer));
            _encryption = encryption;
        }

        public void Save<T>(T data, string filePath) {
            if (data == null) {
                throw new System.Exception($"[JsonFileStorage] Tried to save null data of type {typeof(T).Name}");
            }

            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            string json = _json.ToJson(data);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            if (_encryption != null) {
                bytes = _encryption.Encrypt(bytes);
            }

            File.WriteAllBytes(filePath, bytes);
        }

        public bool Load<T>(string filePath, out T result) {
            if (!File.Exists(filePath)) {
                result = default;
                return false;
            }

            byte[] bytes = File.ReadAllBytes(filePath);

            if (_encryption != null) {
                try {
                    bytes = _encryption.Decrypt(bytes);
                }
                catch (System.Exception e) {
                    result = default;
                    throw new System.Exception($"[JsonFileStorage] Failed to decrypt '{filePath}': {e.Message}");
                }
            }

            string json = Encoding.UTF8.GetString(bytes);

            if (string.IsNullOrWhiteSpace(json)) {
                result = default;
                return false;
            }

            result = _json.FromJson<T>(json);

            if (result == null) {
                throw new System.Exception(
                    $"[JsonFileStorage] Failed to deserialize {typeof(T).Name} from '{filePath}'"
                );
            }

            return true;
        }
    }
}
