using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
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

        private bool bouncyCipherHasIV;

        private int bouncyIvSize;

        #endregion

        #region Public Properties

        public IBufferedCipher BouncyCipher { get; set; }

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
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="plainText">the plain text to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public byte[] EncryptText(int algorithim, int keySize, byte[] password, string plainText)
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

        /// <summary>
        /// Method for decrypting to text
        /// </summary>
        /// <param name="api">the cryptography api to use</param>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="password">the passsword which will be hashed to the specified key size</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted text</returns>
        public string DecryptToText(int algorithim, int keySize, byte[] password, byte[] encrypted)
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
        
        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes(int selectedAlgorithim)
        {
            var keySizes = new ObservableCollection<int>(); bouncyCipherHasIV = false;
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

        /// <summary>
        /// Return a list of Bouncy castle symmetric algorithims
        /// </summary>
        /// <returns></returns>
        public List<string> GetAlgorthims()
        {
            return Enum.GetValues(typeof(SymmetricBouncyCastleCipher)).Cast<SymmetricBouncyCastleCipher>().Select(t => t.ToString()).ToList();
        }

        #endregion

        #region Private Methods

        /// </summary>
        /// <param name="password"></param>
        /// <param name="sizeInBit"></param>
        /// <returns></returns>
        private static byte[] HashToSize(byte[] password, int sizeInBit)
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

        #endregion    
    }
    }
