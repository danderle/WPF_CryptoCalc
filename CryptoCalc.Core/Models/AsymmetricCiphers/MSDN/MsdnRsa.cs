using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    public class MsdnRsa : IAsymmetricEncryption, IAsymmetricSignature
    {
        #region Private Fields

        private RSACryptoServiceProvider cipher { get; set; } = new RSACryptoServiceProvider();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnRsa()
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
            var plain = ByteConvert.StringToAsciiBytes(plainText);
            return EncryptBytes(publicKey, plain);
        }

        public byte[] EncryptBytes(byte[] publicKey, byte[] plainBytes)
        {
            int bytesRead;
            cipher.ImportRSAPublicKey(publicKey, out bytesRead);
            return cipher.Encrypt(plainBytes, false);
        }

        public string DecryptToText(byte[] privateKey, byte[] encrypted)
        {
            var decrypted = DecryptToBytes(privateKey, encrypted);
            return ByteConvert.BytesToAsciiString(decrypted);
        }

        public byte[] DecryptToBytes(byte[] privateKey, byte[] encrypted)
        {
            int byteRead;
            cipher.ImportRSAPrivateKey(privateKey, out byteRead); 
            return cipher.Decrypt(encrypted, false);
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

        #endregion
    }
}
