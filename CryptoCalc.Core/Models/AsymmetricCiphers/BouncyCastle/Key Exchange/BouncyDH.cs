using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CryptoCalc.Core
{
    public class BouncyDH : IAsymmetricKeyExchange, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPair;

        private AsymmetricCipherKeyPair otherKeyPair;

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
            
            otherKeyPair = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var x = ((DHPrivateKeyParameters)keyPair.Private).X.ToByteArrayUnsigned();
            var p = ((DHPrivateKeyParameters)keyPair.Private).Parameters.P.ToByteArrayUnsigned();
            var g = ((DHPrivateKeyParameters)keyPair.Private).Parameters.G.ToByteArrayUnsigned();
            var q = ((DHPrivateKeyParameters)keyPair.Private).Parameters.Q.ToByteArrayUnsigned();
            var m = ((DHPrivateKeyParameters)keyPair.Private).Parameters.M;
            var l = ((DHPrivateKeyParameters)keyPair.Private).Parameters.L;
            var privateKey = new List<byte>();
            privateKey.AddRange(x);
            privateKey.AddRange(p);
            privateKey.AddRange(g);
            privateKey.AddRange(q);
            privateKey.Add(Convert.ToByte(m));
            privateKey.Add(Convert.ToByte(l));
            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var y = ((DHPublicKeyParameters)keyPair.Public).Y.ToByteArrayUnsigned();
            var p = ((DHPublicKeyParameters)keyPair.Public).Parameters.P.ToByteArrayUnsigned();
            var g = ((DHPublicKeyParameters)keyPair.Public).Parameters.G.ToByteArrayUnsigned();
            var q = ((DHPublicKeyParameters)keyPair.Public).Parameters.Q.ToByteArrayUnsigned();
            var m = ((DHPublicKeyParameters)keyPair.Public).Parameters.M;
            var l = ((DHPublicKeyParameters)keyPair.Public).Parameters.L;
            var publicKey = new List<byte>();
            publicKey.AddRange(y);
            publicKey.AddRange(p);
            publicKey.AddRange(g);
            publicKey.AddRange(q);
            publicKey.Add(Convert.ToByte(m));
            publicKey.Add(Convert.ToByte(l));
            return publicKey.ToArray();
        }

        public byte[] DeriveKey(byte[] myPrivateKey, int cipherKeySize, byte[] otherPartyPublicKey)
        {
            //var privKey = CreatePrivateKeyParameterFromBytes(myPrivateKey);
            //var agreement = new DHAgreement();
            //agreement.Init(privKey);
            //var message = agreement.CalculateMessage();
            //var otherPubKey = CreatePublicKeyParameterFromBytes(otherPartyPublicKey);
            //var sharedagreement = agreement.CalculateAgreement(otherPubKey, message);

            var a1 = new DHAgreement();
            var a2 = new DHAgreement();
            var priv = CreatePrivateKeyParameterFromBytes(myPrivateKey);

            a1.Init(keyPair.Private);
            a2.Init(otherKeyPair.Private);

            BigInteger m1 = a1.CalculateMessage();
            BigInteger m2 = a2.CalculateMessage();

            var pubKey = CreatePublicKeyParameterFromBytes(GetPublicKey());
            var param = pubKey.Parameters;
            if(!param.Equals(priv.Parameters))
            {
                string fail = "fail";
            }

            //Both party keys must share the same DHParameters to be able to calculate the agreement
            BigInteger k1 = a1.CalculateAgreement((DHPublicKeyParameters)otherKeyPair.Public, m2);
            BigInteger k2 = a2.CalculateAgreement((DHPublicKeyParameters)keyPair.Public, m1);

            if (!k1.Equals(k2))
            {
                string fail = "fail";
            }

            return k1.ToByteArrayUnsigned();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="DHPublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private DHPublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            int restLength = publicKey.Length - 2;
            int arraySize = restLength / 4;
            var y = new byte[arraySize];
            var p = new byte[arraySize];
            var g = new byte[arraySize];
            var q = new byte[arraySize];

            Array.Copy(publicKey, y, y.Length);
            Array.Copy(publicKey, y.Length, p, 0, p.Length);
            Array.Copy(publicKey, y.Length + p.Length, g, 0, g.Length);
            Array.Copy(publicKey, y.Length + p.Length + g.Length, q, 0, q.Length);
            var M = Convert.ToInt32(publicKey[publicKey.Length - 2]);
            var L = Convert.ToInt32(publicKey[publicKey.Length - 1]);

            var Y = new BigInteger(1, y);
            var P = new BigInteger(1, p);
            var G = new BigInteger(1, g);
            var Q = new BigInteger(1, q);
            var dhParams = new DHParameters(P, G, Q, M, L);
            return new DHPublicKeyParameters(Y, dhParams);
        }

        /// <summary>
        /// Creates a private key <see cref="DHPrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private DHPrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            int restLength = privateKey.Length - 2;
            int arraySize = restLength / 4;
            var x = new byte[arraySize];
            var p = new byte[arraySize];
            var g = new byte[arraySize];
            var q = new byte[arraySize];

            Array.Copy(privateKey, x, x.Length);
            Array.Copy(privateKey, x.Length, p, 0, p.Length);
            Array.Copy(privateKey, x.Length + p.Length, g, 0, g.Length);
            Array.Copy(privateKey, x.Length + p.Length + g.Length, q, 0, q.Length);
            var M = Convert.ToInt32(privateKey[privateKey.Length - 2]);
            var L = Convert.ToInt32(privateKey[privateKey.Length - 1]);

            var X = new BigInteger(1, x);
            var P = new BigInteger(1, p);
            var G = new BigInteger(1, g);
            var Q = new BigInteger(1, q);
            var dhParams = new DHParameters(P, G, Q, M, L);
            return new DHPrivateKeyParameters(X, dhParams);
        }
        
        #endregion
    }
}
