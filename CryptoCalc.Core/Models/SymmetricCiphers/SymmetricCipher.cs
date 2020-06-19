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

namespace CryptoCalc.Core
{
    static class SymmetricCipher
    {
        #region Private Fields

        private static bool bouncyCipherHasIV;

        private static int bouncyIvSize;

        #endregion

        #region Public Properties

        /// <summary>
        /// The symmetric cipher algorithim to use for en-/decryption 
        /// </summary>
        public static SymmetricAlgorithm MsdnCipher { get; set; }

        public static IBufferedCipher BouncyCipher { get; set; }

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
        /// Method for encrypting plain text
        /// </summary>
        /// <param name="api">the cryptography api to use</param>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="plainText">the plain text to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public static byte[] EncryptText(CryptographyApi api, int algorithim, int keySize, byte[] password, string plainText)
        {
            if(api == CryptographyApi.MSDN)
            {
                MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
                MsdnCipher.KeySize = keySize;
                SetKeyAndIV(password);
                CheckCipherSetup(plainText);
                return Encrypt(plainText);
            }
            else
            {
                var algo = ((SymmetricBouncyCastleCipher)algorithim).ToString();
                BouncyCipher = CipherUtilities.GetCipher(algo);
                var plain = ByteConvert.StringToAsciiBytes(plainText);
                var secretKey = HashToSize(password, keySize);
                if (bouncyCipherHasIV)
                {
                    var kp = new KeyParameter(secretKey);
                    byte[] iv = HashToSize(secretKey, bouncyIvSize);
                    var ivp = new ParametersWithIV(kp, iv);
                    BouncyCipher.Init(true, ivp);
                }
                else
                {
                    BouncyCipher.Init(true, new KeyParameter(secretKey));
                }
                return BouncyCipher.DoFinal(plain);
            }
        }

        /// <summary>
        /// Method for encrypting plain bytes
        /// </summary>
        /// <param name="api">the cryptography api to use</param>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="plain">the plain bytes to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public static byte[] EncryptBytes(CryptographyApi api, int algorithim, int keySize, byte[] password, byte[] plain)
        {
            if (api == CryptographyApi.MSDN)
            {
                MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
                MsdnCipher.KeySize = keySize;
                SetKeyAndIV(password);
                CheckCipherSetup(plain);
                return Encrypt(plain);
            }
            else
            {
                var algo = ((SymmetricBouncyCastleCipher)algorithim).ToString();
                BouncyCipher = CipherUtilities.GetCipher(algo);
                var secretKey = HashToSize(password, keySize);
                if(bouncyCipherHasIV)
                {
                    var kp = new KeyParameter(secretKey);
                    byte[] iv = HashToSize(secretKey, bouncyIvSize);
                    var ivp = new ParametersWithIV(kp, iv);
                    BouncyCipher.Init(true, ivp);
                }
                else
                {
                    BouncyCipher.Init(true, new KeyParameter(secretKey));
                }
                return BouncyCipher.DoFinal(plain);
            }
        }

        /// <summary>
        /// Method for decrypting to text
        /// </summary>
        /// <param name="api">the cryptography api to use</param>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted text</returns>
        public static string DecryptToText(CryptographyApi api, int algorithim, int keySize, byte[] password, byte[] encrypted)
        {
            if (api == CryptographyApi.MSDN)
            {
                MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
                MsdnCipher.KeySize = keySize;
                SetKeyAndIV(password);
                CheckCipherSetup(encrypted);
                return DecryptToText(encrypted);
            }
            else
            {
                var algo = ((SymmetricBouncyCastleCipher)algorithim).ToString();
                BouncyCipher = CipherUtilities.GetCipher(algo);
                var secretKey = HashToSize(password, keySize);
                if (bouncyCipherHasIV)
                {
                    var kp = new KeyParameter(secretKey);
                    byte[] iv = HashToSize(secretKey, bouncyIvSize);
                    var ivp = new ParametersWithIV(kp, iv);
                    BouncyCipher.Init(false, ivp);
                }
                else
                {
                    BouncyCipher.Init(false, new KeyParameter(secretKey));
                }
                var output = BouncyCipher.DoFinal(encrypted);
                return ByteConvert.BytesToAsciiString(output);
            }
        }

        /// <summary>
        /// Method for decrypting to bytes
        /// </summary>
        /// <param name="api">the cryptography api to use</param>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public static byte[] DecryptToBytes(CryptographyApi api, int algorithim, int keySize, byte[] password, byte[] encrypted)
        {
            if (api == CryptographyApi.MSDN)
            {
                MsdnCipher = GetMsdnCipher((SymmetricMsdnCipher)algorithim);
                MsdnCipher.KeySize = keySize;
                SetKeyAndIV(password);
                CheckCipherSetup(encrypted);
                return DecryptToByte(encrypted);
            }
            else
            {
                var algo = ((SymmetricBouncyCastleCipher)algorithim).ToString();
                BouncyCipher = CipherUtilities.GetCipher(algo) as PaddedBufferedBlockCipher;
                var secretKey = HashToSize(password, keySize);
                if (bouncyCipherHasIV)
                {
                    var kp = new KeyParameter(secretKey);
                    byte[] iv = HashToSize(secretKey, bouncyIvSize);
                    var ivp = new ParametersWithIV(kp, iv);
                    BouncyCipher.Init(false, ivp);
                }
                else
                {
                    BouncyCipher.Init(false, new KeyParameter(secretKey));
                }
                return BouncyCipher.DoFinal(encrypted);
            }
        }
        
        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedCryptoApi"></param>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public static ObservableCollection<int> GetKeySizes(CryptographyApi selectedCryptoApi, int selectedAlgorithim)
        {
            var keySizes = new ObservableCollection<int>();
            if (selectedCryptoApi == CryptographyApi.MSDN)
            {
                var algorithim = (SymmetricMsdnCipher)selectedAlgorithim;
                var cipher = SymmetricAlgorithm.Create(algorithim.ToString());
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
            else
            {
                bouncyCipherHasIV = false;
                bouncyIvSize = 16;
                int bits;
                int max;
                var algorithim = (SymmetricBouncyCastleCipher)selectedAlgorithim;
                switch(algorithim)
                {
                    case SymmetricBouncyCastleCipher.AES:
                    case SymmetricBouncyCastleCipher.CAMELLIA:
                    case SymmetricBouncyCastleCipher.RIJNDAEL:
                    case SymmetricBouncyCastleCipher.SERPENT:
                    case SymmetricBouncyCastleCipher.TNEPRES:
                    case SymmetricBouncyCastleCipher.TWOFISH:
                        return new ObservableCollection<int> { 128, 192, 256 };
                    case SymmetricBouncyCastleCipher.BLOWFISH:
                        bits = 32;
                        max = 448;
                        while (bits <= max)
                        {
                            keySizes.Add(bits);
                            bits++;
                        }
                        return keySizes;
                    case SymmetricBouncyCastleCipher.CAST5:
                        bits = 40;
                        max = 128;
                        while (bits <= max)
                        {
                            keySizes.Add(bits);
                            bits++;
                        }
                        return keySizes;
                    case SymmetricBouncyCastleCipher.CAST6:
                        bits = 128;
                        max = 256;
                        while (bits <= max)
                        {
                            keySizes.Add(bits);
                            bits++;
                        }
                        return keySizes;
                    case SymmetricBouncyCastleCipher.RC2:
                        bits = 8;
                        max = 1024;
                        while (bits <= max)
                        {
                            keySizes.Add(bits);
                            bits+=8;
                        }
                        return keySizes;
                    case SymmetricBouncyCastleCipher.RC5:
                        bits = 0;
                        max = 2040;
                        while (bits <= max)
                        {
                            keySizes.Add(bits);
                            bits += 64;
                        }
                        return keySizes;
                    case SymmetricBouncyCastleCipher.RC6:
                        bits = 128;
                        max = 2040;
                        while (bits <= max)
                        {
                            keySizes.Add(bits);
                            bits += 64;
                        }
                        return keySizes;
                    case SymmetricBouncyCastleCipher.CHACHA:
                    case SymmetricBouncyCastleCipher.SALSA20:
                        bouncyCipherHasIV = true;
                        bouncyIvSize = 8;
                        return new ObservableCollection<int> { 128, 256 };
                    case SymmetricBouncyCastleCipher.CHACHA7539:
                    case SymmetricBouncyCastleCipher.HC256:
                    case SymmetricBouncyCastleCipher.VMPC:
                    case SymmetricBouncyCastleCipher.VMPC_KSA3:
                        bouncyCipherHasIV = true;
                        return new ObservableCollection<int> { 256 };
                    case SymmetricBouncyCastleCipher.GOST28147:
                    case SymmetricBouncyCastleCipher.THREEFISH_256:
                        return new ObservableCollection<int> { 256 };
                    case SymmetricBouncyCastleCipher.DES:
                        return new ObservableCollection<int> { 64 };
                    case SymmetricBouncyCastleCipher.DESEDE:
                        return new ObservableCollection<int> { 128, 192 };
                    case SymmetricBouncyCastleCipher.HC128:
                        bouncyCipherHasIV = true;
                        return new ObservableCollection<int> { 128 };
                    case SymmetricBouncyCastleCipher.IDEA:
                    case SymmetricBouncyCastleCipher.NOEKEON:
                    case SymmetricBouncyCastleCipher.SEED:
                    case SymmetricBouncyCastleCipher.SM4:
                    case SymmetricBouncyCastleCipher.TEA:
                    case SymmetricBouncyCastleCipher.XTEA:
                        return new ObservableCollection<int> { 128 };
                    case SymmetricBouncyCastleCipher.SKIPJACK:
                        return new ObservableCollection<int> { 80 };
                    case SymmetricBouncyCastleCipher.THREEFISH_512:
                        return new ObservableCollection<int> { 512 };
                    case SymmetricBouncyCastleCipher.THREEFISH_1024:
                        return new ObservableCollection<int> { 1024 };
                }
                return null;
            }
        }

        /// <summary>
        /// Return a list of MSDN symmetric algorithims
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMSDNAlgorthims()
        {
            return Enum.GetValues(typeof(SymmetricMsdnCipher)).Cast<SymmetricMsdnCipher>().Select(t => t.ToString()).ToList();
        }

        /// <summary>
        /// Return a list of Bouncy castle symmetric algorithims
        /// </summary>
        /// <returns></returns>
        public static List<string> GetBouncyCastleAlgorithims()
        {
            return Enum.GetValues(typeof(SymmetricBouncyCastleCipher)).Cast<SymmetricBouncyCastleCipher>().Select(t => t.ToString()).ToList();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Hashes a password to the given key size
        /// </summary>
        /// <param name="password"></param>
        /// <param name="sizeInBit"></param>
        /// <returns></returns>
        private static byte[] HashToSize(byte[] password, int sizeInBit)
        {
            byte[] sizedHash = new byte[sizeInBit / 8];
            List<byte> hash = new List<byte>();
            int b = 0;
            var bytes = Hash.Compute(HashAlgorithim.SHA512, password);
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
        private static SymmetricAlgorithm GetMsdnCipher(SymmetricMsdnCipher algorithim)
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
        private static string DecryptToText(byte[] encrypted)
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
        private static byte[] DecryptToByte(byte[] encrypted)
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
        private static byte[] Encrypt(string plain)
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
        private static byte[] Encrypt(byte[] plain)
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
        private static void SetKeyAndIV(byte[] password)
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
            if (MsdnCipher.Key == null || MsdnCipher.Key.Length <= 0)
                throw new ArgumentNullException("key");
            if (MsdnCipher.IV == null || MsdnCipher.IV.Length <= 0)
                throw new ArgumentNullException("iV");
        }

        #endregion
    }
}
