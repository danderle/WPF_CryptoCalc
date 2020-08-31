using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The design time model for the symmetric cipher page
    /// </summary>
    public class SymmetricDesignModel : SymmetricViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static SymmetricDesignModel Instance => new SymmetricDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SymmetricDesignModel()
        {
            SelectedCipherApi = new BouncySymmetricCipher();
            //SelectedCipherApi = new MsdnSymmetricCipher();
            Algorithims = SelectedCipherApi.GetAlgorthims();
            KeySizes = SelectedCipherApi.GetKeySizes(SelectedAlgorithim);
            SelectedKeySize = KeySizes[0];
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);

            DataSetup.Data = "Some data";
            DataSetup.DataFormatOptions = Enum.GetValues(typeof(Format)).Cast<Format>().Select(t => t.ToString()).ToList();
            DataSetup.DataFormatSelected = Format.File;
            SecretKey = "FFFFFFFFFFFF";
            IV = "FFFFFFFFFFFF";
            IvSize = 6*8;
            KeySizes = new ObservableCollection<int> { 6*8, 55, 23 };
            SelectedKeySize = 6*8;
            IvSize = 6*8;

        }

        #endregion
    }
}
