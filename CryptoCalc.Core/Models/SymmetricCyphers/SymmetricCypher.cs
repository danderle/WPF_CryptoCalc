using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CryptoCalc.Core
{
    static class SymmetricCypher
    {
        private static byte[] aeskey = null;
        private static byte[] aesiv = null;

        #region Constructor

        /// <summary>
        /// Default static constructor
        /// </summary>
        static SymmetricCypher()
        {
            using (var aes = Aes.Create())
            {
                aeskey = aes.Key;
                aesiv = aes.IV;
            }
        }

        #endregion

        #region Public Methods

        public static byte[] AesEncrypt(string plainText)
        {
            return Encrypt(plainText, aeskey, aesiv);
        }

        public static byte[] AesEncrypt(byte[] plainText)
        {
            return Encrypt(plainText, aeskey, aesiv);
        }

        public static string AesDecryptToText(byte[] encrypted)
        {
            return DecryptToText(encrypted, aeskey, aesiv);
        }

        public static byte[] AesDecryptToByte(byte[] encrypted)
        {
            return DecryptToByte(encrypted, aeskey, aesiv);
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

        #endregion
    }
}
