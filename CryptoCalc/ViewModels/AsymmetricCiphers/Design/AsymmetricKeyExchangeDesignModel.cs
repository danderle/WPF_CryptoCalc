using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// The design time model for the asymmetric key exchange page
    /// </summary>
    public class AsymmetricKeyExchangeDesignModel : AsymmetricViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static AsymmetricKeyExchangeDesignModel Instance => new AsymmetricKeyExchangeDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AsymmetricKeyExchangeDesignModel()
        {
            DataInput.DataFormatSelected = Format.Text;
            DataInput.Data = "Some message";
            KeyName = "Some name";
            KeyPairSetup = new KeyPairSetupViewModel(CryptographyApi.MSDN, AsymmetricOperation.KeyExchange);
        }

        #endregion
    }
}
