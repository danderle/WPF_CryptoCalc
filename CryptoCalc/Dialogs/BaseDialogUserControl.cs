using CryptoCalc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoCalc
{
    /// <summary>
    /// The base class for any content that is being used inside of a <see cref="DialogWindow"/>
    /// </summary>
    public class BaseDialogUserControl : UserControl
    {
        #region Private Fields

        /// <summary>
        /// The dialog window we will be contained within
        /// </summary>
        private DialogWindow dialogWindow;

        #endregion

        #region Public Properties

        /// <summary>
        /// the minimum width of this dialog
        /// </summary>
        public int WindowMinimumWidth { get; set; } = 250;

        /// <summary>
        /// The minimum height of this dialog
        /// </summary>
        public int WindowMinimumHeight { get; set; } = 100;

        /// <summary>
        /// The maximum width of this dialog window
        /// </summary>
        public int WindowMaximumWidth { get; set; } = 500;

        /// <summary>
        /// The maximum height of this dialog window
        /// </summary>
        public int WindowMaximumHeight { get; set; } = 800;

        /// <summary>
        /// The height of the title bar
        /// </summary>
        public int TitleHeight { get; set; } = 30;

        /// <summary>
        /// The title for this dialog window
        /// </summary>
        public string Title { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Closes the dialog
        /// </summary>
        public ICommand CloseCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDialogUserControl()
        {
            //create a new dialog window
            dialogWindow = new DialogWindow();
            dialogWindow.ViewModel = new DialogWindowViewModel(dialogWindow);

            CloseCommand = new RelayCommand(() => dialogWindow.Close());
        }

        #endregion

        #region Public Dialog Show Methods

        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <typeparam name="T">The view model type for this control</param>
        /// <returns></returns>
        public Task ShowMessage<T>(T viewModel)
            where T : BaseDialogViewModel
        {
            //Create a task to await the dialog closing
            var tcs = new TaskCompletionSource<bool>();

            //Run on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    //Set the default values for the dialog window
                    dialogWindow.MinWidth = WindowMinimumWidth;
                    dialogWindow.MinHeight = WindowMinimumHeight;
                    dialogWindow.MaxHeight = WindowMaximumHeight;
                    dialogWindow.MaxWidth = WindowMaximumWidth;
                    dialogWindow.ViewModel.TitleHeight = TitleHeight;
                    dialogWindow.ViewModel.BaseDialog.Title = string.IsNullOrEmpty(viewModel.Title) ? Title : viewModel.Title;

                    //Set the type of dialog to show
                    dialogWindow.ViewModel.BaseDialog.DialogType = viewModel.DialogType;

                    // Set this control to the dialog window content
                    dialogWindow.ViewModel.Content = this;

                    //Setup this controls datacontext binding to the view model
                    DataContext = viewModel;

                    //show dialog
                    dialogWindow.Owner = Application.Current.MainWindow;
                    dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    dialogWindow.ShowDialog();
                }
                finally
                {
                    //Let caller know we finished
                    tcs.TrySetResult(true);
                }
            });

            return tcs.Task;
        }

        #endregion
    }
}
