using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    /// <summary>
    /// A class for computing a variety of hash algorithims
    /// </summary>
    static class MsdnHash
    {
        #region Private Properties

        /// <summary>
        /// Dictionary which holds all the hash computation functions
        /// </summary>
        private static Dictionary<MsdnHashAlgorithim, Func<byte[], byte[], byte[]>> hashMethods = new Dictionary<MsdnHashAlgorithim, Func<byte[], byte[], byte[]>> ();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        static MsdnHash()
        {
            SetupHashMethodsDictionary();
        }

        #endregion
        
        #region Public Methods

        /// <summary>
        /// Genereic function for computing hash values
        /// </summary>
        /// <param name="algorithim">the algorthim to compute with</param>
        /// <param name="data">the data in bytes</param>
        /// <returns>the hash value</returns>
        public static byte[] Compute(MsdnHashAlgorithim algorithim, byte[] data, byte[] key = null)
        {
            Func<byte[], byte[], byte[]> method;
            hashMethods.TryGetValue(algorithim, out method);
            return method.Invoke(data, key);
        }

        #region Hash Algorithim methods

        /// <summary>
        /// MD5 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeMd5(byte[] data, byte[] key)
        {
            byte[] hash = null;

            if(key != null)
            {
                var md5hmac = new HMACMD5(key);
                md5hmac.Key = key;
                hash = md5hmac.ComputeHash(data);
            }
            else
            {
                var md5 = MD5.Create();
                hash = md5.ComputeHash(data);
            }
            return hash;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeSha1(byte[] data, byte[] key)
        {
            byte[] hash = null;

            if (key != null)
            {
                var sha1Hmac = new HMACSHA1(key);
                sha1Hmac.Key = key;
                hash = sha1Hmac.ComputeHash(data);
            }
            else
            {
                var sha1 = SHA1.Create();
                hash = sha1.ComputeHash(data);
            }
            return hash;
        }

        /// <summary>
        /// SHA256 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeSha256(byte[] data, byte[] key)
        {
            byte[] hash = null;

            if (key != null)
            {
                var sha256Hmac = new HMACSHA256(key);
                sha256Hmac.Key = key;
                hash = sha256Hmac.ComputeHash(data);
            }
            else
            {
                var sha256 = SHA256.Create();
                hash = sha256.ComputeHash(data);
            }
            return hash;
        }

        /// <summary>
        /// SHA384 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeSha384(byte[] data, byte[] key)
        {
            byte[] hash = null;

            if (key != null)
            {
                var sha384Hmac = new HMACSHA384(key);
                sha384Hmac.Key = key;
                hash = sha384Hmac.ComputeHash(data);
            }
            else
            {
                var sha384 = SHA384.Create();
                hash = sha384.ComputeHash(data);
            }
            return hash;
        }

        /// <summary>
        /// SHA512 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeSha512(byte[] data, byte[] key)
        {
            byte[] hash = null;

            if (key != null)
            {
                var sha512Hmac = new HMACSHA512(key);
                sha512Hmac.Key = key;
                hash = sha512Hmac.ComputeHash(data);
            }
            else
            {
                var sha512 = SHA512.Create();
                hash = sha512.ComputeHash(data);
            }
            return hash;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">not usable</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeAdler32(byte[] data, byte[] key = null)
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
        /// <param name="key">not usable</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeCrc32(byte[] data, byte[] key = null)
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
            foreach (MsdnHashAlgorithim algorithim in Enum.GetValues(typeof(MsdnHashAlgorithim)))
            {
                switch (algorithim)
                {
                    case MsdnHashAlgorithim.MD5:
                        hashMethods.Add(algorithim, ComputeMd5);
                        break;
                    case MsdnHashAlgorithim.SHA1:
                        hashMethods.Add(algorithim, ComputeSha1);
                        break;
                    case MsdnHashAlgorithim.SHA256:
                        hashMethods.Add(algorithim, ComputeSha256);
                        break;
                    case MsdnHashAlgorithim.SHA384:
                        hashMethods.Add(algorithim, ComputeSha384);
                        break;
                    case MsdnHashAlgorithim.SHA512:
                        hashMethods.Add(algorithim, ComputeSha512);
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
