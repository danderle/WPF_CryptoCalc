using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Class for singing and verifying using the DSA algorithim
    /// </summary>
    public class BouncyDsa : BaseBouncyAsymmetric, IAsymmetricSignature, INonECAlgorithims
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyDsa()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the available key sizes for the given algorithim
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<int> GetKeySizes()
        {
            var keySizes = new ObservableCollection<int>();
            int maxStrength = 1024;
            int multiple = 64;
            int strength = 512;
            for(; strength <= maxStrength; strength += multiple)
            {
                keySizes.Add(strength);
            }
            return keySizes;
        }

        /// <summary>
        /// Create a key pair for according to the key size
        /// </summary>
        /// <param name="keySize">the key size in bits</param>
        public void CreateKeyPair(int keySize)
        {
            var dsaParametersGenerator = new DsaParametersGenerator();
            dsaParametersGenerator.Init(keySize, 80, new SecureRandom());
            var parameters = new DsaKeyGenerationParameters(new SecureRandom(), dsaParametersGenerator.GenerateParameters());
            var keyGenerator = new DsaKeyPairGenerator();
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
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privateKey, byte[] data)
        {
            var signer = new DsaDigestSigner(new DsaSigner(), new Sha1Digest());
            var privKey = (DsaPrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(privateKey);
            signer.Init(true, privKey);
            signer.BlockUpdate(data, 0, data.Length);
            byte[] signature;
            try
            {
                signature = signer.GenerateSignature();
            }
            catch(Exception exception)
            {
                string message = "Signature Failure!\n" +
                    $"{exception.Message}.\n" +
                    $"The private key file is corrupted, verify private key file or try another key.\n" +
                    $"If all fails create a new key pair.";
                throw new CryptoException(message, exception);
            }
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
            var signer = new DsaDigestSigner(new DsaSigner(), new Sha1Digest());

            DsaPublicKeyParameters pubKey = null;
            try
            {
                pubKey = (DsaPublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(publicKey);
            }
            catch(Exception exception)
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
    }
}
