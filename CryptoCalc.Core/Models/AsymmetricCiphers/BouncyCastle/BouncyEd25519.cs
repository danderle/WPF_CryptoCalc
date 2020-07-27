using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BouncyEd25519 : IAsymmetricSignature, IECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPair;

        #endregion

        #region Public Properties

        /// <summary>
        /// A flag for knowing if the algorithim uses elliptical curves
        /// </summary>
        public bool UsesCurves => true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyEd25519()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the available ec curve providers
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> GetEcProviders()
        {
            return new ObservableCollection<string> { "ED25519" };
        }

        /// <summary>
        /// Gets a list of all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves(EcCurveProvider provider)
        {
            return new ObservableCollection<string> { "Ed25519" };
        }

        /// <summary>
        /// Create a key pair for by using a given curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            var keyGenerationParameters = new Ed25519KeyGenerationParameters(new SecureRandom());
            var keyGenerator = new Ed25519KeyPairGenerator();
            keyGenerator.Init(keyGenerationParameters);
            keyPair = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var encoded = ((Ed25519PrivateKeyParameters)keyPair.Private).GetEncoded();
            var privateKey = new List<byte>();
            privateKey.AddRange(encoded);

            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var der = ((Ed25519PrivateKeyParameters)keyPair.Private).GeneratePublicKey().GetEncoded();
            var Y = ((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded();
            var publicKey = new List<byte>();
            publicKey.AddRange(der);
            publicKey.AddRange(Y);

            return publicKey.ToArray();
        }


        /// <summary>
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privateKey, byte[] data)
        {
            var signer = new Ed25519Signer();
            var privKey = CreatePrivateKeyParameterFromBytes(privateKey);
            signer.Init(true, privKey);
            signer.BlockUpdate(data, 0, data.Length);
            var signature = signer.GenerateSignature();
            return signature;
        }

        /// <summary>
        /// Verifies a signature to be authentic
        /// </summary>
        /// <param name="originalSignature">The signature which is be verified</param>
        /// <param name="publicKey">the public key used for the verification</param>
        /// <param name="data">the data which is signed</param>
        /// <returns>true if signature is authentic, false if not</returns>
        public bool Verify(byte[] originalSignature, byte[] publicKey, byte[] data)
        {
            var signer = new Ed25519Signer();
            var pubKey = CreatePublicKeyParameterFromBytes(publicKey);
            signer.Init(false, pubKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(originalSignature);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="Ed25519PublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private Ed25519PublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            return new Ed25519PublicKeyParameters(publicKey, 0);
        }

        /// <summary>
        /// Creates a private key <see cref="Gost3410PrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="privateKey">the byte array containing the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private Ed25519PrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            return new Ed25519PrivateKeyParameters(privateKey, 0);
        }

        #endregion
    }
}
