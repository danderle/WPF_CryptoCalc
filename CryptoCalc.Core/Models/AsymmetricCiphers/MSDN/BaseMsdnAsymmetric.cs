using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    public class BaseMsdnAsymmetric
    {
        #region Public Static Methods

        /// <summary>
        /// Returns a MSDN cipher object
        /// </summary>
        /// <param name="selectedAlgorithim">the cipher oparation to return</param>
        /// <returns>The selected cipher object</returns>
        public static IAsymmetricCipher GetMsdnCipher(string selectedAlgorithim)
        {
            var algorithim = (AsymmetricMsdnCiphers)Enum.Parse(typeof(AsymmetricMsdnCiphers), selectedAlgorithim);
            switch (algorithim)
            {
                case AsymmetricMsdnCiphers.RSA:
                    return new MsdnRsa();
                case AsymmetricMsdnCiphers.DSA:
                    return new MsdnDsa();
                case AsymmetricMsdnCiphers.ECDsa:
                    return new MsdnECDsa();
                case AsymmetricMsdnCiphers.ECDiffieHellman:
                    return new MsdnECDH();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Gets a list of possible MSDN asymmetric cipher algorithims accoridng to a selected operation
        /// </summary>
        /// <param name="operation">The type of operation</param>
        /// <returns>The list of cipher algorithims according the selected operation</returns>
        public static List<string> GetMsdnAlgorthims(AsymmetricOperation operation)
        {
            switch (operation)
            {
                case AsymmetricOperation.Encryption:
                    return new List<string> { AsymmetricMsdnCiphers.RSA.ToString() };

                case AsymmetricOperation.Signature:
                    return new List<string>
                    {
                        AsymmetricMsdnCiphers.ECDsa.ToString(),
                        AsymmetricMsdnCiphers.RSA.ToString(),
                        AsymmetricMsdnCiphers.DSA.ToString(),
                    };

                case AsymmetricOperation.KeyExchange:
                    return new List<string>
                    {
                        AsymmetricMsdnCiphers.ECDiffieHellman.ToString()
                    };
                default:
                    Debugger.Break();
                    return null;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a a dictionary for all the available msdn ec curves
        /// </summary>
        protected Dictionary<string, ECCurve> GetAllAvailableMsdnEcCurves()
        {
            Dictionary<string, ECCurve> ecCurves = new Dictionary<string, ECCurve>
            {
                { nameof(ECCurve.NamedCurves.brainpoolP160r1), ECCurve.NamedCurves.brainpoolP160r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP160t1), ECCurve.NamedCurves.brainpoolP160t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP192r1), ECCurve.NamedCurves.brainpoolP192r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP192t1), ECCurve.NamedCurves.brainpoolP192t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP224r1), ECCurve.NamedCurves.brainpoolP224r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP224t1), ECCurve.NamedCurves.brainpoolP224t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP256r1), ECCurve.NamedCurves.brainpoolP256r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP256t1), ECCurve.NamedCurves.brainpoolP256t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP320r1), ECCurve.NamedCurves.brainpoolP320r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP320t1), ECCurve.NamedCurves.brainpoolP320t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP384r1), ECCurve.NamedCurves.brainpoolP384r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP384t1), ECCurve.NamedCurves.brainpoolP384t1 },
                { nameof(ECCurve.NamedCurves.brainpoolP512r1), ECCurve.NamedCurves.brainpoolP512r1 },
                { nameof(ECCurve.NamedCurves.brainpoolP512t1), ECCurve.NamedCurves.brainpoolP512t1 },
                { nameof(ECCurve.NamedCurves.nistP256), ECCurve.NamedCurves.nistP256 },
                { nameof(ECCurve.NamedCurves.nistP384), ECCurve.NamedCurves.nistP384 },
                { nameof(ECCurve.NamedCurves.nistP521), ECCurve.NamedCurves.nistP521 }
            };
            return ecCurves;
        }

        #endregion
    }
}
