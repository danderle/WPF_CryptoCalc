using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Class en-/decrypting using SM2 asymmetric keys
    /// </summary>
    public class BouncySM2 : BaseBouncyAsymmetric, IAsymmetricEncryption, IECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The cipher object for this class
        /// </summary>
        private readonly SM2Engine cipher = new SM2Engine();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncySM2()
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
            var list = Enum.GetValues(typeof(EcCurveProvider)).Cast<EcCurveProvider>().Select(t => t.ToString()).ToList();
            return new ObservableCollection<string>(list);
        }

        /// <summary>
        /// Gets a list of all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves(EcCurveProvider provider)
        {
            return IECAlgorithims.GetBouncyEcCurves(provider);
        }

        /// <summary>
        /// Create a key pair for by using a given curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            var oid = ECNamedCurveTable.GetOid(curveName);
            int xLength;
            int yLength;
            do
            {
                var keyGenerationParameters = new ECKeyGenerationParameters(oid, new SecureRandom());
                var keyGenerator = new ECKeyPairGenerator();
                keyGenerator.Init(keyGenerationParameters);
                keyPair = keyGenerator.GenerateKeyPair();
                xLength = ((ECPublicKeyParameters)keyPair.Public).Q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned().Length;
                yLength = ((ECPublicKeyParameters)keyPair.Public).Q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned().Length;
            }
            while (xLength != yLength);
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
        /// Encrypts a plain text using a public key
        /// </summary>
        /// <param name="publicKey">the key used for encryption</param>
        /// <param name="plainText">the plain text to encrypt</param>
        /// <returns>encrypted bytes</returns>
        public byte[] EncryptText(byte[] publicKey, string plainText)
        {
            var plain = ByteConvert.StringToAsciiBytes(plainText);
            return EncryptBytes(publicKey, plain);
        }

        /// <summary>
        /// Encrypts a plain array of bytes using a public key
        /// </summary>
        /// <param name="publicKey">the public key used for encryption</param>
        /// <param name="plainBytes">the plain bytes to encrypt</param>
        /// <returns></returns>
        public byte[] EncryptBytes(byte[] publicKey, byte[] plainBytes)
        {
            var pubKey = (ECPublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(publicKey);
            var pubKeyWithRandom = new ParametersWithRandom(pubKey, new SecureRandom());
            cipher.Init(true, pubKeyWithRandom);
            return cipher.ProcessBlock(plainBytes, 0, plainBytes.Length);
        }

        /// <summary>
        /// Decrypts the encrypted byte array to a plain text
        /// </summary>
        /// <param name="privateKey">the private key used for decryption</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>the decrypted plain text</returns>
        public string DecryptToText(byte[] privateKey, byte[] encrypted)
        {
            var decrypted = DecryptToBytes(privateKey, encrypted);
            return ByteConvert.BytesToAsciiString(decrypted);
        }

        /// <summary>
        /// Decrypts the encrypted byte array to a plain byte array
        /// </summary>
        /// <param name="privateKey">the private key used for decryption</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted byte array</returns>
        public byte[] DecryptToBytes(byte[] privateKey, byte[] encrypted)
        {
            var privKey = (ECPrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(privateKey);
            cipher.Init(false, privKey);

            byte[] decrypted = null;
            try
            {
                decrypted = cipher.ProcessBlock(encrypted, 0, encrypted.Length);
            }
            catch(InvalidCipherTextException exception)
            {
                string message = "Decryption failed!\n" +
                        $"{exception.Message}.\n" +
                        "The encryption is not valid, this could be caused by a wrong length or corrupted encryption\n" +
                        "-or- The private key is corrupted.\n" +
                        "Verify that the correct key has been used, and the encryption was correctly copied.\n" +
                        "Encrypt and decrypt again";
                throw new CryptoException(message, exception);
            }
            return decrypted;
        }

        #endregion
    }
}
