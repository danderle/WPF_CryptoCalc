using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the <see cref="FolderBrowserDialogViewModel.xaml"/>
    /// </summary>
    public class FolderBrowserDialogViewModel : BaseDialogViewModel
    {
        #region Public Properties

        /// <summary>
        /// The message to display
        /// </summary>
        public string Message => "Select file and press the \"Continue\" button";

        /// <summary>
        /// The text to use for the ok button
        /// </summary>
        public string OkText => "Continue";

        /// <summary>
        /// The path to the selected file
        /// </summary>
        public string SelectedFilePath { get; set; }

        /// <summary>
        /// Get all the logical drives
        /// </summary>
        public TreeViewModel FolderDialogTree { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to execute when a file is selected
        /// </summary>
        public ICommand FileSelectedCommand { get; set; }

        /// <summary>
        /// The command to execute when the continue button is pressed
        /// </summary>
        public ICommand ContinueCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FolderBrowserDialogViewModel()
        {
            FileSelectedCommand = new RelayParameterizedCommand(FileSelected);
                 
            FolderDialogTree = new TreeViewModel(FileSelectedCommand);

            Title = "Folder Browser";
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The file selected command method, which saves the files path to the <see cref="HmacViewModel"/>
        /// </summary>
        /// <param name="obj"></param>
        private void FileSelected(object obj)
        {
            SelectedFilePath = (string)obj;
            Ioc.Application.FilePathFromDialogSelection = SelectedFilePath;
        }

        #endregion
    }
}
