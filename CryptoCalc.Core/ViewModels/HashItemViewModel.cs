namespace CryptoCalc.Core
{
    public class HashItemViewModel : BaseViewModel
    {

        #region Properties

        /// <summary>
        /// True if the hash algorithim is checked
        /// </summary>
        public bool IsChecked { get; set; }

        /// <summary>
        /// The hash algorithims name
        /// </summary>
        public string HashName { get; set; }

        /// <summary>
        /// The hash value
        /// </summary>
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
