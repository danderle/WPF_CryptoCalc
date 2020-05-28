using CryptoCalc.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoCalc
{
    /// <summary>
    /// The view model for the custom window
    /// </summary>
    public class DialogWindowViewModel : WindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The title of this dialog window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The content to host inside the dialog window
        /// </summary>
        public Control Content { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DialogWindowViewModel(Window _window) : base(_window) 
        {
            //Set minimum size of window
            MinimumHeight = 100;
            MinimumWidth = 250;
        }
        
        #endregion

    }
}
