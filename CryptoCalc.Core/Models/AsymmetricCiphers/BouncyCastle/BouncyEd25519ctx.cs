using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
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

namespace CryptoCalc.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class BouncyEd25519ctx : IAsymmetricSignature, IECAlgorithims
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
        public bool UsesCurves => true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyEd25519ctx()
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
            var list = new ObservableCollection<string> { EcCurveProvider.GOST3410.ToString() };
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
                case EcCurveProvider.NIST:
                case EcCurveProvider.TELETRUST:
                case EcCurveProvider.ANSSI:
                case EcCurveProvider.GM:
                case EcCurveProvider.GOST3410:
                    curves = Gost3410NamedParameters.Names.GetEnumerator();
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
            var oid = Gost3410NamedParameters.GetOid(curveName);
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
            var der = ((Gost3410PrivateKeyParameters)keyPair.Private).PublicKeyParamSet.ToAsn1Object().GetDerEncoded();
            var x = ((Gost3410PrivateKeyParameters)keyPair.Private).X.ToByteArrayUnsigned();
            var privateKey = new List<byte>();
            privateKey.AddRange(der);
            privateKey.AddRange(x);

            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var der = ((Gost3410PublicKeyParameters)keyPair.Public).PublicKeyParamSet.ToAsn1Object().GetDerEncoded();
            var Y = ((Gost3410PublicKeyParameters)keyPair.Public).Y.ToByteArrayUnsigned();
            var publicKey = new List<byte>();
            publicKey.AddRange(der);
            publicKey.AddRange(Y);

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
            var signer = new Gost3410Signer();
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
            var signer = new Gost3410Signer();
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
        /// Creates a public key <see cref="Gost3410PublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private Gost3410PublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            //Get length of the DER ecnoded bytes plus 1 for the tag and length of the tlv
            var der = new byte[publicKey[1] + 2];
            int restLength = publicKey.Length - der.Length;
            var y = new byte[restLength];

            Array.Copy(publicKey, der, der.Length);
            Array.Copy(publicKey, der.Length, y, 0, y.Length);

            //Get the der object identifierer
            var derOid = DerObjectIdentifier.GetInstance(Asn1Object.FromByteArray(der));

            //Get the Y coordinates and then create the ECPoint
            var Y = new BigInteger(1, y);

            return new Gost3410PublicKeyParameters(Y, derOid);
        }

        /// <summary>
        /// Creates a private key <see cref="Gost3410PrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="privateKey">the byte array containing the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private Gost3410PrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            //Get length of the DER ecnoded bytes plus 1 for the tag and length of the tlv
            var der = new byte[privateKey[1] + 2];
            int restLength = privateKey.Length - der.Length;
            var x = new byte[restLength];

            Array.Copy(privateKey, der, der.Length);
            Array.Copy(privateKey, der.Length, x, 0, x.Length);

            var derObject = DerObjectIdentifier.GetInstance(Asn1Object.FromByteArray(der));
            var X = new BigInteger(1, x);
            return new Gost3410PrivateKeyParameters(X, derObject);
        }

        #endregion
    }
}
