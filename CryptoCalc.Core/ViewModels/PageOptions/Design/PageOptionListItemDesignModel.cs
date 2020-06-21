using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class PageOptionListItemDesignModel : PageOptionListItemViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static PageOptionListItemDesignModel Instance => new PageOptionListItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageOptionListItemDesignModel()
        {
        } 

        #endregion
    }
}
