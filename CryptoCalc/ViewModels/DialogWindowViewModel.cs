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
        public int MaximumHeight { get; set; } = 600;

        /// <summary>
        /// The maximum width of this dialog window
        /// </summary>
        public int MaximumWidth { get; set; } = 800;

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
        }
        
        #endregion

    }
}
