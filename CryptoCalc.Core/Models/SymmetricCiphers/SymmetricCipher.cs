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

        /// <summary>
        /// The symmetric cipher algorithim to use for en-/decryption 
        /// </summary>
        public static SymmetricAlgorithm Algorithim { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default static constructor
        /// </summary>
        static SymmetricCipher()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Encrypt a plain string
        /// </summary>
        /// <param name="algorithim">The symmetric algorithim to use</param>
        /// <param name="password">The password to hash and create the secret key and iv with</param>
        /// <param name="plain">the plain string to encrypt</param>
        /// <returns>Encrypted bytes</returns>
        public static byte[] Encrypt(SymmetricAlgorithm algorithim, byte[] password, string plain)
        {
            Algorithim = algorithim;
            SetKeyAndIV(password);
            CheckCipherSetup(plain);
            return Encrypt(plain);
        }

        /// <summary>
        /// Encrypt plain bytes
        /// </summary>
        /// <param name="algorithim">The symmetric algorithim to use</param>
        /// <param name="password">The password to hash and create the secret key and iv with</param>
        /// <param name="plain">the plain bytes to encrypt</param>
        /// <returns>Encrypted bytes</returns>
        public static byte[] Encrypt(SymmetricAlgorithm algorithim, byte[] password, byte[] plain)
        {
            Algorithim = algorithim;
            SetKeyAndIV(password);
            CheckCipherSetup(plain);
            return Encrypt(plain);
        }

        /// <summary>
        /// Decrypt to plain string
        /// </summary>
        /// <param name="algorithim">The symmetric algorithim to use</param>
        /// <param name="password">The password to hash and create the secret key and iv with</param>
        /// <param name="encrypted">The encrypted bytes</param>
        /// <returns>decrypted string</returns>
        public static string DecryptToText(SymmetricAlgorithm algorithim, byte[] password, byte[] encrypted)
        {
            Algorithim = algorithim;
            SetKeyAndIV(password);
            CheckCipherSetup(encrypted);
            return DecryptToText(encrypted);
        }

        /// <summary>
        /// Decrypt to plain string
        /// </summary>
        /// <param name="algorithim">The symmetric algorithim to use</param>
        /// <param name="password">The password to hash and create the secret key and iv with</param>
        /// <param name="encrypted">The encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public static byte[] DecryptToByte(SymmetricAlgorithm algorithim, byte[] password, byte[] encrypted)
        {
            Algorithim = algorithim;
            SetKeyAndIV(password);
            CheckCipherSetup(encrypted);
            return DecryptToByte(encrypted);
        }

        /// <summary>
        /// Gets a new symmetric algorithim object
        /// </summary>
        /// <param name="algorithim">the type of symmetric algorithim toi return</param>
        /// <returns>The symmetric algorithim object</returns>
        public static SymmetricAlgorithm GetAlgorithim(SymmetricCipherAlgorithim algorithim)
        {
            switch(algorithim)
            {
                case SymmetricCipherAlgorithim.Aes:
                    return Aes.Create();
                case SymmetricCipherAlgorithim.DES:
                    return DES.Create();
                case SymmetricCipherAlgorithim.TripleDES:
                    return TripleDES.Create();
                case SymmetricCipherAlgorithim.RC2:
                    return RC2.Create();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Decrypt to plain string
        /// </summary>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>the plain string</returns>
        private static string DecryptToText(byte[] encrypted)
        {
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = Algorithim.CreateDecryptor(Algorithim.Key, Algorithim.IV);

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

            return plaintext;
        }

        /// <summary>
        /// Decrypt to plain bytes
        /// </summary>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>the plain bytes</returns>
        private static byte[] DecryptToByte(byte[] encrypted)
        {
            // Declare the string used to hold
            // the decrypted text.
            byte[] plainBytes = null;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = Algorithim.CreateDecryptor(Algorithim.Key, Algorithim.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream())
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                {
                    using (var binWriter = new BinaryWriter(csDecrypt))
                    {
                        binWriter.Write(encrypted);
                    }
                    plainBytes = msDecrypt.ToArray();
                }
            }

            return plainBytes;
        }

        /// <summary>
        /// Encrypt a plain string
        /// </summary>
        /// <param name="plain">the plain string</param>
        /// <returns>encrypted bytes</returns>
        private static byte[] Encrypt(string plain)
        {
            byte[] encrypted;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = Algorithim.CreateEncryptor(Algorithim.Key, Algorithim.IV);

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

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// Encrypt plain bytes
        /// </summary>
        /// <param name="plain">The plain bytes</param>
        /// <returns>the encrypted bytes</returns>
        private static byte[] Encrypt(byte[] plain)
        {
            byte[] encrypted;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = Algorithim.CreateEncryptor(Algorithim.Key, Algorithim.IV);

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

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        /// Takes in the users password and hashes it to create the secret key and iv for the set key size
        /// </summary>
        /// <param name="password"></param>
        /// <param name="keySize"></param>
        private static void SetKeyAndIV(byte[] password)
        {
            byte[] key = new byte[Algorithim.KeySize / 8];
            byte[] iv = new byte[Algorithim.BlockSize / 8];
            List<byte> hash = new List<byte>();
            int b = 0;
            var bytes = Hash.Compute(HashAlgorithim.SHA512, password);
            for(int i = 0; i < key.Length + iv.Length; i++)
            {
                if(!(b < bytes.Length))
                {
                    b = 0;
                }
                hash.Add(bytes[b]);
                b++;
            }
            Array.Copy(hash.ToArray(), key, key.Length);
            Array.Copy(hash.ToArray(), key.Length, iv, 0, iv.Length);
            Algorithim.Key = key;
            Algorithim.IV = iv;
        }

        /// <summary>
        /// Verify that pre conditions are met before doing any cipher actions
        /// </summary>
        /// <param name="input">the plain or encrypted string / bytes</param>
        private static void CheckCipherSetup(object input)
        {
            // Check arguments.
            if (input == null)
                throw new ArgumentNullException("plain");
            if(input is string inputs)
            {
                if(inputs.Length == 0)
                    throw new ArgumentNullException("plain");
            }
            if (input is byte[] inputb)
            {
                if (inputb.Length == 0)
                    throw new ArgumentNullException("plain");
            }
            if (Algorithim.Key == null || Algorithim.Key.Length <= 0)
                throw new ArgumentNullException("key");
            if (Algorithim.IV == null || Algorithim.IV.Length <= 0)
                throw new ArgumentNullException("iV");
        }

        #endregion
    }
}
