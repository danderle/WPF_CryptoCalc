using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CryptoCalc
{
    public class PageOptionItemDesignModel : PageOptionItemViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static PageOptionItemDesignModel Instance => new PageOptionItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageOptionItemDesignModel()
        {
            PageName = "MSDN Hash";
            IsChecked = true;
        } 

        #endregion
    }
}
