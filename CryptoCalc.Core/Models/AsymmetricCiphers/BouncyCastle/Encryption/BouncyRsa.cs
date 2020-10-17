using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Bouncy castle RSA class for doing encryption and signatures
    /// </summary>
    public class BouncyRsa : BaseBouncyAsymmetric, IAsymmetricEncryption, IAsymmetricSignature, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The cipher object for this class
        /// </summary>
        private readonly IAsymmetricBlockCipher cipher = new RsaEngine();
        
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyRsa()
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
            int maxStrength = 16384;
            for(int strength = 384; strength < maxStrength; strength += 512)
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
            var parameters = new KeyGenerationParameters(new SecureRandom(), keySize);
            var keyGenerator = new RsaKeyPairGenerator();
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
            //CreateKeyPair public key
            var pubKey = (RsaKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(publicKey);
            cipher.Init(true, pubKey);

            byte[] encrypted;
            try
            {
                //encrypt plain bytes
                encrypted = cipher.ProcessBlock(plainBytes, 0, plainBytes.Length);
            }
            catch (CryptoException exception)
            {
                if (exception.Message == "input too large for RSA cipher.")
                {
                    string message = "Encryption Failure!\n" +
                        $"{exception.Message}\n" +
                        "The plain data bit size cannot be greater than the selected key size.\n" +
                        $"Key bit size: {cipher.GetInputBlockSize() * 8}\n" +
                        $"Plain data bit size: {plainBytes.Length * 8}";
                    throw new CryptoException(message, exception);
                }
                else
                {
                    string message = "Encryption Failure!\n" +
                        $"{exception.Message}\n" +
                        "Contact developer.";
                    throw new CryptoException(message, exception);
                }
            }
            return encrypted;
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
            //create private key
            var privKey = (RsaKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(privateKey);
            cipher.Init(false, privKey);
            byte[] decrypted;
            try 
            {
                //decrypt
                decrypted = cipher.ProcessBlock(encrypted, 0, encrypted.Length);
            }
            catch(CryptoException exception)
            {
                if(exception.Message == "input too large for RSA cipher.")
                {
                    string message = "Decryption Failure!\n" +
                        $"{exception.Message}\n" +
                        "The encryption bit size cannot be greater than the selected key size.\n" +
                        $"Key bit size: {cipher.GetInputBlockSize() * 8}\n" +
                        $"Encryption bit size: {encrypted.Length * 8}";
                    throw new CryptoException(message, exception);
                }
                else
                {
                    string message = "Encryption Failure!\n" +
                        $"{exception.Message}\n" +
                        "Contact developer.";
                    throw new CryptoException(message, exception);
                }
            }
            return decrypted;
        }

        /// <summary>
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privateKey, byte[] data)
        {
            var signer = new RsaDigestSigner(new Sha1Digest());
            var privKey = (RsaKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(privateKey);
            signer.Init(true, privKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
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
            var signer = new RsaDigestSigner(new Sha1Digest());
            var pubKey = (RsaKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(publicKey);
            signer.Init(false, pubKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(originalSignature);
        }

        #endregion
    }
}
