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

            KeyFormat.Add(DataHashFormat.TextString.ToString());
            KeyFormat.Add(DataHashFormat.HexString.ToString());
            DataFormat.Add(DataHashFormat.File.ToString());
            DataFormat.Add(DataHashFormat.TextString.ToString());
            DataFormat.Add(DataHashFormat.HexString.ToString());

            DataFormatSelected = DataHashFormat.File;
            KeyFormatSelected = DataHashFormat.TextString;
        } 

        #endregion
    }
}
