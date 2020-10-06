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
        /// The command method to show an error dialog window and displays the exception message to the user
        /// <param name="title">the title of the pop up dialog</param>
        /// <param name="dialogType">The type of <see cref="WindowDialogType"/></param>
        /// </summary>
        public static async void OpenErrorMessageBoxAsync(Exception exception, string title, WindowDialogType dialogType)
        {
            //Opens a pop up window folder browser dialog
            await Ioc.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Title = title,
                Message = exception.Message,
                OkText = "Continue",
                DialogType = dialogType
            });
        }
    }
}
