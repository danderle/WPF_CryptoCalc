using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoCalc.Core
{
    public class DataFormatViewModel : BaseViewModel
    {

        #region Properties

        /// <summary>
        /// Keyed-Hash Message Authentication Code
        /// </summary>
        public bool HmacChecked { get; set; } = false;

        /// <summary>
        /// The data to be hashed <see cref="DataHashFormat"/> for hash data format options
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// The Hmac key
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// List for holding the key format options
        /// </summary>
        public List<string> KeyFormat { get; set; } = new List<string>();

        /// <summary>
        /// Holds the dat format options
        /// </summary>
        public List<string> DataFormat { get; set; } = new List<string>();

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public DataHashFormat DataFormatSelected { get; set; } = DataHashFormat.File;

        /// <summary>
        /// Currently selected key format
        /// </summary>
        public DataHashFormat KeyFormatSelected { get; set; } = DataHashFormat.TextString;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataFormatViewModel()
        {
            // Adds the formats to the lists
            DataFormat = Enum.GetValues(typeof(DataHashFormat)).Cast<DataHashFormat>().Select(t => t.ToString()).ToList();
            KeyFormat.Add(DataHashFormat.TextString.ToString());
            KeyFormat.Add(DataHashFormat.HexString.ToString());
        } 

        #endregion
    }
}
