using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class DsaCipher : IAsymmetricCipher
    {
        #region Private Fields

        public DSACryptoServiceProvider cipher { get; set; } = new DSACryptoServiceProvider();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DsaCipher()
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
            cipher = new DSACryptoServiceProvider(keySize);
        }

        public byte[] Sign(byte[] privKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportPkcs8PrivateKey(privKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.SignHash(hash, HashAlgorithmName.SHA1.ToString());
        }

        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportSubjectPublicKeyInfo(pubKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.VerifyHash(hash,  HashAlgorithmName.SHA1.ToString(), originalSignature);
        }

        public byte[] GetPrivateKey()
        {
            return cipher.ExportPkcs8PrivateKey();
        }

        public byte[] GetPublicKey()
        {
            return cipher.ExportSubjectPublicKeyInfo();
        }

        public byte[] DeriveKey(byte[] myPrivateKey, int cipherKeySize, byte[] otherPartyPublicKey)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
