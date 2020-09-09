namespace CryptoCalc.Core
{
    /// <summary>
    /// The design time model for the asymmetric cipher page
    /// </summary>
    public class AsymmetricDesignModel : AsymmetricViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static AsymmetricDesignModel Instance => new AsymmetricDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AsymmetricDesignModel()
        {
            DataInput.DataFormatSelected = Format.File;
            KeyName = "Some name";
            PrivateKeyPath = $"C:/home/Desktop/Privatefile.txt";
            PublicKeyPath = $"C:/home/Desktop/Publicfile.txt";
        }

        #endregion
    }
}
