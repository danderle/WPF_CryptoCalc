using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    public class BouncySymmetricCipher : ISymmetricCipher
    {
        #region Private Fields

        // <summary>
        /// The buffered cipher to use for en- / decryption
        /// </summary>
        private IBufferedCipher bufferedCipher { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default static constructor
        /// </summary>
        public BouncySymmetricCipher()
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
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString().Replace("_","-");;
            bufferedCipher = CipherUtilities.GetCipher(algorithim);
            var plain = ByteConvert.StringToUTF8Bytes(plainText);
            if (GetIvSize(selectedAlgorithim) > 0)
            {
                var kp = new KeyParameter(secretKey);
                var ivp = new ParametersWithIV(kp, iv);
                bufferedCipher.Init(true, ivp);
            }
            else
            {
                bufferedCipher.Init(true, new KeyParameter(secretKey));
            }
            return bufferedCipher.DoFinal(plain);
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
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString().Replace("_","-");;
            bufferedCipher = CipherUtilities.GetCipher(algorithim);
            if (GetIvSize(selectedAlgorithim) > 0)
            {
                var kp = new KeyParameter(secretKey);
                var ivp = new ParametersWithIV(kp, iv);
                bufferedCipher.Init(true, ivp);
            }
            else
            {
                bufferedCipher.Init(true, new KeyParameter(secretKey));
            }
            return bufferedCipher.DoFinal(plain);
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
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString().Replace("_","-");;
            bufferedCipher = CipherUtilities.GetCipher(algorithim);
            if (GetIvSize(selectedAlgorithim) > 0)
            {
                var kp = new KeyParameter(secretKey);
                var ivp = new ParametersWithIV(kp, iv);
                bufferedCipher.Init(false, ivp);
            }
            else
            {
                bufferedCipher.Init(false, new KeyParameter(secretKey));
            }
            var output = bufferedCipher.DoFinal(encrypted);
            return ByteConvert.BytesToUTF8String(output);
        }

        /// <summary>
        /// Method for decrypting to bytes
        /// </summary>
        /// <param name="selectedAlgorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public byte[] DecryptToBytes(int selectedAlgorithim, int keySize, byte[] secretKey, byte[] iv, byte[] encrypted)
        {
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString().Replace("_","-");;
            bufferedCipher = CipherUtilities.GetCipher(algorithim) as PaddedBufferedBlockCipher;
            if (GetIvSize(selectedAlgorithim) > 0)
            {
                var kp = new KeyParameter(secretKey);
                var ivp = new ParametersWithIV(kp, iv);
                bufferedCipher.Init(false, ivp);
            }
            else
            {
                bufferedCipher.Init(false, new KeyParameter(secretKey));
            }
            return bufferedCipher.DoFinal(encrypted);
        }
        
        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes(int selectedAlgorithim)
        {
            var keySizes = new ObservableCollection<int>(); 
            int keySkipSize;
            int keySize = 0;
            int maxKeySize;
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
                    keySkipSize = 32;
                    keySize = keySkipSize;
                    maxKeySize = 448;
                    while (keySize <= maxKeySize)
                    {
                        keySizes.Add(keySize);
                        keySize += keySkipSize;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.CAST5:
                    keySkipSize = 40;
                    keySize = keySkipSize;
                    maxKeySize = 128;
                    while (keySize <= maxKeySize)
                    {
                        keySizes.Add(keySize);
                        keySize += keySkipSize;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.CAST6:
                    keySkipSize = 128;
                    keySize = keySkipSize;
                    maxKeySize = 256;
                    while (keySize <= maxKeySize)
                    {
                        keySizes.Add(keySize);
                        keySize += keySkipSize;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.RC2:
                    keySkipSize = 8;
                    keySize = keySkipSize;
                    maxKeySize = 1024;
                    while (keySize <= maxKeySize)
                    {
                        keySizes.Add(keySize);
                        keySize += keySkipSize;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.RC5:
                    keySkipSize = 64;
                    keySize = keySkipSize;
                    maxKeySize = 2040;
                    while (keySize <= maxKeySize)
                    {
                        keySizes.Add(keySize);
                        keySize += keySkipSize;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.RC6:
                    keySkipSize = 128;
                    keySize = keySkipSize;
                    maxKeySize = 2040;
                    while (keySize <= maxKeySize)
                    {
                        keySizes.Add(keySize);
                        keySize += keySkipSize/2;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.CHACHA:
                case SymmetricBouncyCastleCipher.SALSA20:
                    return new ObservableCollection<int> { 128, 256 };
                case SymmetricBouncyCastleCipher.CHACHA7539:
                case SymmetricBouncyCastleCipher.HC256:
                case SymmetricBouncyCastleCipher.VMPC:
                case SymmetricBouncyCastleCipher.VMPC_KSA3:
                    return new ObservableCollection<int> { 256 };
                case SymmetricBouncyCastleCipher.GOST28147:
                case SymmetricBouncyCastleCipher.THREEFISH_256:
                    return new ObservableCollection<int> { 256 };
                case SymmetricBouncyCastleCipher.DES:
                    return new ObservableCollection<int> { 64 };
                case SymmetricBouncyCastleCipher.DESEDE:
                    return new ObservableCollection<int> { 128, 192 };
                case SymmetricBouncyCastleCipher.HC128:
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

        /// <summary>
        /// Return a list of Bouncy castle symmetric algorithims
        /// </summary>
        /// <returns></returns>
        public List<string> GetAlgorthims()
        {
            return Enum.GetValues(typeof(SymmetricBouncyCastleCipher)).Cast<SymmetricBouncyCastleCipher>().Select(t => t.ToString()).ToList();
        }

        /// <summary>
        /// Gets the iv size in bits
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public int GetIvSize(int selectedAlgorithim)
        {
            var algorithim = (SymmetricBouncyCastleCipher)selectedAlgorithim;
            switch (algorithim)
            {
                case SymmetricBouncyCastleCipher.CHACHA:
                case SymmetricBouncyCastleCipher.SALSA20:
                    return 64;
                case SymmetricBouncyCastleCipher.CHACHA7539:
                    return 96;
                case SymmetricBouncyCastleCipher.HC256:
                case SymmetricBouncyCastleCipher.VMPC:
                case SymmetricBouncyCastleCipher.VMPC_KSA3:
                case SymmetricBouncyCastleCipher.HC128:
                    return 128;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Generates a secret key according to the selected algorithim and the keysize
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public List<byte[]> GenerateKey(int selectedAlgorithim, int keySize)
        {
            var keyAndIv = new List<byte[]>();
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString().Replace("_","-");
            var keyGenerator = GeneratorUtilities.GetKeyGenerator(algorithim);
            var keyParameters = new KeyGenerationParameters(new SecureRandom(), keySize);
            keyGenerator.Init(keyParameters);
            keyAndIv.Add(keyGenerator.GenerateKey());
            var ivSize = GetIvSize(selectedAlgorithim);
            if (ivSize > 0)
            {
                algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString().Replace("_","-");
                keyGenerator = GeneratorUtilities.GetKeyGenerator(algorithim);
                keyParameters = new KeyGenerationParameters(new SecureRandom(), ivSize);
                keyGenerator.Init(keyParameters);
                keyAndIv.Add(keyGenerator.GenerateKey());
            }
            return keyAndIv;
        }

        #endregion

        #region Private Methods

        #endregion
    }
    }
