using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// An interface for algortihims that do not use EC curves
    /// </summary>
    public interface INonECAlgorithims : IAsymmetricCipher
    {
        /// <summary>
        /// A flag for knowing if the algorithim uses key sizes for key creation
        /// </summary>
        public bool UsesKeySize => true;

        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes();

        /// <summary>
        /// Create a key pair for according to the key size
        /// </summary>
        /// <param name="keySize">the key size in bits</param>
        public void CreateKeyPair(int keySize);
    }
}
