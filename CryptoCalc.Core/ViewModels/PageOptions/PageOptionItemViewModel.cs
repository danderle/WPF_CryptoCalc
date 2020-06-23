using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the page option items
    /// </summary>
    public class PageOptionItemViewModel : BaseViewModel
    {
        #region Public Properties

        public bool IsChecked{ get; set; } 
        public string PageName { get; set; }
        public ApplicationPage SelectedPage { get; set; }

        #endregion

        #region Commands

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

        private void PageSelected()
        {
            var api = PageName.Contains("MSDN") ? CryptographyApi.MSDN : CryptographyApi.BouncyCastle;
            SelectedPage = (ApplicationPage)Enum.Parse(typeof(ApplicationPage), PageName.Replace(" ", ""));
           
            Ioc.Application.GoToPage(SelectedPage);
        }

        #endregion

        #region Private Methods



        #endregion
    }
}
