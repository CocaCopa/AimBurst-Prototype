using System;
using System.IO;
using System.Security.Cryptography;

namespace CocaCopa.SaveSystem.Runtime.Encryption {
    internal sealed class AesEncryptionTransform : IEncryptionTransform {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        /// <summary>
        /// Creates an AES encryption transform by deriving a 256-bit key and a 128-bit IV
        /// from the given passphrase and salt using PBKDF2 (Rfc2898DeriveBytes).
        /// <para>
        /// Key derivation uses PBKDF2 with 10,000 iterations, producing:  
        /// 32-byte AES-256 key
        /// 16-byte IV
        /// </para>
        /// <para>
        /// Throws <see cref="ArgumentException"/> if inputs are invalid.
        /// </para>
        /// </summary>
        /// <param name="passphrase">must be a high-entropy secret unique to your app.</param>
        /// <param name="salt">must be a constant value of at least 8 bytes.</param>
        public AesEncryptionTransform(string passphrase, byte[] salt) {
            if (string.IsNullOrEmpty(passphrase))
                throw new ArgumentException("Passphrase must not be null or empty.", nameof(passphrase));
            if (salt == null || salt.Length < 8)
                throw new ArgumentException("Salt must be at least 8 bytes.", nameof(salt));

            // Derive key & IV from passphrase + salt
            using var keyDerivation = new Rfc2898DeriveBytes(passphrase, salt, 10000);
            // AES-256 = 32 bytes key, 16 bytes IV
            _key = keyDerivation.GetBytes(32);
            _iv = keyDerivation.GetBytes(16);
        }

        public byte[] Encrypt(byte[] plainBytes) {
            if (plainBytes == null || plainBytes.Length == 0)
                return plainBytes;

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var ms = new MemoryStream())
            using (var crypto = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write)) {
                crypto.Write(plainBytes, 0, plainBytes.Length);
                crypto.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        public byte[] Decrypt(byte[] cipherBytes) {
            if (cipherBytes == null || cipherBytes.Length == 0)
                return cipherBytes;

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var ms = new MemoryStream();
            using var crypto = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            crypto.Write(cipherBytes, 0, cipherBytes.Length);
            crypto.FlushFinalBlock();
            return ms.ToArray();
        }
    }
}
