﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CryptoCalc.Core
{
    /// <summary>
    /// A interface for any asymmetric operations
    /// </summary>
    public interface IAsymmetricCipher
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
                case AsymmetricMsdnCiphers.ECDifiieHellman:
                    return new MsdnECDH();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Returns a Bouncy Castle cipher object
        /// </summary>
        /// <param name="selectedAlgorithim">The cipher algorithim to return</param>
        /// <returns>The selected cipher object</returns>
        public static IAsymmetricCipher GetBouncyCipher(string selectedAlgorithim)
        {
            var algorithim = (AsymmetricBouncyCiphers)Enum.Parse(typeof(AsymmetricBouncyCiphers), selectedAlgorithim);
            switch (algorithim)
            {
                case AsymmetricBouncyCiphers.RSA:
                    return new BouncyRsa();
                case AsymmetricBouncyCiphers.DSA:
                    return new MsdnDsa();
                case AsymmetricBouncyCiphers.ECDsa:
                    return new MsdnECDsa();
                case AsymmetricBouncyCiphers.ECDifiieHellman:
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
            switch(operation)
            {
                case AsymmetricOperation.Encryption:
                    return new List<string> { AsymmetricMsdnCiphers.RSA.ToString() };

                case AsymmetricOperation.Signature:
                    return new List<string> { AsymmetricMsdnCiphers.RSA.ToString(), AsymmetricMsdnCiphers.DSA.ToString(), AsymmetricMsdnCiphers.ECDsa.ToString() };

                case AsymmetricOperation.KeyExchange:
                    return new List<string> { AsymmetricMsdnCiphers.ECDifiieHellman.ToString() };
                default:
                    Debugger.Break();
                    return null;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes();

        /// <summary>
        /// Create a key pair for according to the key size
        /// </summary>
        /// <param name="keySize">the key size in bits</param>
        public void CreateKeyPair(int keySize);

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