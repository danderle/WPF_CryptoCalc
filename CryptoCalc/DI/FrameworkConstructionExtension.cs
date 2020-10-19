using CryptoCalc.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoCalc
{
    /// <summary>
    /// Extension methods for the <see cref="FrameworkConstruction"/>
    /// </summary>
    public static class FrameworkConstructionExtensions
    {
        /// <summary>
        /// Injects the view models needed for CryptoCalc application
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction AddCryptoCalcViewModels(this FrameworkConstruction construction)
        {
            // Bind to a single instance of Application view model
            construction.Services.AddSingleton<ApplicationViewModel>();

            // Return the construction for chaining
            return construction;
        }

        /// <summary>
        /// Injects the CryptoCalc client application services needed
        /// for the Fasetto Word application
        /// </summary>
        /// <param name="construction"></param>
        /// <returns></returns>
        public static FrameworkConstruction AddCryptoCalcClientServices(this FrameworkConstruction construction)
        {

            // Bind a UI Manager
            construction.Services.AddTransient<IUIManager, UIManager>();

            // Return the construction for chaining
            return construction;
        }
    }
}