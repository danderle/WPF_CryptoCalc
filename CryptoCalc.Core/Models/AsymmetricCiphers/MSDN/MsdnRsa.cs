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

        /// <summary>
        /// A flag for knowing if the algorithim uses key sizes for key creation
        /// </summary>
        public bool UsesCurves => false;

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
                    keySize += 512;
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
            var plain = ByteConvert.StringToUTF8Bytes(plainText);
            return EncryptBytes(publicKey, plain);
        }

        ///TODO RSA plain bytes should not be larger than the selected key size
        ///create try catch
        /// <summary>
        /// Encrypts a plain array of bytes using a public key
        /// </summary>
        /// <param name="publicKey">the public key used for encryption</param>
        /// <param name="plainBytes">the plain bytes to encrypt</param>
        /// <returns></returns>
        public byte[] EncryptBytes(byte[] publicKey, byte[] plainBytes)
        {
            int bytesRead;
            byte[] encryption;

            try
            {
                //imnport the public key
                cipher.ImportRSAPublicKey(publicKey, out bytesRead);

                //Encrypt the plain bytes
                encryption = cipher.Encrypt(plainBytes, false);
            }
            catch(CryptographicException exception)
            {
                if(exception.Message == "ASN1 corrupted data.")
                {
                    string message = "Encryption failed!\n" +
                        "The public key file seems to be corrupted.\n" +
                        "Verify if the key is correct, try another key or create a new key.";
                    throw new CryptographicException(message, exception);
                }
                else if(exception.Message == "Ungültige Länge")
                {
                    string message = "Encryption failed!\n" +
                        "The key size is too small for the data size!\n" +
                        "Increase the key size or decrease the data size to be encrypted.\n" +
                        $"Key bit size: {cipher.KeySize}\n" +
                        $"Plain bit size: {plainBytes.Length*8}";
                    throw new CryptographicException(message, exception);
                }
                else
                {
                    throw new CryptographicException("Contact developer for help.", exception);
                }
            }
            return encryption;
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
            return ByteConvert.BytesToUTF8String(decrypted);
        }

        /// <summary>
        /// Decrypts the encrypted byte array to a plain byte array
        /// TODO programm crashes if trying to decrypt a wrong encrypted message
        /// </summary>
        /// <param name="privateKey">the private key used for decryption</param>
        /// <param name="encrypted">the encrypted bytes</param>
        /// <returns>decrypted byte array</returns>
        public byte[] DecryptToBytes(byte[] privateKey, byte[] encrypted)
        {
            int byteRead;
            byte[] decrypted = null;
            try
            {
                //Import the private key
                cipher.ImportRSAPrivateKey(privateKey, out byteRead); 

                //decrypt with default PKCS#1 padding
                decrypted = cipher.Decrypt(encrypted, false);
            }
            catch(CryptographicException exception)
            {
                if (exception.Message == "ASN1 corrupted data." ||
                    exception.Message == "Ein an das System angeschlossenes Gerät funktioniert nicht.")
                {
                    string message = "Decryption failed!\n" +
                        "The private key file seems to be corrupted.\n" +
                        "Verify if the key is correct, try another key or create a new key pair";
                    throw new CryptographicException(message, exception);
                }
                else if(exception.Message == "Falscher Parameter.")
                {
                    string message = "Decryption failed!\n" +
                        "Wrong parameters are detected.\n" +
                        "Verify that the encryption is correctly entered or that the right key was used for encryption";
                    throw new CryptographicException(message, exception);
                }
                else if (cipher.KeySize != encrypted.Length * 8)
                {
                    string message = "Decryption failed!\n" +
                        "The encrypted bit size must be equal to the selected key size!\n" +
                        $"Key bit size: {cipher.KeySize}\n" +
                        $"Encrypted bit size: {encrypted.Length * 8}";
                    throw new CryptographicException(message, exception);
                }
                else
                {
                    throw new CryptographicException("Contact developer for help.", exception);
                }
            }
            return decrypted;
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
