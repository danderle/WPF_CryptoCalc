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
            Message = "Design Time Message Box";
            OkText = "The ok Button";
            Title = "Desgin Time";
        } 

        #endregion
    }
}
