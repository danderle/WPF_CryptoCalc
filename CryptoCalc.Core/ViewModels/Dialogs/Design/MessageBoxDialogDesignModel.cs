namespace CryptoCalc.Core
{
    public class MessageBoxDialogDesignModel : MessageBoxDialogViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static MessageBoxDialogDesignModel Instance => new MessageBoxDialogDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MessageBoxDialogDesignModel()
        {
            DialogType = WindowDialogType.FolderBrowser;
            Message = "Encryption failed. The plain byte length is greater than the selected key size. The key size must greater than the plain bytes   ";
            OkText = "Continue";
            Title = "Encryption Failure";
        } 

        #endregion
    }
}
