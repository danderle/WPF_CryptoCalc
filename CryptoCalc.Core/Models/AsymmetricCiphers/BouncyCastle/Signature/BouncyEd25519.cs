using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// class for signing and verifing data using ED25519 asymmetric keys
    /// </summary>
    public class BouncyEd25519 : BaseBouncyAsymmetric, IAsymmetricSignature, IECAlgorithims
    {
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
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privateKey, byte[] data)
        {
            Ed25519PrivateKeyParameters privKey = null;
            try
            {
                privKey = (Ed25519PrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(privateKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid Ed25519 key parameter\n" +
                    "Verify that the key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }
            var signer = new Ed25519Signer();
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
            Ed25519PublicKeyParameters pubKey = null;
            try
            {
                pubKey = (Ed25519PublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(publicKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid Ed25519 key parameter\n" +
                    "Verify that the key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }
            var signer = new Ed25519Signer();
            signer.Init(false, pubKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(originalSignature);
        }

        #endregion
    }
}
