using System;
using System.Collections.Generic;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Security;
using System.Linq;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class BouncySymmetricCipher : ISymmetricCipher
    {
        #region Private Fields

        /// <summary>
        /// The size of the iv in bytes
        /// </summary>
        private int ivSize;

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
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString();
            bufferedCipher = CipherUtilities.GetCipher(algorithim);
            var plain = ByteConvert.StringToAsciiBytes(plainText);
            if (iv.Length == ivSize)
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
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString();
            bufferedCipher = CipherUtilities.GetCipher(algorithim);
            if(iv.Length == ivSize)
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
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString();
            bufferedCipher = CipherUtilities.GetCipher(algorithim);
            if (iv.Length == ivSize)
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
            return ByteConvert.BytesToAsciiString(output);
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
            var algorithim = ((SymmetricBouncyCastleCipher)selectedAlgorithim).ToString();
            bufferedCipher = CipherUtilities.GetCipher(algorithim) as PaddedBufferedBlockCipher;
            if (iv.Length == ivSize)
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
            ivSize = -1;
            int keySkipSize;
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
                    maxKeySize = 448;
                    while (keySkipSize <= maxKeySize)
                    {
                        keySizes.Add(keySkipSize);
                        keySkipSize++;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.CAST5:
                    keySkipSize = 40;
                    maxKeySize = 128;
                    while (keySkipSize <= maxKeySize)
                    {
                        keySizes.Add(keySkipSize);
                        keySkipSize++;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.CAST6:
                    keySkipSize = 128;
                    maxKeySize = 256;
                    while (keySkipSize <= maxKeySize)
                    {
                        keySizes.Add(keySkipSize);
                        keySkipSize++;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.RC2:
                    keySkipSize = 8;
                    maxKeySize = 1024;
                    while (keySkipSize <= maxKeySize)
                    {
                        keySizes.Add(keySkipSize);
                        keySkipSize+=8;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.RC5:
                    keySkipSize = 0;
                    maxKeySize = 2040;
                    while (keySkipSize <= maxKeySize)
                    {
                        keySizes.Add(keySkipSize);
                        keySkipSize += 64;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.RC6:
                    keySkipSize = 128;
                    maxKeySize = 2040;
                    while (keySkipSize <= maxKeySize)
                    {
                        keySizes.Add(keySkipSize);
                        keySkipSize += 64;
                    }
                    return keySizes;
                case SymmetricBouncyCastleCipher.CHACHA:
                case SymmetricBouncyCastleCipher.SALSA20:
                    ivSize = 8;
                    return new ObservableCollection<int> { 128, 256 };
                case SymmetricBouncyCastleCipher.CHACHA7539:
                case SymmetricBouncyCastleCipher.HC256:
                case SymmetricBouncyCastleCipher.VMPC:
                case SymmetricBouncyCastleCipher.VMPC_KSA3:
                    ivSize = 16;
                    return new ObservableCollection<int> { 256 };
                case SymmetricBouncyCastleCipher.GOST28147:
                case SymmetricBouncyCastleCipher.THREEFISH_256:
                    return new ObservableCollection<int> { 256 };
                case SymmetricBouncyCastleCipher.DES:
                    return new ObservableCollection<int> { 64 };
                case SymmetricBouncyCastleCipher.DESEDE:
                    return new ObservableCollection<int> { 128, 192 };
                case SymmetricBouncyCastleCipher.HC128:
                    ivSize = 16;
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
            var size = ivSize * 8;
            return size > 0 ? size : 0;
        }

        #endregion

        #region Private Methods

        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="sizeInBit"></param>
        /// <returns></returns>
        private static byte[] HashToSize(byte[] secretKey, int sizeInBit)
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

        

        #endregion
    }
    }
