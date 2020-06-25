using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class MsdnSymmetricCipher : ISymmetricCipher
    {
        #region Private Fields

        /// <summary>
        /// The symmetric cipher algorithim to use for en-/decryption 
        /// </summary>
        private SymmetricAlgorithm cipher { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default static constructor
        /// </summary>
        public MsdnSymmetricCipher()
        {
        }

        #endregion

        #region Public interface implementations

        /// <summary>
        /// Method for encrypting plain text
        /// </summary>
        /// <param name="selectedAlgorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="plainText">the plain text to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public byte[] EncryptText(int selectedAlgorithim, int keySize, byte[] secretKey, byte[] iv, string plainText)
        {
            var algorithim = (SymmetricMsdnCipher)selectedAlgorithim;
            cipher = SymmetricAlgorithm.Create(algorithim.ToString());
            cipher.KeySize = keySize;
            SetKeyAndIV(secretKey, iv);
            CheckCipherSetup(plainText);
            return Encrypt(plainText);
        }

        /// <summary>
        /// Method for encrypting plain bytes
        /// </summary>
        /// <param name="selectedAlgorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="plain">the plain bytes to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public byte[] EncryptBytes(int selectedAlgorithim, int keySize, byte[] secretKey, byte[] iv, byte[] plain)
        {
            var algorithim = (SymmetricMsdnCipher)selectedAlgorithim;
            cipher = SymmetricAlgorithm.Create(algorithim.ToString());
            cipher.KeySize = keySize;
            SetKeyAndIV(secretKey, iv);
            CheckCipherSetup(plain);
            return Encrypt(plain);
        }

        /// <summary>
        /// Method for decrypting to text
        /// </summary>
        /// <param name="selectedAlgorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted text</returns>
        public string DecryptToText(int selectedAlgorithim, int keySize, byte[] secretKey, byte[] iv, byte[] encrypted)
        {
            var algorithim = (SymmetricMsdnCipher)selectedAlgorithim;
            cipher = SymmetricAlgorithm.Create(algorithim.ToString());
            cipher.KeySize = keySize;
            SetKeyAndIV(secretKey, iv);
            CheckCipherSetup(encrypted);
            return DecryptToText(encrypted);
        }

        /// <summary>
        /// Method for decrypting to bytes
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public byte[] DecryptToBytes(int selectedAlgorithim, int keySize, byte[] secretKey, byte[] iv, byte[] encrypted)
        {
            var algorithim = (SymmetricMsdnCipher)selectedAlgorithim;
            cipher = SymmetricAlgorithm.Create(algorithim.ToString());
            cipher.KeySize = keySize;
            SetKeyAndIV(secretKey, iv);
            CheckCipherSetup(encrypted);
            return DecryptToByte(encrypted);
        }
        
        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes(int selectedAlgorithim)
        {
            var keySizes = new ObservableCollection<int>();
            var algortihim = (SymmetricMsdnCipher)selectedAlgorithim;
            var cipher = SymmetricAlgorithm.Create(algortihim.ToString());
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

        /// <summary>
        /// Return a list of MSDN symmetric algorithims
        /// </summary>
        /// <returns></returns>
        public List<string> GetAlgorthims()
        {
            return Enum.GetValues(typeof(SymmetricMsdnCipher)).Cast<SymmetricMsdnCipher>().Select(t => t.ToString()).ToList();
        }

        /// <summary>
        /// Gets the iv size for the selected cipher
        /// </summary>
        /// <returns></returns>
        public int GetIvSize(int selectedAlgorithim)
        {
            var algorithim = (SymmetricMsdnCipher)selectedAlgorithim;
            cipher = SymmetricAlgorithm.Create(algorithim.ToString());
            return cipher.BlockSize;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Hashes a secretKey to the given key size
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="sizeInBit"></param>
        /// <returns></returns>
        private byte[] HashToSize(byte[] secretKey, int sizeInBit)
        {
            byte[] sizedHash = new byte[sizeInBit / 8];
            List<byte> hash = new List<byte>();
            int b = 0;
            var bytes = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, secretKey);
            for (int i = 0; i < sizedHash.Length; i++)
            {
                if (!(b < bytes.Length))
                {
                    b = 0;
                }
                hash.Add(bytes[b]);
                b++;
            }
            Array.Copy(hash.ToArray(), sizedHash, sizedHash.Length);
            return sizedHash;
        }

        /// <summary>
        /// Return a MSDN cipher algorithim
        /// </summary>
        /// <param name="algorithim"></param>
        /// <returns></returns>
        private SymmetricAlgorithm Getcipher(SymmetricMsdnCipher algorithim)
        {
            switch (algorithim)
            {
                case SymmetricMsdnCipher.Aes:
                    return Aes.Create();
                case SymmetricMsdnCipher.DES:
                    return  DES.Create();
                case SymmetricMsdnCipher.TripleDES:
                    return TripleDES.Create();
                case SymmetricMsdnCipher.RC2:
                    return RC2.Create();
                case SymmetricMsdnCipher.Rijndael:
                    return Rijndael.Create();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Decrypt to plain string
        /// </summary>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>the plain string</returns>
        private string DecryptToText(byte[] encrypted)
        {
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = cipher.CreateDecryptor(cipher.Key, cipher.IV);

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
        private byte[] DecryptToByte(byte[] encrypted)
        {
            // Declare the string used to hold
            // the decrypted text.
            byte[] plainBytes = null;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = cipher.CreateDecryptor(cipher.Key, cipher.IV);

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
        private byte[] Encrypt(string plain)
        {
            byte[] encrypted;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = cipher.CreateEncryptor(cipher.Key, cipher.IV);

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
        private byte[] Encrypt(byte[] plain)
        {
            byte[] encrypted;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = cipher.CreateEncryptor(cipher.Key, cipher.IV);

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
        /// Sets the secret key and iv
        /// </summary>
        /// <param name="secretKey"></param>
        private void SetKeyAndIV(byte[] secretKey, byte[] iv)
        {
            cipher.Key = secretKey;
            cipher.IV = iv;
        }

        /// <summary>
        /// Verify that pre conditions are met before doing any cipher actions
        /// </summary>
        /// <param name="input">the plain or encrypted string / bytes</param>
        private void CheckCipherSetup(object input)
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
            if (cipher.Key == null || cipher.Key.Length <= 0)
                throw new ArgumentNullException("key");
            if (cipher.IV == null || cipher.IV.Length <= 0)
                throw new ArgumentNullException("iV");
        }

        #endregion
    }
}
