using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// class for signing and verifing data using ED448 asymmetric keys
    /// </summary>
    public class BouncyEd448 : IAsymmetricSignature, IECAlgorithims
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
        public BouncyEd448()
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
            return new ObservableCollection<string> { "ED448" };
        }

        /// <summary>
        /// Gets a list of all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves(EcCurveProvider provider)
        {
            return new ObservableCollection<string> { "Ed448" };
        }

        /// <summary>
        /// Create a key pair for by using a given curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            var keyGenerationParameters = new Ed448KeyGenerationParameters(new SecureRandom());
            var keyGenerator = new Ed448KeyPairGenerator();
            keyGenerator.Init(keyGenerationParameters);
            keyPair = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var encoded = ((Ed448PrivateKeyParameters)keyPair.Private).GetEncoded();
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
            var der = ((Ed448PrivateKeyParameters)keyPair.Private).GeneratePublicKey().GetEncoded();
            var Y = ((Ed448PublicKeyParameters)keyPair.Public).GetEncoded();
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
            var signer = new Ed448Signer(ByteConvert.StringToAsciiBytes("context"));

            Ed448PrivateKeyParameters privKey = null;
            try
            {
                privKey = CreatePrivateKeyParameterFromBytes(privateKey);
            }
            catch (ArgumentException exception)
            {
                string message = "Private Key Creation Failure!\n" +
                    $"{exception.Message}.\n" +
                    $"The private key file is corrupted, verify private key file or try another key.\n" +
                    $"If all fails create a new key pair.";
                throw new CryptoException(message, exception);
            }

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
            var signer = new Ed448Signer(ByteConvert.StringToAsciiBytes("context"));

            Ed448PublicKeyParameters pubKey = null;
            try
            {
                pubKey = CreatePublicKeyParameterFromBytes(publicKey);
            }
            catch (ArgumentException exception)
            {
                string message = "Public Key Creation Failure!\n" +
                    $"{exception.Message}.\n" +
                    $"The public key file is corrupted, verify public key file or try another key.\n" +
                    $"If all fails create a new key pair.";
                throw new CryptoException(message, exception);
            }

            signer.Init(false, pubKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(originalSignature);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="Ed448PublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private Ed448PublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            return new Ed448PublicKeyParameters(publicKey, 0);
        }

        /// <summary>
        /// Creates a private key <see cref="Ed448PrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="privateKey">the byte array containing the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private Ed448PrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            return new Ed448PrivateKeyParameters(privateKey, 0);
        }

        #endregion
    }
}
