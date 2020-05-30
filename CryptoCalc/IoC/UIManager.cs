using CryptoCalc.Core;
using System.Threading.Tasks;

namespace CryptoCalc
{
    /// <summary>
    /// The applications implementation of the <see cref="IUIManager"/>
    /// </summary>
    public class UIManager : IUIManager
    {
        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowMessage(MessageBoxDialogViewModel viewModel)
        {
            return new DialogMessageBox().ShowMessage(viewModel);
        }

        /// <summary>
        /// Displays a single folder broser dialog to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <returns></returns>
        public Task ShowFolderDialog(FolderBrowserDialogViewModel viewModel)
        {
            return new FolderBrowserDialog().ShowMessage(viewModel);
        }
    }
}
