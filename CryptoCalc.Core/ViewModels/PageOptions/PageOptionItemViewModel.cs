using System;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for a page option item
    /// </summary>
    public class PageOptionItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Flag to know if the page option is currently checked
        /// </summary>
        public bool IsChecked{ get; set; }

        /// <summary>
        /// The page name
        /// </summary>
        public string PageName { get; set; }

        /// <summary>
        /// The page type
        /// </summary>
        public ApplicationPage SelectedPage { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command when the page is selected
        /// </summary>
        public ICommand PageSelectedCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PageOptionItemViewModel()
        {
            //Initialize the commands
            PageSelectedCommand = new RelayCommand(PageSelected);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method when selecting a page
        /// </summary>
        private void PageSelected()
        {
            //convert the page name to the page enum
            SelectedPage = (ApplicationPage)Enum.Parse(typeof(ApplicationPage), PageName.Replace(" ", ""));

            //true if not already selected then go to the next page
            if(Ioc.Application.CurrentPage != SelectedPage)
            {
                Ioc.Application.GoToPage(SelectedPage);
            }
        }

        #endregion
    }
}
