using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class ECDiffieHellmanCipher : IAsymmetricCipher
    {
        #region Private Fields

        public ECDiffieHellman cipher { get; set; } = ECDiffieHellman.Create();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ECDiffieHellmanCipher()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes()
        {
            var keySizes = new ObservableCollection<int>();
            foreach (var legalkeySize in cipher.LegalKeySizes)
            {
                int keySize = legalkeySize.MinSize;
                while (keySize <= legalkeySize.MaxSize)
                {
                    keySizes.Add(keySize);
                    if (legalkeySize.SkipSize == 0)
                        break;
                    keySize += legalkeySize.SkipSize;
                }
            }
            return keySizes;
        }


        public byte[] EncryptText(byte[] publicKey, string plainText)
        {
            throw new NotImplementedException();
        }

        public byte[] EncryptBytes(string selectedAlgorithim, int keySize, byte[] plainBytes)
        {
            throw new NotImplementedException();
        }

        public string DecryptToText(byte[] privateKey, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public byte[] DecryptToBytes(string selectedAlgorithim, int keySize, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public void CreateKeyPair(int keySize)
        {
            
        }

        public byte[] Sign(byte[] privKey, byte[] data)
        {
            return null;
        }

        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            return false;
        }

        public byte[] GetPrivateKey()
        {
            throw new NotImplementedException();
        }

        public byte[] GetPublicKey()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
