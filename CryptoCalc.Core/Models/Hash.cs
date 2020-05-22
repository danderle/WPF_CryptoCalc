using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;

namespace CryptoCalc.Core.Models
{
    /// <summary>
    /// A class for computing a variety of hash algorithims
    /// </summary>
    static class Hash
    {
        #region Private Properties

        /// <summary>
        /// Dictionary which hold all the hash computation functions
        /// </summary>
        private static Dictionary<HashAlgorithim, Func<byte[], byte[]>> hashMethods = new Dictionary<HashAlgorithim, Func<byte[], byte[]>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        static Hash()
        {
            SetupHashMethodsDictionary();
        }

        #endregion

        
        #region Public Methods

        /// <summary>
        /// Reads all bytes from file
        /// </summary>
        /// <param name="filePath">path to the file</param>
        /// <returns></returns>
        public static byte[] GetBytesFromFile(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Converts a hex string into byte array
        /// </summary>
        /// <param name="stringData">the hex string</param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string stringData)
        {
            List<byte> bytes = new List<byte>();
            for(int i = 0; i < stringData.Length; i+=2)
            {
                byte bite = Convert.ToByte(stringData.Substring(i, 2), 16);
                bytes.Add(bite);
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Genereic function for computing hash values
        /// </summary>
        /// <param name="algorithim">the algorthim to compute with</param>
        /// <param name="data">the data in bytes</param>
        /// <returns></returns>
        public static byte[] Compute(HashAlgorithim algorithim, byte[] data)
        {
            Func<byte[], byte[]> method;
            hashMethods.TryGetValue(algorithim, out method);
            return method.Invoke(data);
        }

        #region Hash Algorithims´methods

        /// <summary>
        /// MD5 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeMd5(byte[] data)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// MD4 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeMd4(byte[] data)
        {
            var md4 = new MD4Digest();
            md4.BlockUpdate(data, 0, data.Length);
            byte[] outData = new byte[16];
            md4.DoFinal(outData, 0);
            return outData;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeSha1(byte[] data)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA256 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeSha256(byte[] data)
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA384 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeSha384(byte[] data)
        {
            var sha384 = SHA384.Create();
            var hash = sha384.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA512 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeSha512(byte[] data)
        {
            var sha512 = SHA512.Create();
            var hash = sha512.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// RipeMd160 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeRipeMd160(byte[] data)
        {
            var ripe = new RipeMD160Digest();
            ripe.BlockUpdate(data, 0, data.Length);
            byte[] outData = new byte[20];
            ripe.DoFinal(outData, 0);
            return outData;
        }

        /// <summary>
        /// Whirlpool computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeWhirlpool(byte[] data)
        {
            var digest = new WhirlpoolDigest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] outData = new byte[64];
            digest.DoFinal(outData, 0);
            return outData;
        }

        /// <summary>
        /// Tiger computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeTiger(byte[] data)
        {
            var tiger = new TigerDigest();
            tiger.BlockUpdate(data, 0, data.Length);
            byte[] outData = new byte[24];
            tiger.DoFinal(outData, 0);
            return outData;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeMd2(byte[] data)
        {
            var md2 = new MD2Digest();
            md2.BlockUpdate(data, 0, data.Length);
            byte[] outData = new byte[16];
            md2.DoFinal(outData, 0);
            return outData;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeAdler32(byte[] data)
        {
            var digest = new AdlerChecksum();
            digest.MakeForBuff(data);
            var hash = digest.ChecksumValue;
            return BitConverter.GetBytes(hash).Reverse().ToArray();
        }

        /// <summary>
        /// Crc32 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeCrc32(byte[] data)
        {
            var hash = Crc32.Compute(data);
            return BitConverter.GetBytes(hash).Reverse().ToArray();
        }

        #endregion


        #endregion

        #region Private Methods

        /// <summary>
        /// Method for adding the hash methods to a dictionary
        /// </summary>
        private static void SetupHashMethodsDictionary()
        {
            foreach (HashAlgorithim algorithim in Enum.GetValues(typeof(HashAlgorithim)))
            {
                switch (algorithim)
                {
                    case HashAlgorithim.MD5:
                        hashMethods.Add(algorithim, ComputeMd5);
                        break;
                    case HashAlgorithim.MD4:
                        hashMethods.Add(algorithim, ComputeMd4);
                        break;
                    case HashAlgorithim.SHA1:
                        hashMethods.Add(algorithim, ComputeSha1);
                        break;
                    case HashAlgorithim.SHA256:
                        hashMethods.Add(algorithim, ComputeSha256);
                        break;
                    case HashAlgorithim.SHA384:
                        hashMethods.Add(algorithim, ComputeSha384);
                        break;
                    case HashAlgorithim.SHA512:
                        hashMethods.Add(algorithim, ComputeSha512);
                        break;
                    case HashAlgorithim.RIPEMD160:
                        hashMethods.Add(algorithim, ComputeRipeMd160);
                        break;
                    case HashAlgorithim.WHIRLPOOL:
                        hashMethods.Add(algorithim, ComputeWhirlpool);
                        break;
                    case HashAlgorithim.TIGER:
                        hashMethods.Add(algorithim, ComputeTiger);
                        break;
                    case HashAlgorithim.MD2:
                        hashMethods.Add(algorithim, ComputeMd2);
                        break;
                    case HashAlgorithim.ADLER32:
                        hashMethods.Add(algorithim, ComputeAdler32);
                        break;
                    case HashAlgorithim.CRC32:
                        hashMethods.Add(algorithim, ComputeCrc32);
                        break;
                    default:
                        Debugger.Break();
                        break;
                }
            }
        }


        #endregion
    }
}
