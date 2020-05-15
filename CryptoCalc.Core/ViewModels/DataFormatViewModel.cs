using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCalc.Core
{
    public class DataFormatViewModel : BaseViewModel
    {

        #region Properties

        public bool HmacChecked { get; set; } = false;

        public string Data { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public List<string> KeyFormat { get; set; } = new List<string>();

        public List<string> DataFormat { get; set; } = new List<string>();

        public DataHashFormat DataFormatSelected { get; set; } = DataHashFormat.File;
        public DataHashFormat KeyFormatSelected { get; set; } = DataHashFormat.TextString;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataFormatViewModel()
        {
            DataFormat.Add(DataHashFormat.File.ToString());
            DataFormat.Add(DataHashFormat.TextString.ToString());
            DataFormat.Add(DataHashFormat.HexString.ToString());

            KeyFormat.Add(DataHashFormat.TextString.ToString());
            KeyFormat.Add(DataHashFormat.HexString.ToString());
        } 

        #endregion
    }
}
