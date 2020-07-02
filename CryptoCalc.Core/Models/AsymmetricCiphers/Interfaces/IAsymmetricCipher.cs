using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CryptoCalc.Core
{
    public interface IAsymmetricCipher
    {
        #region Public Static Methods
        
        public static IAsymmetricCipher GetMsdnCipher(string selectedAlgorithim)
        {
            var algorithim = (AsymmetricMsdnCiphers)Enum.Parse(typeof(AsymmetricMsdnCiphers), selectedAlgorithim);
            switch (algorithim)
            {
                case AsymmetricMsdnCiphers.RSA:
                    return new RsaCipher();
                case AsymmetricMsdnCiphers.DSA:
                    return new DsaCipher();
                case AsymmetricMsdnCiphers.ECDsa:
                    return new ECDsaCipher();
                case AsymmetricMsdnCiphers.ECDifiieHellman:
                    return new ECDiffieHellmanCipher();
                default:
                    Debugger.Break();
                    return null;
            }
        }

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

        public void CreateKeyPair(int keySize);

        public byte[] GetPrivateKey();

        public byte[] GetPublicKey();


        #endregion
    }
}
