using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The RSA algorithim provided by the MSDN library
    /// </summary>
    public class MsdnRsa : IAsymmetricEncryption, IAsymmetricSignature, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The RSA cipher object
        /// </summary>
        private RSACryptoServiceProvider cipher { get; set; } = new RSACryptoServiceProvider();

        #endregion

        #region Public Properties

        public bool UsesEcCurves => false;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnRsa()
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
            foreach (var legalkeySize in cipher.LegalKeySizes)
            {
                int keySize = legalkeySize.MinSize;
                while (keySize <= legalkeySize.MaxSize)
                {
                    keySizes.Add(keySize);
                    if (legalkeySize.SkipSize == 0)
                        break;
                    keySize += legalkeySize.SkipSize;
                }
            }
            return keySizes;
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
            int bytesRead;
            cipher.ImportRSAPublicKey(publicKey, out bytesRead);
            return cipher.Encrypt(plainBytes, false);
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
            int byteRead;
            cipher.ImportRSAPrivateKey(privateKey, out byteRead); 
            return cipher.Decrypt(encrypted, false);
        }

        /// <summary>
        /// Create a key pair for according to the key size
        /// </summary>
        /// <param name="keySize">the key size in bits</param>
        public void CreateKeyPair(int keySize)
        {
            cipher = new RSACryptoServiceProvider(keySize);
        }

        /// <summary>
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportRSAPrivateKey(privKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.SignHash(hash, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// Verifies a signature to be authentic
        /// </summary>
        /// <param name="originalSignature">The signature which is be verified</param>
        /// <param name="publicKey">the public key used for the verification</param>
        /// <param name="data">the data which is signed</param>
        /// <returns>true if signature is authentic, false if not</returns>
        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportRSAPublicKey(pubKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.VerifyHash(hash, originalSignature, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            return cipher.ExportRSAPrivateKey();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            return cipher.ExportRSAPublicKey();
        }

        #endregion
    }
}
