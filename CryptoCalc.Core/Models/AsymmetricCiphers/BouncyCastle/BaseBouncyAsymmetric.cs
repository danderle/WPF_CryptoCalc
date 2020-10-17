using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.IO;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The base class to alll the Bouncy asymmetric classes
    /// </summary>
    public class BaseBouncyAsymmetric
    {
        #region Protected Fields

        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        protected AsymmetricCipherKeyPair keyPair;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a public key <see cref="AsymmetricKeyParameter"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        protected AsymmetricKeyParameter CreateAsymmetricKeyParameterFromPublicKeyInfo(byte[] publicKeyInfo)
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
                    "Verify that the public key is not corrupted";
                throw new CryptoException(message, exception);
            }
            catch (IOException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of source do not represent an ASN.1 - DER - encoded X.509 SubjectPublicKeyInfo structure.\n" +
                    "Verify that the public key is not corrupted";
                throw new CryptoException(message, exception);
            }
            catch (ArgumentException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source indicate the key is for an algorithm other than the algorithm represented by this instance.\n" +
                    "- or - The contents of source represent the key in a format that is not supported.\n" +
                    "- or - The algorithm - specific key import failed." +
                    "Verify that the public key is not corrupted";
                throw new CryptoException(message, exception);
            }
            return publicKey;
        }

        /// <summary>
        /// Creates a private key <see cref="AsymmetricKeyParameter"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        protected AsymmetricKeyParameter CreateAsymmetricKeyParameterFromPrivateKeyInfo(byte[] privateKeyInfo)
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
            return privateKey;
        }

        /// <summary>
        /// Returns the private key info ber encoded
        /// </summary>
        /// <returns>private key in bytes</returns>
        protected byte[] GetPrivateKeyInfo()
        {
            //get the private key info
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            return privateKeyInfo.GetEncoded();
        }

        /// <summary>
        /// Returns the public key info der encoded
        /// </summary>
        /// <returns>the public key in bytes</returns>
        protected byte[] GetPublicKeyInfo()
        {
            //extract the public key info
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keyPair.Public);
            return publicKeyInfo.GetDerEncoded();
        }
        #endregion
    }
}
