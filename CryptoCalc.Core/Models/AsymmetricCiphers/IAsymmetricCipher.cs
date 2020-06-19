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
    public interface IAsymmetricCipher
    {
        #region Public Properties

        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }
        public bool AbleToEncrypt { get; }

        #endregion

        #region Public Static Methods
        
        public static IAsymmetricCipher GetCipher(AsymmetricMsdnCiphers cipher)
        {
            switch (cipher)
            {
                case AsymmetricMsdnCiphers.RSA:
                    return new RsaCipher();
                case AsymmetricMsdnCiphers.DSA:
                    return new DsaCipher();
                case AsymmetricMsdnCiphers.ECDsa:
                    return new ECDsaCipher();
                case AsymmetricMsdnCiphers.ECDifiieHellman:
                    return new RsaCipher();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public static List<string> GetMsdnAlgorthims()
        {
            return Enum.GetValues(typeof(AsymmetricMsdnCiphers)).Cast<AsymmetricMsdnCiphers>().Select(t => t.ToString()).ToList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes(int selectedAlgorithim)
        {
            var keySizes = new ObservableCollection<int>();
            
            var algorithim = (AsymmetricMsdnCiphers)selectedAlgorithim;
            var cipher = AsymmetricAlgorithm.Create(algorithim.ToString());
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


        public byte[] EncryptText(byte[] publicKey, string plainText);

        public byte[] EncryptBytes(int selectedAlgorithim, int keySize, byte[] password, byte[] plainBytes);

        public string DecryptToText(byte[] privateKey, byte[] encrypted);

        public byte[] DecryptToBytes(int selectedAlgorithim, int keySize, byte[] password, byte[] encrypted);

        public void CreateKeyPair(int keySize);

        public byte[] Sign(byte[] privKey, byte[] data);

        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data);

        #endregion
    }
}
