using System.IO;
using CocaCopa.SaveSystem.API;
using CocaCopa.SaveSystem.Runtime.Encryption;
using CocaCopa.SaveSystem.SPI;

namespace CocaCopa.SaveSystem.Runtime {
    /// <summary>
    /// Internal default implementation that connects the JSON file storage + optional AES
    /// to the public ISaveStorage contract.
    /// 
    /// <para>Internal and only visible to friend assemblies via InternalsVisibleTo.</para>
    /// </summary>
    internal sealed class DefaultSaveStorage : ISaveStorage {
        private readonly JsonFileStorage fileStorage;
        private readonly string rootDirectory;

        /// <param name="rootDirectory">
        /// Optional root directory that filePath will be relative to.
        /// If null or empty, filePath is treated as a full path.
        /// </param>
        /// <param name="passphrase">
        /// Optional passphrase. If null/empty, no encryption is used.
        /// </param>
        /// <param name="salt">
        /// Salt for key derivation if encryption is enabled. Must be at least 8 bytes.
        /// </param>
        public DefaultSaveStorage(IJsonSerializer jsonSerializer, string rootDirectory = null, string passphrase = null, byte[] salt = null) {
            IEncryptionTransform encryption = null;

            if (!string.IsNullOrWhiteSpace(passphrase)) {
                encryption = new AesEncryptionTransform(passphrase, salt);
            }

            fileStorage = new JsonFileStorage(jsonSerializer, encryption);
            this.rootDirectory = string.IsNullOrWhiteSpace(rootDirectory) ? null : rootDirectory;
        }

        public void Save<T>(T data, string filePath) {
            string resolved = ResolvePath(filePath);
            fileStorage.Save(data, resolved);
        }

        public bool TryLoad<T>(string filePath, out T result) {
            string resolved = ResolvePath(filePath);
            return fileStorage.Load(resolved, out result);
        }

        private string ResolvePath(string filePath) {
            if (string.IsNullOrWhiteSpace(rootDirectory))
                return filePath;

            if (Path.IsPathRooted(filePath))
                return filePath;

            return Path.Combine(rootDirectory, filePath);
        }
    }
}
