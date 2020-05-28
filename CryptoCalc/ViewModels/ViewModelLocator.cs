using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Locates view models from the Ioc container for binding in the xaml files
    /// </summary>
    public class ViewModelLocator
    {
        #region Public Propeties
        
        /// <summary>
        /// Singleton instance of the locator
        /// </summary>
        public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

        /// <summary>
        /// The application view model
        /// </summary>
        public static ApplicationViewModel ApplicationViewModel => Ioc.Get<ApplicationViewModel>();

        #endregion
    }
}
