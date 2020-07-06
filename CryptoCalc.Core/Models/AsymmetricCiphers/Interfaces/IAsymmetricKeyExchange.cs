namespace CryptoCalc.Core
{
    /// <summary>
    /// A interface used for asymmetric key exchange
    /// </summary>
    public interface IAsymmetricKeyExchange
    {
        #region Public Methods

        /// <summary>
        /// Drevies a shared secret key from a private key and another persons public key
        /// </summary>
        /// <param name="myPrivateKey">the private key which is used</param>
        /// <param name="cipherKeySize">the size of the private key</param>
        /// <param name="otherPartyPublicKey">the public key of the other person</param>
        /// <returns></returns>
        public byte[] DeriveKey(byte[] myPrivateKey, int cipherKeySize, byte[] otherPartyPublicKey);

        #endregion
    }
}
