using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// The design time model for the asymmetric encryption page
    /// </summary>
    public class AsymmetricEncryptionDesignModel : AsymmetricViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static AsymmetricEncryptionDesignModel Instance => new AsymmetricEncryptionDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AsymmetricEncryptionDesignModel()
        {
            DataInput.DataFormatSelected = Format.File;
            DataInput.Data = @"FilePath\Desktop\file.txt";
            KeyName = "Some name";
            KeyPairSetup = new KeyPairSetupViewModel(CryptographyApi.MSDN, AsymmetricOperation.Encryption);
        }

        #endregion
    }
}
