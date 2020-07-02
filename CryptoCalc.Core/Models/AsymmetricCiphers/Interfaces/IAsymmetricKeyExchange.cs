namespace CryptoCalc.Core
{
    public interface IAsymmetricKeyExchange
    {
        #region Public Methods

        public byte[] DeriveKey(byte[] myPrivateKey, int cipherKeySize, byte[] otherPartyPublicKey);

        #endregion
    }
}
