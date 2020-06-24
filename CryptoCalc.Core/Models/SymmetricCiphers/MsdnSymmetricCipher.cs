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
        #region Public Properties

        /// <summary>
        /// The symmetric cipher algorithim to use for en-/decryption 
        /// </summary>
        public SymmetricAlgorithm MsdnCipher { get; set; }

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
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="plainText">the plain text to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public byte[] EncryptText(int algorithim, int keySize, byte[] password, string plainText)
        {
            MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
            MsdnCipher.KeySize = keySize;
            SetKeyAndIV(password);
            CheckCipherSetup(plainText);
            return Encrypt(plainText);
        }

        /// <summary>
        /// Method for encrypting plain bytes
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="plain">the plain bytes to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public byte[] EncryptBytes(int algorithim, int keySize, byte[] password, byte[] plain)
        {
            MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
            MsdnCipher.KeySize = keySize;
            SetKeyAndIV(password);
            CheckCipherSetup(plain);
            return Encrypt(plain);
        }

        /// <summary>
        /// Method for decrypting to text
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted text</returns>
        public string DecryptToText(int algorithim, int keySize, byte[] password, byte[] encrypted)
        {
            MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
            MsdnCipher.KeySize = keySize;
            SetKeyAndIV(password);
            CheckCipherSetup(encrypted);
            return DecryptToText(encrypted);
        }

        /// <summary>
        /// Method for decrypting to bytes
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public byte[] DecryptToBytes(int algorithim, int keySize, byte[] password, byte[] encrypted)
        {
            MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
            MsdnCipher.KeySize = keySize;
            SetKeyAndIV(password);
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
            var symmetricMsdnCipher = (SymmetricMsdnCipher)selectedAlgorithim;
            var cipher = SymmetricAlgorithm.Create(symmetricMsdnCipher.ToString());
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Hashes a password to the given key size
        /// </summary>
        /// <param name="password"></param>
        /// <param name="sizeInBit"></param>
        /// <returns></returns>
        private byte[] HashToSize(byte[] password, int sizeInBit)
        {
            byte[] sizedHash = new byte[sizeInBit / 8];
            List<byte> hash = new List<byte>();
            int b = 0;
            var bytes = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, password);
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
        private SymmetricAlgorithm GetMsdnCipher(SymmetricMsdnCipher algorithim)
        {
            switch (algorithim)
            {
                case SymmetricMsdnCipher.Aes:
                    return Aes.Create();
                case SymmetricMsdnCipher.DES:
                    return DES.Create();
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
            ICryptoTransform decryptor = MsdnCipher.CreateDecryptor(MsdnCipher.Key, MsdnCipher.IV);

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
            ICryptoTransform decryptor = MsdnCipher.CreateDecryptor(MsdnCipher.Key, MsdnCipher.IV);

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
            ICryptoTransform encryptor = MsdnCipher.CreateEncryptor(MsdnCipher.Key, MsdnCipher.IV);

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
            ICryptoTransform encryptor = MsdnCipher.CreateEncryptor(MsdnCipher.Key, MsdnCipher.IV);

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
        private void SetKeyAndIV(byte[] password)
        {
            var secretKey = HashToSize(password, MsdnCipher.KeySize);
            byte[] iv = HashToSize(secretKey, MsdnCipher.BlockSize);
            MsdnCipher.Key = secretKey;
            MsdnCipher.IV = iv;
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
            if (MsdnCipher.Key == null || MsdnCipher.Key.Length <= 0)
                throw new ArgumentNullException("key");
            if (MsdnCipher.IV == null || MsdnCipher.IV.Length <= 0)
                throw new ArgumentNullException("iV");
        }

        #endregion
    }
}
