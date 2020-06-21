using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class ECDsaCipher : IAsymmetricCipher
    {
        #region Private Fields

        public ECDsa cipher { get; set; }

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
        public ECDsaCipher()
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
            cipher = ECDsa.Create("ECDsa");
            cipher.KeySize = keySize;
            PrivateKey = cipher.ExportPkcs8PrivateKey();
            PublicKey = cipher.ExportSubjectPublicKeyInfo();
        }

        public byte[] Sign(byte[] privKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportPkcs8PrivateKey(privKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);
            return cipher.SignHash(hash);
        }

        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportSubjectPublicKeyInfo(pubKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);
            return cipher.VerifyHash(hash, originalSignature);
        }

        #endregion
    }
}
