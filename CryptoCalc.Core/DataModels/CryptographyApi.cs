namespace CryptoCalc.Core
{
    /// <summary>
    /// The available cryptography librarys
    /// </summary>
    public enum CryptographyApi
    {
        /// <summary>
        /// Hash a file
        /// </summary>
        MSDN = 0,

        /// <summary>
        /// Hash a string
        /// </summary>
        BouncyCastle,
    }
}
