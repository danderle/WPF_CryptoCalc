using Org.BouncyCastle.Asn1.X9;
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
    /// Classs for signing and verifing data using the EC Gost3410 asymmetric keys
    /// </summary>
    public class BouncyECGost3410 : BaseBouncyAsymmetric, IAsymmetricSignature, IECAlgorithims
    {
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
            return IECAlgorithims.GetBouncyEcCurves(provider);
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
            ECPrivateKeyParameters privKey = null;
            try
            {
                privKey = (ECPrivateKeyParameters)CreateAsymmetricKeyParameterFromPrivateKeyInfo(privateKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid EC key parameter\n" +
                    "Verify that the key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }

            var signer = new ECGost3410Signer();
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
            ECPublicKeyParameters pubKey = null;
            try
            {
                pubKey = (ECPublicKeyParameters)CreateAsymmetricKeyParameterFromPublicKeyInfo(publicKey);
            }
            catch (InvalidCastException exception)
            {
                string message = "Public Key Import Failed!\n" +
                    $"{exception.Message}.\n" +
                    "The contents of the source do not represent a valid EC key parameter\n" +
                    "Verify that the key is not corrupted.\n" +
                    "- or - Verify that the correct key is selected.";
                throw new CryptoException(message, exception);
            }

            var signer = new ECGost3410Signer();
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
