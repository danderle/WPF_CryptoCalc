namespace CryptoCalc.Core
{
    /// <summary>
    /// Data format options for hashing
    /// </summary>
    public enum Format
    {
        /// <summary>
        /// Hash a file
        /// </summary>
        File = 0,

        /// <summary>
        /// Hash a string
        /// </summary>
        TextString,

        /// <summary>
        /// Hash a hex string
        /// </summary>
        HexString,
    }
}
