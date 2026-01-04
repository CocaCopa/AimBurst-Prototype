namespace CocaCopa.SaveSystem.Runtime.Encryption {
    internal interface IEncryptionTransform {
        byte[] Encrypt(byte[] plainBytes);
        byte[] Decrypt(byte[] cipherBytes);
    }
}
