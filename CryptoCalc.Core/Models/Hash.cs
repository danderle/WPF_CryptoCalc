
using System.Security.Cryptography;

namespace CryptoCalc.Core.Models
{
    /// <summary>
    /// A class for computing a variety of hash algorithims
    /// </summary>
    static class Hash
    {
        #region Public Methods

        public static byte[] Compute(HashAlgorithim algorithim, byte[] data)
        {
            return ComputeSha1(data);
        }

        public static byte[] ComputeSha1(byte[] data)
        {
            var sha1 = SHA1.Create();
            var hash = sha1.ComputeHash(data);
            return hash;
        }

        public static byte[] ComputeCrc2(byte[] data)
        {
            
            return data;
        }
        #endregion
    }
}
