using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// A shorthand access class to get DI services with nice clean short code
    /// </summary>
    public static class DI
    {
        /// <summary>
        /// A shortcut to access the <see cref="IUIManager"/>
        /// </summary>
        public static IUIManager UI => Framework.Service<IUIManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ApplicationViewModel"/>
        /// </summary>
        public static ApplicationViewModel Application => Framework.Service<ApplicationViewModel>();
    }
}
