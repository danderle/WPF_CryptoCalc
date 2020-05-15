using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCalc.Core
{
    public class HashViewModel : BaseViewModel
    {

        #region Properties

        public DataFormatViewModel DataFormatSetup { get; set; } = new DataFormatViewModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashViewModel()
        {
        } 

        #endregion
    }
}
