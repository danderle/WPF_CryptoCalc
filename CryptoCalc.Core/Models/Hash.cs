using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace CryptoCalc.Core.Models
{
    /// <summary>
    /// A class for computing a variety of hash algorithims
    /// </summary>
    static class Hash
    {
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
            return ComputeSha1(data);
        }

        #region Hash Algorithims´methods

        /// <summary>
        /// MD5 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeMD5(byte[] data)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeMD4(byte[] data)
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
        /// SHA1 computation
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
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeRipeMd160(byte[] data)
        {
            var sha1 = SHA256.Create();
            var hash = sha1.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputePanama(byte[] data)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeTiger(byte[] data)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeMd2(byte[] data)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            return hash;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ComputeAdler32(byte[] data)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            return hash;
        }
        public static byte[] ComputeCrc32(byte[] data)
        {

            return data;
        }

        #endregion

        #endregion
    }
}
