namespace CryptoCalc.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; private set; } = ApplicationPage.Hash;

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page"></param>
        public void GoToPage(ApplicationPage page)
        {
            CurrentPage = page;
        } 

        #endregion
    }
}
