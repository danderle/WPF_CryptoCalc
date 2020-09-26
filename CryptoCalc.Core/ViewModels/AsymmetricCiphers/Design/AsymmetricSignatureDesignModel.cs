namespace CryptoCalc.Core
{
    /// <summary>
    /// The design time model for the asymmetric signature page
    /// </summary>
    public class AsymmetricSignatureDesignModel : AsymmetricViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static AsymmetricSignatureDesignModel Instance => new AsymmetricSignatureDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AsymmetricSignatureDesignModel()
        {
            DataInput.DataFormatSelected = Format.HexString;
            DataInput.Data = "ffaa1111ff";
            KeyName = "Some name";
            KeyPairSetup = new KeyPairSetupViewModel(CryptographyApi.MSDN, AsymmetricOperation.Signature);
            KeyPairSetup.SelectedAlgorithimIndex = 2;
            KeyPairSetup.ChangedAlgorithim();
        }

        #endregion
    }
}
