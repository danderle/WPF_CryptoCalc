using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Org.BouncyCastle.Math.EC;

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

        public byte[] DeriveKey(byte[] myPrivateKey, int cipherKeySize, byte[] otherPartyPublicKey)
        {
            var myDiffie = ECDiffieHellman.Create( );
            int bytesRead;
            myDiffie.KeySize = cipherKeySize;
            
            myDiffie.ImportPkcs8PrivateKey(myPrivateKey, out bytesRead);
            var otherCipher = ECDiffieHellman.Create();
            otherCipher.KeySize = cipherKeySize;
            otherCipher.ImportSubjectPublicKeyInfo(otherPartyPublicKey, out bytesRead);
            return myDiffie.DeriveKeyMaterial(otherCipher.PublicKey);
        }

        public byte[] EncryptText(byte[] publicKey, string plainText)
        {
            var cipher2 = ECDiffieHellman.Create();
            cipher2.KeySize = cipher.KeySize;
            var pubKey2 = cipher2.ExportSubjectPublicKeyInfo();
            var cipher3 = ECDiffieHellman.Create();
            cipher3.KeySize = cipher.KeySize;
            int bytesRead;
            cipher3.ImportSubjectPublicKeyInfo(publicKey, out bytesRead);
            var cipher2Key = cipher2.DeriveKeyMaterial(cipher3.PublicKey);
            byte[] encryptedMessage;
            byte[] iv;
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = cipher2Key;
                iv = aes.IV;
                // Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
            byte[] plaintextMessage = Encoding.UTF8.GetBytes(plainText);

                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    encryptedMessage = ciphertext.ToArray();
                }
            }
            using (Aes aes = new AesCryptoServiceProvider())
            {
                cipher3.ImportSubjectPublicKeyInfo(pubKey2, out bytesRead);
                aes.Key = cipher.DeriveKeyMaterial(cipher3.PublicKey);
                aes.IV = iv;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        string message = Encoding.UTF8.GetString(plaintext.ToArray());
                        Console.WriteLine(message);
                    }
                }
            }
            return iv;
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
            cipher = ECDiffieHellman.Create();
            cipher.KeySize = keySize;
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
            return cipher.ExportPkcs8PrivateKey();
        }

        public byte[] GetPublicKey()
        {
            return cipher.ExportSubjectPublicKeyInfo();
        }

        #endregion
    }
}
