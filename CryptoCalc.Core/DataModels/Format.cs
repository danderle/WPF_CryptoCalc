namespace CryptoCalc.Core
{
    /// <summary>
    /// Data format options for hashing
    /// </summary>
    public enum Format
    {
        /// <summary>
        /// Hash a string
        /// </summary>
        TextString = 0,

        /// <summary>
        /// Hash a hex string
        /// </summary>
        HexString,

        /// <summary>
        /// Hash a file
        /// </summary>
        File,

    }
}
