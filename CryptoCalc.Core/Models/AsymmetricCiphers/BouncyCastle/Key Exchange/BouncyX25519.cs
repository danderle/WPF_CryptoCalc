using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace CryptoCalc.Core
{
    public class BouncyX25519 : IAsymmetricKeyExchange, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPair;

        #endregion

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
            //get the private key info
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            return privateKeyInfo.GetEncoded();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            //extract the public key info
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            return publicKeyInfo.GetDerEncoded();
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
            
            var privateKey = CreatePrivateKeyParameterFromBytes(myPrivateKey);
            a1.Init(privateKey);
            
            byte[] k1 = new byte[a1.AgreementSize];

            var publicKey = CreatePublicKeyParameterFromBytes(otherPartyPublicKey);

            a1.CalculateAgreement(publicKey, k1, 0);

            return k1;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="X25519PublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private X25519PublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKeyInfo)
        {
            AsymmetricKeyParameter publicKey = null;
            try
            {
                publicKey = PublicKeyFactory.CreateKey(publicKeyInfo);
            }
            catch (SecurityUtilityException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of source do not represent an ASN.1 - DER - encoded X.509 SubjectPublicKeyInfo structure.\n" +
                    "- or - The contents of the source do not represent a usable object identifier\n" +
                    "Verify that the public key is not corrupted.";
                throw new CryptoException(message, exception);
            }
            catch (IOException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of source do not represent an ASN.1 - DER - encoded X.509 SubjectPublicKeyInfo structure.\n" +
                    "Verify that the public key is not corrupted.";
                throw new CryptoException(message, exception);
            }
            catch (ArgumentException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source indicate the key is for an algorithm other than the algorithm represented by this instance.\n" +
                    "- or - The contents of source represent the key in a format that is not supported.\n" +
                    "- or - The algorithm - specific key import failed.\n" +
                    "Verify that the public key is not corrupted.";
                throw new CryptoException(message, exception);
            }
            return (X25519PublicKeyParameters)publicKey;
        }

        /// <summary>
        /// Creates a private key <see cref="X25519PrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="privateKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private X25519PrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKeyInfo)
        {
            AsymmetricKeyParameter privateKey = null;
            try
            {
                privateKey = PrivateKeyFactory.CreateKey(privateKeyInfo);
            }
            catch (SecurityUtilityException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a usable object identifier\n" +
                    "Verify that the public key is not corrupted";
                throw new CryptoException(message, exception);
            }
            catch (IOException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of source do not represent an ASN1 - BER - encoded PKCS#8 structure.\n" +
                    "Verify that the public key is not corrupted";
                throw new CryptoException(message, exception);
            }
            catch (ArgumentException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source do not represent an ASN.1 - BER - encoded PKCS#8 structure.\n" +
                    "Verify that the private key is not corrupted";
                throw new CryptoException(message, exception);
            }
            return (X25519PrivateKeyParameters)privateKey;
        }

        
        #endregion
    }
}
