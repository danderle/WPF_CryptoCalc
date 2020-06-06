using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace CryptoCalc.Core
{
    /// <summary>
    /// A class for computing a variety of hash algorithims
    /// </summary>
    static class Hash
    {
        #region Private Properties

        /// <summary>
        /// Dictionary which holds all the hash computation functions
        /// </summary>
        private static Dictionary<HashAlgorithim, Func<byte[], byte[], byte[]>> hashMethods = new Dictionary<HashAlgorithim, Func<byte[], byte[], byte[]>> ();

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
        /// Genereic function for computing hash values
        /// </summary>
        /// <param name="algorithim">the algorthim to compute with</param>
        /// <param name="data">the data in bytes</param>
        /// <returns>the hash value</returns>
        public static byte[] Compute(HashAlgorithim algorithim, byte[] data, byte[] key = null)
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
        /// MD4 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeMd4(byte[] data, byte[] key)
        {
            byte[] outData = new byte[16];
            if (key != null)
            {
                var md4Hmac = new HMac(new MD4Digest());
                md4Hmac.Init(new KeyParameter(key));
                md4Hmac.BlockUpdate(data, 0, data.Length);
                md4Hmac.DoFinal(outData, 0);
            }
            else
            {
                var md4 = new MD4Digest();
                md4.BlockUpdate(data, 0, data.Length);
                md4.DoFinal(outData, 0);
            }
            return outData;
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
        /// RipeMd160 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeRipeMd160(byte[] data, byte[] key)
        {
            byte[] outData = new byte[20];
            
            if (key != null)
            {
                var ripeMd160Hmac = new HMac(new RipeMD160Digest());
                ripeMd160Hmac.Init(new KeyParameter(key));
                ripeMd160Hmac.BlockUpdate(data, 0, data.Length);
                ripeMd160Hmac.DoFinal(outData, 0);
            }
            else
            {
                var ripe = new RipeMD160Digest();
                ripe.BlockUpdate(data, 0, data.Length);
                ripe.DoFinal(outData, 0);
            }
            
            return outData;
        }

        /// <summary>
        /// Whirlpool computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeWhirlpool(byte[] data, byte[] key)
        {
            byte[] outData = new byte[64];

            if (key != null)
            {
                var poolHmac = new HMac(new WhirlpoolDigest());
                poolHmac.Init(new KeyParameter(key));
                poolHmac.BlockUpdate(data, 0, data.Length);
                poolHmac.DoFinal(outData, 0);
            }
            else
            {
                var digest = new WhirlpoolDigest();
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            
            return outData;
        }

        /// <summary>
        /// Tiger computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeTiger(byte[] data, byte[] key)
        {
            byte[] outData = new byte[24];

            if (key != null)
            {
                var tigerHmac = new HMac(new TigerDigest());
                tigerHmac.Init(new KeyParameter(key));
                tigerHmac.BlockUpdate(data, 0, data.Length);
                tigerHmac.DoFinal(outData, 0);
            }
            else
            {
                var tiger = new TigerDigest();
                tiger.BlockUpdate(data, 0, data.Length);
                tiger.DoFinal(outData, 0);
            }
            
            return outData;
        }

        /// <summary>
        /// SHA1 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">not usable</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeMd2(byte[] data, byte[] key)
        {
            byte[] outData = new byte[16];

            if (key != null)
            {
                var md2Hmac = new HMac(new MD2Digest());
                md2Hmac.Init(new KeyParameter(key));
                md2Hmac.BlockUpdate(data, 0, data.Length);
                md2Hmac.DoFinal(outData, 0);
            }
            else
            {
                var md2 = new MD2Digest();
                md2.BlockUpdate(data, 0, data.Length);
                md2.DoFinal(outData, 0);
            }
            
            return outData;
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
