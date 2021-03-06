﻿using CryptoCalc.Core;

namespace CryptoCalc
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
        public ApplicationPage CurrentPage { get; private set; } = ApplicationPage.MSDNHash;

        /// <summary>
        /// The list of available pages view model
        /// </summary>
        public PageOptionListItemViewModel PageList { get; set; } = new PageOptionListItemViewModel();

        /// <summary>
        /// The view model to use for the current page when the CurrentPage changes
        /// </summary>
        public BaseViewModel CurrentPageViewModel { get; set; }

        /// <summary>
        /// Holds the file path which weas selected from the file path dialog
        /// </summary>
        public string FilePathFromDialogSelection { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page">The page to go to</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page</param>
        public void GoToPage(ApplicationPage page, BaseViewModel viewModel = null)
        {
            // Set the view model
            CurrentPageViewModel = viewModel;

            // Set the current page
            CurrentPage = page;

            // Fire off a currentPage changed event
            OnPropertyChanged(nameof(CurrentPage));
        }

        /// <summary>
        /// Closes the application
        /// </summary>
        /// <param name="exitCode">The exit code when closing the application</param>
        public void Close(int exitCode = 0)
        {
            System.Environment.Exit(exitCode);
        }

        #endregion
    }
}
