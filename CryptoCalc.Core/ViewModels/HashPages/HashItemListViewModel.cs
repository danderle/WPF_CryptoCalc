﻿using System;
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
            
        }

        public HashItemListViewModel(bool getBouncyHashes)
        {
            
            if(getBouncyHashes)
            {
                foreach (var item in Enum.GetValues(typeof(BouncyHashAlgorithim)).Cast<BouncyHashAlgorithim>().ToList())
                {
                    Items.Add(new HashItemViewModel
                    {
                        HashName = item.ToString(),
                    });
                }
            }
            else
            {
                foreach (var item in Enum.GetValues(typeof(MsdnHashAlgorithim)).Cast<MsdnHashAlgorithim>().ToList())
                {
                    Items.Add(new HashItemViewModel
                    {
                        HashName = item.ToString(),
                    });
                }
            }
            
        }

        #endregion
    }
}