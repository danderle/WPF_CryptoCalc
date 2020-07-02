using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class RsaCipher : IAsymmetricCipher
    {
        #region Private Fields

        private RSACryptoServiceProvider cipher { get; set; } = new RSACryptoServiceProvider();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RsaCipher()
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
            int bytesRead;
            cipher.ImportRSAPublicKey(publicKey, out bytesRead);
            var plain = ByteConvert.StringToAsciiBytes(plainText);
            return cipher.Encrypt(plain, false);
        }

        public byte[] EncryptBytes(string selectedAlgorithim, int keySize, byte[] plainBytes)
        {
            throw new NotImplementedException();
        }

        public string DecryptToText(byte[] privateKey, byte[] encrypted)
        {
            int byteRead;
            cipher.ImportRSAPrivateKey(privateKey, out byteRead);
            var decrypted = cipher.Decrypt(encrypted, false);
            return ByteConvert.BytesToAsciiString(decrypted);
        }

        public byte[] DecryptToBytes(string selectedAlgorithim, int keySize, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public void CreateKeyPair(int keySize)
        {
            cipher = new RSACryptoServiceProvider(keySize);
        }

        public byte[] Sign(byte[] privKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportRSAPrivateKey(privKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportRSAPublicKey(pubKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.VerifyHash(hash, originalSignature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        public byte[] GetPrivateKey()
        {
            return cipher.ExportRSAPrivateKey();
        }

        public byte[] GetPublicKey()
        {
            return cipher.ExportRSAPublicKey();
        }

        public byte[] DeriveKey(byte[] myPrivateKey, int cipherKeySize, byte[] otherPartyPublicKey)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
