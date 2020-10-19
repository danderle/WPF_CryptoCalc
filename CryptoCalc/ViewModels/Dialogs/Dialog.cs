using CryptoCalc.Core;
using System;

namespace CryptoCalc
{
    /// <summary>
    /// static class for displaying different dialogs
    /// </summary>
    public static class Dialog
    {
        /// <summary>
        /// Static constructor
        /// </summary>
        static Dialog()
        {
        }

        /// <summary>
        /// Opens an error message box
        /// <param name="title">the title of the pop up dialog</param>
        /// <param name="dialogType">The type of <see cref="WindowDialogType"/></param>
        /// </summary>
        public static async void OpenErrorMessageBoxAsync(Exception exception, string title, WindowDialogType dialogType)
        {
            //Opens a pop up message box dialog
            await DI.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = title,
                Message = exception.Message,
                OkText = "Continue",
                DialogType = dialogType
            });
        }

        /// <summary>
        /// Opens a folder browser dialog
        /// </summary>
        public static async void OpenFolderBrowserAsync()
        { 
            //Opens a pop up window folder browser dialog
            await DI.UI.ShowFolderDialog(new FolderBrowserDialogViewModel());
        }

        /// <summary>
        /// Opens a help message box dialog
        /// </summary>
        public static async void OpenHelpMessageBoxAsync()
        {
            //TODO Help dialog window
            await DI.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Message = "Hello this is a pop up message",
                Title = "First dialog message",
                OkText = "Press ok to continue",
            });
        }
    }
}
