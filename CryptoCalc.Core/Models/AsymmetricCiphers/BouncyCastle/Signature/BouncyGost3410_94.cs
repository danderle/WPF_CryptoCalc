using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Class for signing and verifying using the Gost3410_94 algorithim
    /// </summary>
    public class BouncyGost3410_94 : BaseBouncyAsymmetric, IAsymmetricSignature, IECAlgorithims
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyGost3410_94()
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
            var curves = Gost3410NamedParameters.Names.GetEnumerator();
            var list = new ObservableCollection<string>();
            while(curves.MoveNext())
            {
                list.Add((string)curves.Current);
            }

            //Sort list alphabeticaly
            list = new ObservableCollection<string>(list.OrderBy(x => x).ToList());

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
            return GetPrivateKeyInfo();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            return GetPublicKeyInfo();
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
            var privKey = (Gost3410PrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(privateKey);
            
            try
            {
                signer.Init(true, privKey);
            }
            catch (Exception exception)
            {
                string message = "Private Key Creation Failure!\n" +
                    $"{exception.Message}.\n" +
                    $"The private key file is corrupted, verify private key file or try another key.\n" +
                    $"If all fails create a new key pair.";
                throw new CryptoException(message, exception);
            }

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
            var pubKey = (Gost3410PublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(publicKey);
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
    }
}
