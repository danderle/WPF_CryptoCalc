using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using Org.BouncyCastle;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Security;
using System.Linq;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace CryptoCalc.Core
{
    public class DsaCipher : IAsymmetricCipher
    {
        #region Private Fields

        public DSACryptoServiceProvider cipher { get; set; }

        #endregion

        #region Public Properties

        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public bool AbleToEncrypt => false;
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

        public byte[] EncryptBytes(int selectedAlgorithim, int keySize, byte[] password, byte[] plainBytes)
        {
            throw new NotImplementedException();
        }

        public string DecryptToText(byte[] privateKey, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public byte[] DecryptToBytes(int selectedAlgorithim, int keySize, byte[] password, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public void CreateKeyPair(int keySize)
        {
            cipher = new DSACryptoServiceProvider(keySize);
            PrivateKey = cipher.ExportPkcs8PrivateKey();
            PublicKey = cipher.ExportSubjectPublicKeyInfo();
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

        #endregion
    }
}
