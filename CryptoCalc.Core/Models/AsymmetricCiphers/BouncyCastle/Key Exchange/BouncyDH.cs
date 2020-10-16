using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace CryptoCalc.Core
{
    public class BouncyDH : IAsymmetricKeyExchange, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPair;

        #endregion

        #region Public Properties


        #endregion

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
            var a1 = new DHAgreement();

            var priv = CreatePrivateKeyParameterFromBytes(myPrivateKey);
            a1.Init(priv);

            BigInteger m1 = a1.CalculateMessage();

            var pubKey = CreatePublicKeyParameterFromBytes(otherPartyPublicKey);

            //Both party keys must share the same DHParameters to be able to calculate the agreement
            BigInteger k1 = a1.CalculateAgreement(pubKey, m1);

            return k1.ToByteArrayUnsigned();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="DHPublicKeyParameters"/> from a der encoded public key info
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private DHPublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKeyInfo)
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

            return (DHPublicKeyParameters)publicKey;
        }

        /// <summary>
        /// Creates a private key <see cref="DHPrivateKeyParameters"/> from the ber encoded private key info
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private DHPrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKeyInfo)
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
            return (DHPrivateKeyParameters)privateKey;
        }
        
        #endregion
    }
}
