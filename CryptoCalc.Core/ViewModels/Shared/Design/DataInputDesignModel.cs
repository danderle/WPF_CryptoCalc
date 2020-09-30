namespace CryptoCalc.Core
{
    public class DataInputDesignModel : DataInputViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static DataInputDesignModel Instance => new DataInputDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataInputDesignModel()
        {
            DataFormatSelected = Format.File;
            Data = "Some Data to be processed, Some Data to be processed,Some Data to be processed,Some Data to be processed";
        } 

        #endregion
    }
}
