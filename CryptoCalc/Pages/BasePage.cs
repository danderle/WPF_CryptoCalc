using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BasePage : BaseUserControl
    {
    }

    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BasePage<VM> : BasePage
        where VM : BaseViewModel, new()
    {
        #region Private Member

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private VM viewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        public VM ViewModel
        {
            get { return viewModel; }
            set
            {
                // If nothing has changed, return
                if (viewModel == value)
                    return;

                // Update the value
                viewModel = value;

                // Set the data context for this page
                this.DataContext = viewModel;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePage()
        {
            // Create a default view model
            this.ViewModel = new VM();
        }

        
        #endregion
    }
}
