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
        Text = 0,

        /// <summary>
        /// Hash a hex string
        /// </summary>
        Hex,

        /// <summary>
        /// Hash a file
        /// </summary>
        File,

    }
}
