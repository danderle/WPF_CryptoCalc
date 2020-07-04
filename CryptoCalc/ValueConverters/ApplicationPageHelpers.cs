using CryptoCalc.Core;
using System.Diagnostics;

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
                case ApplicationPage.MSDNHash:
                    return new MsdnHashPage(viewModel == null ? new HashViewModel(CryptographyApi.MSDN) : viewModel as HashViewModel);
                case ApplicationPage.MSDNSymmetricEncryption:
                    return new MsdnSymmetricPage(viewModel == null ? new SymmetricViewModel(CryptographyApi.MSDN) : viewModel as SymmetricViewModel);
                case ApplicationPage.MSDNPublicKeyEncryption:
                    return new MsdnPkEncryptionPage(viewModel == null ? new AsymmetricViewModel(CryptographyApi.MSDN, AsymmetricOperation.Encryption) : viewModel as AsymmetricViewModel);
                case ApplicationPage.MSDNDigitalSignature:
                    return new MsdnPkSignaturePage(viewModel == null ? new AsymmetricViewModel(CryptographyApi.MSDN, AsymmetricOperation.Signature) : viewModel as AsymmetricViewModel);
                case ApplicationPage.MSDNKeyExchange:
                    return new MsdnPkKeyExchangePage(viewModel == null ? new AsymmetricViewModel(CryptographyApi.MSDN, AsymmetricOperation.KeyExchange) : viewModel as AsymmetricViewModel);
                case ApplicationPage.BouncyCastleHash:
                    return new BouncyHashPage(viewModel == null ? new HashViewModel(CryptographyApi.BouncyCastle) : viewModel as HashViewModel);
                case ApplicationPage.BouncyCastleSymmetricEncryption:
                    return new BouncySymmetricPage(viewModel == null ? new SymmetricViewModel(CryptographyApi.BouncyCastle) : viewModel as SymmetricViewModel);
                case ApplicationPage.BouncyCastlePublicKeyEncryption:
                    return new BouncyPkEncryptionPage(viewModel == null ? new AsymmetricViewModel(CryptographyApi.BouncyCastle, AsymmetricOperation.Encryption) : viewModel as AsymmetricViewModel);
                case ApplicationPage.BouncyCastleDigitalSignature:
                case ApplicationPage.BouncyCastleKeyExchange:
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
            if (page is MsdnHashPage)
                return ApplicationPage.MSDNHash;
            if (page is MsdnSymmetricPage)
                return ApplicationPage.MSDNSymmetricEncryption;
            if (page is MsdnPkEncryptionPage)
                return ApplicationPage.MSDNPublicKeyEncryption;
            if (page is MsdnPkSignaturePage)
                return ApplicationPage.MSDNDigitalSignature;
            if (page is MsdnPkKeyExchangePage)
                return ApplicationPage.MSDNKeyExchange;
            if (page is BouncyHashPage)
                return ApplicationPage.BouncyCastleHash;
            if (page is BouncySymmetricPage)
                return ApplicationPage.BouncyCastleSymmetricEncryption;
            if (page is BouncyPkEncryptionPage)
                return ApplicationPage.BouncyCastlePublicKeyEncryption;
            //if (page is MsdnHashPage)
            //    return ApplicationPage.BouncyCastleDigitalSignature;
            //if (page is MsdnHashPage)
            //    return ApplicationPage.BouncyCastleKeyExchange;

            //Alert developer
            Debugger.Break();
            return default(ApplicationPage);
        }
    }
}
