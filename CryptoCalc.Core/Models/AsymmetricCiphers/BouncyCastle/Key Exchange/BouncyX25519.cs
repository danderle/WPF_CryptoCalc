using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Class for X25519 key exchange
    /// </summary>
    public class BouncyX25519 : BaseBouncyAsymmetric, IAsymmetricKeyExchange, INonECAlgorithims
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyX25519()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <param name="selectedAlgorithim"></param>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes()
        {
            var keySizes = new ObservableCollection<int>();
            keySizes.Add(255);
            return keySizes;
        }

        /// <summary>
        /// Create a key pair for according to the key size. 
        /// For X25519 the key size is not relevant, it always has a strength of 255 bits.
        /// </summary>
        /// <param name="keySize">the key size in bits</param>
        public void CreateKeyPair(int keySize = 255)
        {
            var parameters = new X25519KeyGenerationParameters(new SecureRandom());
            var keyGenerator = new X25519KeyPairGenerator();
            keyGenerator.Init(parameters);
            keyPair = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            return GetPrivateKeyInfo();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            return GetPublicKeyInfo();
        }

        /// <summary>
        /// Derives a shared secret key from a private key and another persons public key
        /// </summary>
        /// <param name="myPrivateKey">the private key which is used</param>
        /// <param name="otherPartyPublicKey">the public key of the other person</param>
        /// <returns></returns>
        public byte[] DeriveKey(byte[] myPrivateKey, byte[] otherPartyPublicKey)
        {
            var a1 = new X25519Agreement();
            
            var privateKey = (X25519PrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(myPrivateKey);
            a1.Init(privateKey);
            
            byte[] k1 = new byte[a1.AgreementSize];

            var publicKey = (X25519PublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(otherPartyPublicKey);

            a1.CalculateAgreement(publicKey, k1, 0);

            return k1;
        }

        #endregion
    }
}
