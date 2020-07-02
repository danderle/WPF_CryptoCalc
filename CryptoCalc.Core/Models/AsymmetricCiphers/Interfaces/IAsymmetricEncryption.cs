namespace CryptoCalc.Core
{
    public interface IAsymmetricEncryption : IAsymmetricCipher
    {
        #region Public Methods

        public byte[] EncryptText(byte[] publicKey, string plainText);

        public byte[] EncryptBytes(string selectedAlgorithim, int keySize, byte[] plainBytes);

        public string DecryptToText(byte[] privateKey, byte[] encrypted);

        public byte[] DecryptToBytes(string selectedAlgorithim, int keySize, byte[] encrypted);

        #endregion
    }
}
