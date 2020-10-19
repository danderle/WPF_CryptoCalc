using CryptoCalc.Core;
using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// The view model for the custom window
    /// </summary>
    public class DialogWindowViewModel : WindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The maximum height of this dialog
        /// </summary>
        public int MaximumHeight { get; set; } = 800;

        /// <summary>
        /// The maximum width of this dialog window
        /// </summary>
        public int MaximumWidth { get; set; } = 600;

        /// <summary>
        /// The content to host inside the dialog window
        /// </summary>
        public Control Content { get; set; }

        /// <summary>
        /// The base dialog object
        /// </summary>
        public BaseDialogViewModel BaseDialog { get; set; } = new BaseDialogViewModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DialogWindowViewModel(Window _window) : base(_window) 
        {
        }
        
        #endregion

    }
}
