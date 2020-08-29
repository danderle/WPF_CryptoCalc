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
            DataSetup.Data = "Data to be hashed";
            DataSetup.DataFormatSelected = Format.File;
            HmacChecked = false;
            Key = "some kind of hash key";

            KeyFormat.Add(Format.TextString.ToString());
            KeyFormat.Add(Format.HexString.ToString());
            KeyFormatSelected = Format.TextString;
        } 

        #endregion
    }
}
