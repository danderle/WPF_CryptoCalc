using CryptoCalc.Core;

namespace CryptoCalc
{

    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BasePage : BaseUserControl
    {
        #region Private Member

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private object viewModelObject;

        #endregion

        #region Public Properties

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        public object ViewModelObject
        {
            get { return viewModelObject; }
            set
            {
                // If nothing has changed, return
                if (viewModelObject == value)
                    return;

                // Update the value
                viewModelObject = value;

                // Set the data context for this page
                DataContext = viewModelObject;
            }
        }

        #endregion
    }

    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BasePage<VM> : BasePage
        where VM : BaseViewModel, new()
    {
        #region Public Properties

        /// <summary>
        /// The view model associated with this page
        /// </summary>
        public VM ViewModel
        {
            get => (VM)ViewModelObject;
            set => ViewModelObject = value;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePage() : base()
        {
            // Create a default view model
            ViewModel = Ioc.Get<VM>();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public BasePage(VM specificViewModel = null) : base()
        {
            // Set specific view model
            if(specificViewModel != null)
            {
                ViewModel = specificViewModel;
            }
            else
            {
                //Create a default view model
                ViewModel = Ioc.Get<VM>();
            }
        }
        #endregion
    }
}
