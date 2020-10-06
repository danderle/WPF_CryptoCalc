using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoCalc.Core
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
            await Ioc.UI.ShowMessage(new MessageBoxDialogViewModel
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
            await Ioc.UI.ShowFolderDialog(new FolderBrowserDialogViewModel());
        }

        /// <summary>
        /// Opens a help message box dialog
        /// </summary>
        public static async void OpenHelpMessageBoxAsync()
        {
            //TODO Help dialog window
            await Ioc.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Message = "Hello this is a pop up message",
                Title = "First dialog message",
                OkText = "Press ok to continue",
            });
        }
    }
}
