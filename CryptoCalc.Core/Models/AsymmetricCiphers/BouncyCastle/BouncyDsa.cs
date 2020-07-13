using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class BouncyDsa : IAsymmetricSignature, INonECAlgorithims
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
        public bool UsesEcCurves => false;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyDsa()
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
            int maxStrength = 1024;
            int multiple = 64;
            int strength = 512;
            for(; strength <= maxStrength; strength += multiple)
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
            var dsaParametersGenerator = new DsaParametersGenerator();
            dsaParametersGenerator.Init(keySize, 80, new SecureRandom());
            var parameters = new DsaKeyGenerationParameters(new SecureRandom(), dsaParametersGenerator.GenerateParameters());
            var keyGenerator = new DsaKeyPairGenerator();
            keyGenerator.Init(parameters);
            keyPair = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var x = ((DsaPrivateKeyParameters)keyPair.Private).X.ToByteArrayUnsigned();
            var p = ((DsaKeyParameters)keyPair.Private).Parameters.P.ToByteArrayUnsigned();
            var q = ((DsaKeyParameters)keyPair.Private).Parameters.Q.ToByteArrayUnsigned();
            var g = ((DsaKeyParameters)keyPair.Private).Parameters.G.ToByteArrayUnsigned();
            
            var privateKey = new List<byte>();
            privateKey.AddRange(x);
            privateKey.AddRange(p);
            privateKey.AddRange(q);
            privateKey.AddRange(g);
            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var y = ((DsaPublicKeyParameters)keyPair.Public).Y.ToByteArrayUnsigned();
            var p = ((DsaKeyParameters)keyPair.Public).Parameters.P.ToByteArrayUnsigned();
            var q = ((DsaKeyParameters)keyPair.Public).Parameters.Q.ToByteArrayUnsigned();
            var g = ((DsaKeyParameters)keyPair.Public).Parameters.G.ToByteArrayUnsigned();

            var publicKey = new List<byte>();
            publicKey.AddRange(y);
            publicKey.AddRange(p);
            publicKey.AddRange(q);
            publicKey.AddRange(g);
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
            var signer = new DsaDigestSigner(new DsaSigner(), new Sha1Digest());
            var privKey = CreatePrivateKeyParameterFromBytes(privateKey);
            signer.Init(true, privKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.GenerateSignature();
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
            var signer = new DsaDigestSigner(new DsaSigner(), new Sha1Digest());
            var pubKey = CreatePublicKeyParameterFromBytes(publicKey);
            signer.Init(false, pubKey);
            signer.BlockUpdate(data, 0, data.Length);
            return signer.VerifySignature(originalSignature);
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="RsaKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public RSA key parameter object</returns>
        private DsaPublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            //y, p and g are all the same length. q is 20 byte long
            var q = new byte[20];
            int restLength = publicKey.Length - 20;
            var y = new byte[restLength / 3];
            var p = new byte[restLength / 3];
            var g = new byte[restLength / 3];
            Array.Copy(publicKey, y, y.Length);
            Array.Copy(publicKey, y.Length, p, 0, p.Length);
            Array.Copy(publicKey, y.Length + p.Length, q, 0, q.Length);
            Array.Copy(publicKey, y.Length + p.Length + q.Length, g, 0, g.Length);

            var Y = new BigInteger(1, y);
            var P = new BigInteger(1, p);
            var Q = new BigInteger(1, q);
            var G = new BigInteger(1, g);
            return new DsaPublicKeyParameters(Y, new DsaParameters(P, Q, G));
        }

        /// <summary>
        /// Creates a private key <see cref="RsaKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private RSA key parameter object</returns>
        private DsaPrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            // x and q are always 20 byte long, p and g are always equal in length
            var x = new byte[20];
            var q = new byte[20];
            int restLength = privateKey.Length - 40;
            var p = new byte[restLength / 2];
            var g = new byte[restLength / 2];
            Array.Copy(privateKey, x, x.Length);
            Array.Copy(privateKey, x.Length, p, 0, p.Length);
            Array.Copy(privateKey, x.Length + p.Length, q, 0, q.Length);
            Array.Copy(privateKey, x.Length + p.Length + q.Length, g, 0, g.Length);

            var X = new BigInteger(1, x);
            var P = new BigInteger(1, p);
            var Q = new BigInteger(1, q);
            var G = new BigInteger(1, g);
            return new DsaPrivateKeyParameters(X, new DsaParameters(P,Q,G));
        }

        #endregion
    }
}
