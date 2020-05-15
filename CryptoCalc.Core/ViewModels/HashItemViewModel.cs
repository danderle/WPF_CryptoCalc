using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCalc.Core
{
    public class HashItemViewModel : BaseViewModel
    {

        #region Properties

        public bool IsChecked { get; set; }

        public string HashName { get; set; }

        public string HashValue { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashItemViewModel()
        {
        } 

        #endregion
    }
}
