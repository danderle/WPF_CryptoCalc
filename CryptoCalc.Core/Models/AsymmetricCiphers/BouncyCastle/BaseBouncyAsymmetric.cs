using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        #region Public Static Methods

        /// <summary>
        /// Returns a Bouncy Castle cipher object
        /// </summary>
        /// <param name="selectedAlgorithim">The cipher algorithim to return</param>
        /// <returns>The selected cipher object</returns>
        public static IAsymmetricCipher GetCipher(string selectedAlgorithim)
        {
            var algorithim = (AsymmetricBouncyCiphers)Enum.Parse(typeof(AsymmetricBouncyCiphers), selectedAlgorithim);
            switch (algorithim)
            {
                case AsymmetricBouncyCiphers.RSA:
                    return new BouncyRsa();
                case AsymmetricBouncyCiphers.SM2:
                    return new BouncySM2();
                case AsymmetricBouncyCiphers.DSA:
                    return new BouncyDsa();
                case AsymmetricBouncyCiphers.ECDsa:
                    return new BouncyECDsa();
                case AsymmetricBouncyCiphers.ECGost3410:
                    return new BouncyECGost3410();
                case AsymmetricBouncyCiphers.Gost3410_94:
                    return new BouncyGost3410_94();
                case AsymmetricBouncyCiphers.ECNR:
                    return new BouncyECNR();
                case AsymmetricBouncyCiphers.ED25519:
                    return new BouncyEd25519();
                case AsymmetricBouncyCiphers.ED448:
                    return new BouncyEd448();
                case AsymmetricBouncyCiphers.DiffieHellman:
                    return new BouncyDH();
                case AsymmetricBouncyCiphers.ECDiffieHellman:
                    return new BouncyECDH();
                case AsymmetricBouncyCiphers.ECMQV:
                    return new BouncyECMqv();
                case AsymmetricBouncyCiphers.X25519:
                    return new BouncyX25519();
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Gets a list of possible Bouncy castle asymmetric cipher algorithims accoridng to a selected operation
        /// </summary>
        /// <param name="operation">The type of operation</param>
        /// <returns>The list of cipher algorithims according the selected operation</returns>
        public static List<string> GetAlgorthims(AsymmetricOperation operation)
        {
            switch (operation)
            {
                case AsymmetricOperation.Encryption:
                    return new List<string>
                    {
                        AsymmetricBouncyCiphers.SM2.ToString(),
                        AsymmetricBouncyCiphers.RSA.ToString(),
                    };

                case AsymmetricOperation.Signature:
                    return new List<string>
                    {
                        AsymmetricBouncyCiphers.ECDsa.ToString(),
                        AsymmetricBouncyCiphers.RSA.ToString(),
                        AsymmetricBouncyCiphers.DSA.ToString(),
                        AsymmetricBouncyCiphers.ECGost3410.ToString(),
                        AsymmetricBouncyCiphers.Gost3410_94.ToString(),
                        AsymmetricBouncyCiphers.ECNR.ToString(),
                        AsymmetricBouncyCiphers.ED25519.ToString(),
                        AsymmetricBouncyCiphers.ED448.ToString(),
                    };

                case AsymmetricOperation.KeyExchange:
                    return new List<string>
                    {
                        AsymmetricBouncyCiphers.DiffieHellman.ToString(),
                        AsymmetricBouncyCiphers.ECDiffieHellman.ToString(),
                        AsymmetricBouncyCiphers.ECMQV.ToString(),
                        AsymmetricBouncyCiphers.X25519.ToString(),
                    };
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Gets a list of available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetCurves(EcCurveProvider provider)
        {
            IEnumerator curves = null;
            switch (provider)
            {
                case EcCurveProvider.SEC:
                    curves = SecNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.NIST:
                    curves = NistNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.TELETRUST:
                    curves = TeleTrusTNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.ANSSI:
                    curves = AnssiNamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.GOST3410:
                    curves = ECGost3410NamedCurves.Names.GetEnumerator();
                    break;
                case EcCurveProvider.GM:
                    curves = GMNamedCurves.Names.GetEnumerator();
                    break;
            }
            var list = new ObservableCollection<string>();
            while (curves.MoveNext())
            {
                list.Add((string)curves.Current);
            }

            //sort list
            list = new ObservableCollection<string>(list.OrderBy(x => x));
            return list;
        }
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
