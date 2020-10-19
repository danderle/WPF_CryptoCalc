using CryptoCalc.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CryptoCalc
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
        }

        /// <summary>
        /// Adds all the hash algorithims strings to the list
        /// </summary>
        /// <param name="api"></param>
        public HashItemListViewModel(CryptographyApi api)
        {
            switch(api)
            {
                case CryptographyApi.MSDN:
                    foreach (var item in Enum.GetValues(typeof(MsdnHashAlgorithim)).Cast<MsdnHashAlgorithim>().ToList())
                    {
                        Items.Add(new HashItemViewModel
                        {
                            HashName = item.ToString(),
                        });
                    }
                    break;
                case CryptographyApi.BouncyCastle:
                    foreach (var item in Enum.GetValues(typeof(BouncyHashAlgorithim)).Cast<BouncyHashAlgorithim>().ToList())
                    {
                        Items.Add(new HashItemViewModel
                        {
                            HashName = item.ToString(),
                        });
                    }
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }

        #endregion
    }
}
