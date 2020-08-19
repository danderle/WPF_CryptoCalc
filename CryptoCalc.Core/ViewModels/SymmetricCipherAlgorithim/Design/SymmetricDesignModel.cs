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
            DataSetup.Data = "Some data";
            DataSetup.DataFormatOptions = Enum.GetValues(typeof(Format)).Cast<Format>().Select(t => t.ToString()).ToList();
            DataSetup.DataIsCorrectlyFormatted = true;
            SecretKey = "ff33aaacdd";
            IV = "FFFFFFFFFFF";
            IvSize = 128;
            Algorithims = Enum.GetValues(typeof(SymmetricMsdnCipher)).Cast<SymmetricMsdnCipher>().Select(t => t.ToString()).ToList();
            KeySizes = new ObservableCollection<int> { 56, 55, 23 };
            SecretKeyAcceptable = false;
            IvSize = 50;
            IvAcceptable = false;
        }

        #endregion
    }
}
