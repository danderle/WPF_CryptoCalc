using System.Collections.Generic;

namespace CryptoCalc.Core
{
    public class HashItemListViewModel : BaseViewModel
    {

        #region Properties

        /// <summary>
        /// A list of the <see cref="HashItemViewModel"/>
        /// </summary>
        public List<HashItemViewModel> Items { get; set; } = new List<HashItemViewModel>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashItemListViewModel()
        {
        } 

        #endregion
    }
}
