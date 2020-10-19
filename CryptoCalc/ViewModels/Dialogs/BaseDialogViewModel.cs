using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// A base view model for any dialogs
    /// </summary>
    public class BaseDialogViewModel : BaseViewModel
    {
        /// <summary>
        /// The dialog title text
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The type of dialog
        /// </summary>
        public WindowDialogType DialogType { get; set; }
    }
}
