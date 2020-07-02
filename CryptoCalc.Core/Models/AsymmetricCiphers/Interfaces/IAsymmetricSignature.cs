namespace CryptoCalc.Core
{
    public interface IAsymmetricSignature : IAsymmetricCipher
    {
        #region Public Methods

        public byte[] Sign(byte[] privKey, byte[] data);


        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data);

        #endregion
    }
}
