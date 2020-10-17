using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Class for Diffie Hellmann key exchange
    /// </summary>
    public class BouncyDH : BaseBouncyAsymmetric, IAsymmetricKeyExchange, INonECAlgorithims
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyDH()
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
            for (; strength <= maxStrength; strength += multiple)
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
            var dhParamGen = new DHParametersGenerator();
            dhParamGen.Init(keySize, 20, new SecureRandom());
            var dhParams = dhParamGen.GenerateParameters();
            var keyGenerationParameters = new DHKeyGenerationParameters(new SecureRandom(), dhParams);
            var keyGenerator = new DHKeyPairGenerator();
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
        /// Derives a shared secret key from a private key and another persons public key
        /// </summary>
        /// <param name="myPrivateKey">the private key which is used</param>
        /// <param name="otherPartyPublicKey">the public key of the other person</param>
        /// <returns></returns>
        public byte[] DeriveKey(byte[] myPrivateKey, byte[] otherPartyPublicKey)
        {
            DHPrivateKeyParameters privKey = null;
            try
            {
                privKey = (DHPrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(myPrivateKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid DH key parameter\n" +
                    "Verify that the key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }

            var a1 = new DHAgreement();
            a1.Init(privKey);

            BigInteger m1 = a1.CalculateMessage();

            DHPublicKeyParameters pubKey = null;
            try
            {
                pubKey = (DHPublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(otherPartyPublicKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid DH key parameter\n" +
                    "Verify that the key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }

            //Both party keys must share the same DHParameters to be able to calculate the agreement
            BigInteger k1 = a1.CalculateAgreement(pubKey, m1);

            return k1.ToByteArrayUnsigned();
        }

        #endregion
    }
}
