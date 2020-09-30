namespace CryptoCalc.Core
{
    /// <summary>
    /// The desing time model for the <see cref="HashItemViewModel"/>
    /// </summary>
    public class HashItemDesignModel : HashItemViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static HashItemDesignModel Instance => new HashItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashItemDesignModel()
        {
            IsChecked = true;
            HashName = "MD5";
            HashValue = "436547f766gfwcdsg546745436547f766gfwcdsg546745436gfwcdsg546745436547f766gfwcdsg546745";
        } 

        #endregion
    }
}
