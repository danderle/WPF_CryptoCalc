using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// An interface for algortihims that use EC curves
    /// </summary>
    public interface IECAlgorithims : IAsymmetricCipher
    {
        /// <summary>
        /// Ges a list ofr all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves();

        /// <summary>
        /// Create a key pair by using a given ec curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName);
    }
}
