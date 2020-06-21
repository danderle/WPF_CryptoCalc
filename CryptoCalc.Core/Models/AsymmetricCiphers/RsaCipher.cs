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
    public class RsaCipher : IAsymmetricCipher
    {
        #region Private Fields

        public RSACryptoServiceProvider cipher { get; set; }

        #endregion

        #region Public Properties

        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public bool AbleToEncrypt => true;
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

        public byte[] EncryptBytes(int selectedAlgorithim, int keySize, byte[] password, byte[] plainBytes)
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

        public byte[] DecryptToBytes(int selectedAlgorithim, int keySize, byte[] password, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public void CreateKeyPair(int keySize)
        {
            cipher = new RSACryptoServiceProvider(keySize);
            PrivateKey = cipher.ExportRSAPrivateKey();
            PublicKey = cipher.ExportRSAPublicKey();
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

        #endregion
    }
}
