using CryptoCalc.Core.Models;
using System;
using System.Text;
using System.ComponentModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the <see cref="HashItemControl.xaml"/>
    /// </summary>
    public class HashItemViewModel : BaseViewModel
    {
        #region Private Fields

        /// <summary>
        /// The hash algorithim to use when calculating the hash value
        /// </summary>
        private HashAlgorithim hashAlgorithim => (HashAlgorithim)Enum.Parse(typeof(HashAlgorithim), HashName);

        #endregion

        #region Properties

        /// <summary>
        /// True if the hash algorithim is checked
        /// </summary>
        public bool IsChecked { get; set; } = false;

        /// <summary>
        /// The hash algorithims name
        /// </summary>
        public string HashName { get; set; }

        /// <summary>
        /// The hash value
        /// </summary>
        public string HashValue { get; set; } = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashItemViewModel()
        {
        }

        #endregion

        #region Public Methods

        public void CalculateHash(byte[] data)
        {
            var value = Hash.Compute(hashAlgorithim, data);
            HashValue = BitConverter.ToString(value).Replace("-", string.Empty);
        }

        #endregion
    }
}
