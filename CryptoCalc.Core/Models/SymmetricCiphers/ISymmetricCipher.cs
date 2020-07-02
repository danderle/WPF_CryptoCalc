using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public interface ISymmetricCipher
    {
        #region Public Methods

        /// <summary>
        /// Method for encrypting plain text
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="plainText">the plain text to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public byte[] EncryptText(int algorithim, int keySize, byte[] secretKey, byte[] iv, string plainText);

        /// <summary>
        /// Method for encrypting plain bytes
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="plain">the plain bytes to encrypt</param>
        /// <returns>the encrypted bytes</returns>
        public byte[] EncryptBytes(int algorithim, int keySize, byte[] secretKey, byte[] iv, byte[] plain);

        /// <summary>
        /// Method for decrypting to text
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted text</returns>
        public string DecryptToText(int algorithim, int keySize, byte[] secretKey, byte[] iv, byte[] encrypted);

        /// <summary>
        /// Method for decrypting to bytes
        /// </summary>
        /// <param name="algorithim">the algorithim to use, will be cast to enum</param>
        /// <param name="keySize">the key size to use</param>
        /// <param name="secretKey">the secret key for the algorithim</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted bytes</returns>
        public byte[] DecryptToBytes(int algorithim, int keySize, byte[] secretKey, byte[] iv, byte[] encrypted);

        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes(int selectedAlgorithim);

        /// <summary>
        /// Returns a list of symmetric algorithims
        /// </summary>
        /// <returns></returns>
        public List<string> GetAlgorthims();

        /// <summary>
        /// Gets the Iv size in bits
        /// </summary>
        /// <returns></returns>
        public int GetIvSize(int selectedAlgorithim);

        /// <summary>
        /// Generates a secret key according to the selected algorithim and the keysize
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <param name="keySize"></param>
        /// <returns></returns>
        public List<byte[]> GenerateKey(int selectedAlgorithim, int keySize);

        #endregion
    }
}
