using CryptoCalc.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the symmetric cipher page
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
            SelectedCipherApi = new MsdnSymmetricCipher();
            Algorithims = SelectedCipherApi.GetAlgorthims();
            KeySizes = SelectedCipherApi.GetKeySizes(SelectedAlgorithim);
            SelectedKeySize = KeySizes[0];
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);

            DataSetup.Data = "Some data";
            DataSetup.DataFormatOptions = Enum.GetValues(typeof(Format)).Cast<Format>().Select(t => t.ToString()).ToList();
            DataSetup.DataFormatSelected = Format.TextString;
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
