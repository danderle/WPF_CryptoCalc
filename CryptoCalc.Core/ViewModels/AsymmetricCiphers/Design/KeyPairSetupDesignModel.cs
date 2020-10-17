namespace CryptoCalc.Core
{
    /// <summary>
    /// The design time model for the key pair setup control
    /// </summary>
    public class KeyPairSetupDesignModel : KeyPairSetupViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static KeyPairSetupDesignModel Instance => new KeyPairSetupDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyPairSetupDesignModel()
        {
            SelectedOperation = AsymmetricOperation.Encryption;
            Algorithims = BaseMsdnAsymmetric.GetAlgorthims(SelectedOperation);
            //Algorithims = IAsymmetricCipher.GetBouncyAlgorthims(SelectedOperation);
            SelectedAlgorithimIndex = 0;
            BaseMsdnAsymmetric.GetCipher(Algorithims[SelectedAlgorithimIndex]);

            PrivateKey = ByteConvert.HexStringToBytes("FFFFFFFFF111000");
            PublicKey = ByteConvert.HexStringToBytes("00000000FFFFFFF");
        }

        #endregion
    }
}
