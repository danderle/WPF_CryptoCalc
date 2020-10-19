using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// The desing time model for the <see cref="HmacViewModel"/>
    /// </summary>
    public class HmacDesignModel : HmacViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static HmacDesignModel Instance => new HmacDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HmacDesignModel()
        {
            HmacChecked = true;
            Key = "some kind of hash key";

            KeyFormat.Add(Format.TextString.ToString());
            KeyFormat.Add(Format.HexString.ToString());
            KeyFormatSelected = Format.TextString;
        } 

        #endregion
    }
}
