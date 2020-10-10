using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    public class BouncySM2 : IAsymmetricEncryption, IECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The cipher object for this class
        /// </summary>
        private SM2Engine cipher = new SM2Engine();
        
        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPair;

        private int keyLength = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// A flag for knowing if the algorithim uses elliptical curves
        /// </summary>
        public bool UsesCurves => true;

        /// <summary>
        /// A flag for knowing if the algorithim uses key sizes for key creation
        /// </summary>
        public bool UsesKeySize => false;

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
                default:
                    return new ObservableCollection<string>();
            }
            var list = new List<string>();
            while (curves.MoveNext())
            {
                list.Add((string)curves.Current);
            }

            //sort the list alphabetically
            var sortedList = new ObservableCollection<string>(list.OrderBy(x => x));
            return sortedList;

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
            keyLength = xLength * 8;
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var der = ((ECPrivateKeyParameters)keyPair.Private).PublicKeyParamSet.ToAsn1Object().GetDerEncoded();
            var d = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArrayUnsigned();
            var privateKey = new List<byte>();
            privateKey.AddRange(der);
            privateKey.AddRange(d);

            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var der = ((ECPublicKeyParameters)keyPair.Public).PublicKeyParamSet.ToAsn1Object().GetDerEncoded();
            var Q = ((ECPublicKeyParameters)keyPair.Public).Q.GetEncoded();
            var publicKey = new List<byte>();
            publicKey.AddRange(der);
            publicKey.AddRange(Q);
            return publicKey.ToArray();
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
            var pubKey = CreatePublicKeyParameterFromBytes(publicKey);
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
            var privKey = CreatePrivateKeyParameterFromBytes(privateKey);
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

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="ECPublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public RSA key parameter object</returns>
        private ECPublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            //Get length of the DER ecnoded bytes plus 1 for the tag and length of the tlv
            var der = new byte[publicKey[1] + 2];
            int restLength = publicKey.Length - der.Length;

            //The x an y split the rest length
            var x = new byte[restLength / 2];
            var y = new byte[restLength / 2];
            var q = new byte[restLength];
            Array.Copy(publicKey, der, der.Length);
            Array.Copy(publicKey, der.Length, x, 0, x.Length);
            Array.Copy(publicKey, der.Length + x.Length, y, 0, y.Length);
            Array.Copy(publicKey, der.Length, q, 0, q.Length);

            //Get the der object identifierer
            DerObjectIdentifier derOid;
            try
            {
                derOid = DerObjectIdentifier.GetInstance(Asn1Object.FromByteArray(der));
            }
            catch (ArgumentException exception)
            {
                string message = "Encryption failed!\n" +
                        $"{exception.Message}.\n" +
                        "The public key creation failed\n" +
                        "The key is not in the expected ASN1 structure.\n" +
                        "Make sure the correct public key file is used.";
                throw new CryptoException(message, exception);
            }

            //Find the curve for the object identifier
            var x9 = ECNamedCurveTable.GetByOid(derOid);

            //Get the X and Y coordinates and then create the ECPoint
            var X = new BigInteger(1, x);
            var Y = new BigInteger(1, y);
            ECPoint ecPoint;
            try
            {
                ecPoint = x9.Curve.DecodePoint(q);
            }
            catch(ArgumentException exception)
            {
                string message = "Encryption failed!\n" +
                        $"{exception.Message}.\n" +
                        "The public key creation failed\n" +
                        "This could be caused by a corrupted key file.";
                throw new CryptoException(message, exception);
            }

            return new ECPublicKeyParameters("EC", ecPoint, derOid);
        }

        /// <summary>
        /// Creates a private key <see cref="ECPrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private RSA key parameter object</returns>
        private ECPrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            //Get length of the DER ecnoded bytes plus 2 for the tag and length of the tlv
            var der = new byte[privateKey[1] + 2];
            int restLength = privateKey.Length - der.Length;
            var d = new byte[restLength];
            Array.Copy(privateKey, der, der.Length);
            Array.Copy(privateKey, der.Length, d, 0, d.Length);

            DerObjectIdentifier derObject;
            try
            {
                derObject = DerObjectIdentifier.GetInstance(Asn1Object.FromByteArray(der));
            }
            catch(ArgumentException exception)
            {
                string message = "Decryption failed!\n" +
                        $"{exception.Message}.\n" +
                        "The private key creation failed\n" +
                        "Make sure the correct private key file is used.\n" +
                        "The key is not in the expected ASN1 structure.";
                throw new CryptoException(message, exception);
            }
            var D = new BigInteger(1, d);
            return new ECPrivateKeyParameters("EC", D, derObject);
        }

       
        #endregion
    }
}
