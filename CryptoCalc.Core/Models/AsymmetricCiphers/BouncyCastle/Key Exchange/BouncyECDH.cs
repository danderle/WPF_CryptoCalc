using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Class for EC Diffie Hellmann key exchange
    /// </summary>
    public class BouncyECDH : IAsymmetricKeyExchange, IECAlgorithims
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
        public BouncyECDH()
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
        /// Create a key pair by using a given ec curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            var oid = ECNamedCurveTable.GetOid(curveName);
            int xLength;
            int yLength;
            var keyGenerationParameters = new ECKeyGenerationParameters(oid, new SecureRandom());
            var keyGenerator = new ECKeyPairGenerator();

            do
            {
                keyGenerator.Init(keyGenerationParameters);
                keyPair = keyGenerator.GenerateKeyPair();
                xLength = ((ECPublicKeyParameters)keyPair.Public).Q.AffineXCoord.ToBigInteger().ToByteArrayUnsigned().Length;
                yLength = ((ECPublicKeyParameters)keyPair.Public).Q.AffineYCoord.ToBigInteger().ToByteArrayUnsigned().Length;
            }
            while (xLength != yLength);
        }

        /// <summary>
        /// Returns the private key der encoded
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            //get the private key info
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            return privateKeyInfo.GetDerEncoded();
        }

        /// <summary>
        /// Returns the public key info der encoded
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
            var a1 = new ECDHBasicAgreement();

            var priv = CreatePrivateKeyParameterFromBytes(myPrivateKey);
            a1.Init(priv);

            var pubKey = CreatePublicKeyParameterFromBytes(otherPartyPublicKey);

            BigInteger k = null;
            try
            {
                k = a1.CalculateAgreement(pubKey);
            }
            catch(InvalidOperationException exception)
            {
                string message = "Key Deriviation Failed!\n" +
                    $"{exception.Message}.\n" +
                    "Different EC curves were used to create the public keys.";
                throw new CryptoException(message, exception);
            }

            return k.ToByteArrayUnsigned();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="ECPublicKeyParameters"/> from the der encoded public key info
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private ECPublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKeyInfo)
        {
            AsymmetricKeyParameter publicKey = null;
            try
            {
                publicKey = PublicKeyFactory.CreateKey(publicKeyInfo);
            }
            catch(SecurityUtilityException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a usable object identifier\n" +
                    "Verify that the public key is not corrupted";
                throw new CryptoException(message, exception);
            }
            catch(IOException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of source do not represent an ASN1 - DER - encoded structure.\n" +
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
            return (ECPublicKeyParameters)publicKey;
        }

        /// <summary>
        /// Creates a private key <see cref="ECPrivateKeyParameters"/> from the der encoded private key info
        /// </summary>
        /// <param name="privateKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private ECPrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKeyInfo)
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
                    "The contents of source do not represent an ASN1 - DER - encoded structure.\n" +
                    "Verify that the public key is not corrupted";
                throw new CryptoException(message, exception);
            }
            catch (ArgumentException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source do not represent an ASN.1 - DER - encoded structure.\n" +
                    "Verify that the private key is not corrupted";
                throw new CryptoException(message, exception);
            }
            return (ECPrivateKeyParameters)privateKey;
        }

        #endregion
    }
}
