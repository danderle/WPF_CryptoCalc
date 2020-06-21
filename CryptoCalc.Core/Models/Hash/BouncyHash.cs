using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace CryptoCalc.Core
{
    /// <summary>
    /// A class for computing a variety of hash algorithims
    /// </summary>
    static class BouncyHash
    {
        #region Private Properties

        /// <summary>
        /// Dictionary which holds all the hash computation functions
        /// </summary>
        private static Dictionary<BouncyHashAlgorithim, Func<byte[], byte[], byte[]>> hashMethods = new Dictionary<BouncyHashAlgorithim, Func<byte[], byte[], byte[]>> ();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        static BouncyHash()
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
        public static byte[] Compute(BouncyHashAlgorithim algorithim, byte[] data, byte[] key = null)
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
            byte[] outData = new byte[16];
            if (key != null)
            {
                var digest = new HMac(new MD5Digest());
                digest.Init(new KeyParameter(key));
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            else
            {
                var digest = new MD5Digest();
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            return outData;
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
                var digest = new HMac(new MD4Digest());
                digest.Init(new KeyParameter(key));
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            else
            {
                var digest = new MD4Digest();
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
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
            byte[] outData = new byte[16];
            if (key != null)
            {
                var digest = new HMac(new Sha1Digest());
                digest.Init(new KeyParameter(key));
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            else
            {
                var digest = new Sha1Digest();
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            return outData;
        }

        /// <summary>
        /// SHA256 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeSha256(byte[] data, byte[] key)
        {
            byte[] outData = new byte[16];
            if (key != null)
            {
                var digest = new HMac(new Sha256Digest());
                digest.Init(new KeyParameter(key));
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            else
            {
                var digest = new Sha256Digest();
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            return outData;
        }

        /// <summary>
        /// SHA384 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeSha384(byte[] data, byte[] key)
        {
            byte[] outData = new byte[16];
            if (key != null)
            {
                var digest = new HMac(new Sha384Digest());
                digest.Init(new KeyParameter(key));
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            else
            {
                var digest = new Sha384Digest();
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            return outData;
        }

        /// <summary>
        /// SHA512 computation
        /// </summary>
        /// <param name="data">the data to hash</param>
        /// <param name="key">optional hmac key</param>
        /// <returns>the hash value</returns>
        private static byte[] ComputeSha512(byte[] data, byte[] key)
        {
            byte[] outData = new byte[16];
            if (key != null)
            {
                var digest = new HMac(new Sha512Digest());
                digest.Init(new KeyParameter(key));
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            else
            {
                var digest = new Sha512Digest();
                digest.BlockUpdate(data, 0, data.Length);
                digest.DoFinal(outData, 0);
            }
            return outData;
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

        #endregion


        #endregion

        #region Private Methods

        /// <summary>
        /// Method for adding the hash methods to a dictionary
        /// </summary>
        private static void SetupHashMethodsDictionary()
        {
            foreach (BouncyHashAlgorithim algorithim in Enum.GetValues(typeof(BouncyHashAlgorithim)))
            {
                switch (algorithim)
                {
                    case BouncyHashAlgorithim.MD5:
                        hashMethods.Add(algorithim, ComputeMd5);
                        break;
                    case BouncyHashAlgorithim.MD4:
                        hashMethods.Add(algorithim, ComputeMd4);
                        break;
                    case BouncyHashAlgorithim.SHA1:
                        hashMethods.Add(algorithim, ComputeSha1);
                        break;
                    case BouncyHashAlgorithim.SHA256:
                        hashMethods.Add(algorithim, ComputeSha256);
                        break;
                    case BouncyHashAlgorithim.SHA384:
                        hashMethods.Add(algorithim, ComputeSha384);
                        break;
                    case BouncyHashAlgorithim.SHA512:
                        hashMethods.Add(algorithim, ComputeSha512);
                        break;
                    case BouncyHashAlgorithim.RIPEMD160:
                        hashMethods.Add(algorithim, ComputeRipeMd160);
                        break;
                    case BouncyHashAlgorithim.WHIRLPOOL:
                        hashMethods.Add(algorithim, ComputeWhirlpool);
                        break;
                    case BouncyHashAlgorithim.TIGER:
                        hashMethods.Add(algorithim, ComputeTiger);
                        break;
                    case BouncyHashAlgorithim.MD2:
                        hashMethods.Add(algorithim, ComputeMd2);
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
