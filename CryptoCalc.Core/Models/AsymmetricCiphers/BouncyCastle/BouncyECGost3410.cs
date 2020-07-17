using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    public class BouncyECGost3410 : IAsymmetricSignature, IECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPair;

        #endregion

        #region Public Properties

        /// <summary>
        /// A flag for knowing if the algorithim uses elliptical curves
        /// </summary>
        public bool UsesEcCurves => true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyECGost3410()
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
            switch(provider)
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
            while(curves.MoveNext())
            {
                list.Add((string)curves.Current);
            }
            return list;
        }

        /// <summary>
        /// Create a key pair for by using a given curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            var oid = ECNamedCurveTable.GetOid(curveName);
            var keyGenerationParameters = new Gost3410KeyGenerationParameters(new SecureRandom(), oid);
            var keyGenerator = new Gost3410KeyPairGenerator();
            keyGenerator.Init(keyGenerationParameters);
            keyPair = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var x = ((Gost3410PrivateKeyParameters)keyPair.Private).X.ToByteArrayUnsigned();
            var der = ((Gost3410PrivateKeyParameters)keyPair.Private).PublicKeyParamSet.ToAsn1Object().GetDerEncoded();
            var privateKey = new List<byte>();
            privateKey.AddRange(x);
            privateKey.AddRange(der);

            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var Y = ((Gost3410PublicKeyParameters)keyPair.Public).Y.ToByteArrayUnsigned();
            var der = ((Gost3410PublicKeyParameters)keyPair.Public).PublicKeyParamSet.ToAsn1Object().GetDerEncoded();
            var publicKey = new List<byte>();
            publicKey.AddRange(Y);
            publicKey.AddRange(der);

            return publicKey.ToArray();
        }


        /// <summary>
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privateKey, byte[] data)
        {
            var signer = new ECDsaSigner();
            var privKey = CreatePrivateKeyParameterFromBytes(privateKey);
            signer.Init(true, privKey);
            var bigIntSig = signer.GenerateSignature(data);
            var signature = new List<byte>();
            signature.AddRange(bigIntSig[0].ToByteArrayUnsigned());
            signature.AddRange(bigIntSig[1].ToByteArrayUnsigned());
            return signature.ToArray();
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
            var signer = new ECDsaSigner();
            var pubKey = CreatePublicKeyParameterFromBytes(publicKey);
            signer.Init(false, pubKey);
            var r = new byte[originalSignature.Length / 2];
            var s = new byte[originalSignature.Length / 2];
            Array.Copy(originalSignature, r, r.Length);
            Array.Copy(originalSignature, r.Length, s, 0, s.Length);
            var R = new BigInteger(1, r);
            var S = new BigInteger(1, s);
            return signer.VerifySignature(data, R, S);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="ECPublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private ECPublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            //der is always 11 bytes long and X, and Y are always equal
            var der = new byte[11];
            int restLength = publicKey.Length - 11;
            var x = new byte[restLength / 2];
            var y = new byte[restLength / 2];
            Array.Copy(publicKey, x, x.Length);
            Array.Copy(publicKey, x.Length, y, 0, y.Length);
            Array.Copy(publicKey, x.Length + y.Length, der, 0, der.Length);

            //Get the der object identifierer
            var derOid = DerObjectIdentifier.GetInstance(Asn1Object.FromByteArray(der));

            //Find the curve for the object identifier
            var x9 = TeleTrusTNamedCurves.GetByOid(derOid);

            //Get the X and Y coordinates and then create the ECPoint
            var X = new BigInteger(1, x);
            var Y = new BigInteger(1, y);
            var ecPoint = x9.Curve.CreatePoint(X, Y);

            return new ECPublicKeyParameters("EC", ecPoint, derOid);
        }

        /// <summary>
        /// Creates a private key <see cref="ECPrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private ECPrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            // der is always 11 bytes long
            var der = new byte[11];
            var d = new byte[privateKey.Length - 11];
            Array.Copy(privateKey, d, d.Length);
            Array.Copy(privateKey, d.Length, der, 0, der.Length);

            var derObject = DerObjectIdentifier.GetInstance(Asn1Object.FromByteArray(der));
            var D = new BigInteger(1, d);
            return new ECPrivateKeyParameters("EC", D, derObject);
        }

        #endregion
    }
}
