using System.Collections.Generic;

namespace CryptoCalc.Core
{
    public class HashItemListDesignModel : HashItemListViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static HashItemListDesignModel Instance => new HashItemListDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashItemListDesignModel()
        {
            Items = new List<HashItemViewModel>
            {
                new HashItemViewModel
                {
                    IsChecked = true,
                    HashName = "SHA1",
                    HashValue = "444ffff444f4f4aaa3",
                },
                new HashItemViewModel
                {
                    IsChecked = true,
                    HashName = "SHA2",
                    HashValue = "juzjzuesf3444f4f4aaa3",
                },
                new HashItemViewModel
                {
                    IsChecked = true,
                    HashName = "SHA3",
                    HashValue = "wfew4t564uh",
                },
                new HashItemViewModel
                {
                    IsChecked = true,
                    HashName = "CRC32",
                    HashValue = "7ijhefg234",
                },
                new HashItemViewModel
                {
                    IsChecked = true,
                    HashName = "MD2",
                    HashValue = "g65h",
                },
                new HashItemViewModel
                {
                    IsChecked = true,
                    HashName = "Whirlpool",
                    HashValue = "876i8kjhg",
                },

            };
        } 

        #endregion
    }
}
