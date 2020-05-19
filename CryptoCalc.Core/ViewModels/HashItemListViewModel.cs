using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model that holds all the <see cref="HashItemViewModel"/> view models
    /// </summary>
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
            foreach (var item in Enum.GetValues(typeof(HashAlgorithim)).Cast<HashAlgorithim>().ToList())
            {
                Items.Add(new HashItemViewModel
                {
                    HashName = item.ToString(),
                });
            }
        } 

        #endregion
    }
}
