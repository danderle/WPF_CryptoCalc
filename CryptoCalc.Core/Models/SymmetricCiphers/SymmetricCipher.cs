using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;

namespace CryptoCalc.Core
{
    static class SymmetricCipher
    {
        #region Public Properties

        public static Aes AesCrypt { get; set; } 

        #endregion

        #region Constructor

        /// <summary>
        /// Default static constructor
        /// </summary>
        static SymmetricCipher()
        {
            AesCrypt = Aes.Create();
            var bsize = AesCrypt.BlockSize;
            var fsize = AesCrypt.FeedbackSize;
            var ksize = AesCrypt.KeySize;
            var lsize = AesCrypt.LegalBlockSizes;
            var msize = AesCrypt.LegalKeySizes;
        }

        #endregion

        #region Public Methods

        public static byte[] AesEncrypt(byte[] password, int keySize, string plainText)
        {
            SetKeyAndIV(password, keySize);
            return Encrypt(plainText, AesCrypt.Key, AesCrypt.IV);
        }

        public static byte[] AesEncrypt(byte[] password, int keySize, byte[] plainText)
        {
            SetKeyAndIV(password, keySize);
            return Encrypt(plainText, AesCrypt.Key, AesCrypt.IV);
        }

        public static string AesDecryptToText(byte[] password, int keySize, byte[] encrypted)
        {
            SetKeyAndIV(password, keySize);
            return DecryptToText(encrypted, AesCrypt.Key, AesCrypt.IV);
        }

        public static byte[] AesDecryptToByte(byte[] password, int keySize, byte[] encrypted)
        {
            SetKeyAndIV(password, keySize);
            return DecryptToByte(encrypted, AesCrypt.Key, AesCrypt.IV);
        }

        #endregion

        #region Private Methods

        private static string DecryptToText(byte[] encrypted, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (encrypted == null || encrypted.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
        private static byte[] DecryptToByte(byte[] encrypted, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (encrypted == null || encrypted.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iV");

            // Declare the string used to hold
            // the decrypted text.
            byte[] plainBytes = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        using(var binWriter = new BinaryWriter(csDecrypt))
                        {
                            binWriter.Write(encrypted);
                        }
                        plainBytes = msDecrypt.ToArray();
                    }
                }
            }
            return plainBytes;
        }

        private static byte[] Encrypt(string plain, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plain == null || plain.Length <= 0)
                throw new ArgumentNullException("plain");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plain);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        private static byte[] Encrypt(byte[] plain, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plain == null || plain.Length <= 0)
                throw new ArgumentNullException("plain");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plain);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        private static void SetKeyAndIV(byte[] password, int keySize)
        {
            AesCrypt.KeySize = keySize;
            byte[] hash = null;
            byte[] key = null;
            byte[] iv = new byte[16];
            switch (keySize)
            {
                case 128:
                    key = new byte[16];
                    hash = Hash.Compute(HashAlgorithim.SHA256, password);
                    Array.Copy(hash, key, 16);
                    Array.Copy(hash, key.Length, iv, 0, 16);
                    AesCrypt.Key = key;
                    AesCrypt.IV = iv;
                    break;
                case 192:
                    key = new byte[24];
                    hash = Hash.Compute(HashAlgorithim.SHA384, password);
                    Array.Copy(hash, key, 24);
                    Array.Copy(hash, key.Length, iv, 0, 16);
                    AesCrypt.Key = key;
                    AesCrypt.IV = iv;
                    break;
                case 256:
                    key = new byte[32];
                    hash = Hash.Compute(HashAlgorithim.SHA512, password);
                    Array.Copy(hash, key, 32);
                    Array.Copy(hash, key.Length, iv, 0, 16);
                    AesCrypt.Key = key;
                    AesCrypt.IV = iv;
                    break;
                default:
                    Debugger.Break();
                    break;
            }
        }

        #endregion
    }
}
