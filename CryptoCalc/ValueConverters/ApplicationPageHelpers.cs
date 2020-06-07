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
                    return new HashPage(viewModel as HashViewModel);
                case ApplicationPage.SymmetricCiphers:
                    return new SymmetricCipherPage(viewModel as SymmetricCipherViewModel);
                case ApplicationPage.AsymmetricCiphers:
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Take in a <see cref="BasePage"/> and returns a <see cref="ApplicationPage"/>
        /// </summary>
        /// <param name="page">The type of page passed in</param>
        /// <returns>The page as an enum</returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            if (page is HashPage)
                return ApplicationPage.Hash;
            if (page is SymmetricCipherPage)
                return ApplicationPage.SymmetricCiphers;
            //Alert developer
            Debugger.Break();
            return default(ApplicationPage);
        }
    }
}
