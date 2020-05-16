namespace CryptoCalc.Core
{
    public class HashViewModel : BaseViewModel
    {

        #region Properties

        /// <summary>
        /// The view model to setup the hashing process
        /// </summary>
        public DataFormatViewModel DataFormatSetup { get; set; } = new DataFormatViewModel();


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashViewModel()
        {
        } 

        #endregion
    }
}
