using CryptoCalc.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageHelpers
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                case ApplicationPage.Hash:
                case ApplicationPage.SymmetricCiphers:
                case ApplicationPage.AsymmetricCiphers:
                    return new HashPage(viewModel as HashViewModel);
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            if (page is HashPage)
                return ApplicationPage.Hash;

            //Alert developer
            Debugger.Break();
            return default(ApplicationPage);
        }
    }
}
