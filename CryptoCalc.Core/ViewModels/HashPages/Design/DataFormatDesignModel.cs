using System.Collections.Generic;

namespace CryptoCalc.Core
{
    public class DataFormatDesignModel : DataFormatViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static DataFormatDesignModel Instance => new DataFormatDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataFormatDesignModel()
        {
            HmacChecked = false;
            Data = "Data to be hashed";
            Key = "some kind of hash key";

            KeyFormat.Add(Format.TextString.ToString());
            KeyFormat.Add(Format.HexString.ToString());
            DataFormatOptions.Add(Format.File.ToString());
            DataFormatOptions.Add(Format.TextString.ToString());
            DataFormatOptions.Add(Format.HexString.ToString());

            DataFormatSelected = 0;
            KeyFormatSelected = Format.TextString;
        } 

        #endregion
    }
}
