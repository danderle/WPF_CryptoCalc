using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CryptoCalc.Core
{
    /// <summary>
    /// A interface for any asymmetric operations
    /// </summary>
    public interface IAsymmetricCipher
    {
        #region Public Methods

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey();

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey();

        #endregion
    }
}
