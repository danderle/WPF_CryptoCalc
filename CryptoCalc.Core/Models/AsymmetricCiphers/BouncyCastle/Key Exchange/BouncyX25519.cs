using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Agreement;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CryptoCalc.Core
{
    public class BouncyX25519 : IAsymmetricKeyExchange, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPairA;
        private AsymmetricCipherKeyPair keyPairB;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyX25519()
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
            int maxStrength = 255;
            int multiple = 255;
            int strength = 255;
            for (; strength <= maxStrength; strength += multiple)
            {
                keySizes.Add(strength);
            }
            return keySizes;
        }

        /// <summary>
        /// Create a key pair for according to the key size. 
        /// For X25519 the key size is not relevant, it always has a strength of 255 bits.
        /// </summary>
        /// <param name="keySize">the key size in bits</param>
        public void CreateKeyPair(int keySize = 255)
        {
            var parameters = new X25519KeyGenerationParameters(new SecureRandom());
            var keyGenerator = new X25519KeyPairGenerator();
            keyGenerator.Init(parameters);
            keyPairA = keyGenerator.GenerateKeyPair();
            keyPairB = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var keyBytes = ((X25519PrivateKeyParameters)keyPairA.Private).GetEncoded();
            var privateKey = new List<byte>();
            privateKey.AddRange(keyBytes);

            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var keyBytes = ((X25519PublicKeyParameters)keyPairA.Public).GetEncoded();
            var publicKey = new List<byte>();
            publicKey.AddRange(keyBytes);
            return publicKey.ToArray();
        }

        /// <summary>
        /// Derives a shared secret key from a private key and another persons public key
        /// </summary>
        /// <param name="myPrivateKey">the private key which is used</param>
        /// <param name="otherPartyPublicKey">the public key of the other person</param>
        /// <returns></returns>
        public byte[] DeriveKey(byte[] myPrivateKey, byte[] otherPartyPublicKey)
        {
            var a1 = new X25519Agreement();
            
            var privateKey = CreatePrivateKeyParameterFromBytes(myPrivateKey);
            a1.Init(privateKey);
            
            byte[] k1 = new byte[a1.AgreementSize];

            var publicKey = CreatePublicKeyParameterFromBytes(otherPartyPublicKey);
            a1.CalculateAgreement(publicKey, k1, 0);

            return k1;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a public key <see cref="X25519PublicKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The public key parameter object</returns>
        private X25519PublicKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            return new X25519PublicKeyParameters(publicKey, 0);
        }

        /// <summary>
        /// Creates a private key <see cref="X25519PrivateKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="privateKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private key parameter object</returns>
        private X25519PrivateKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            return new X25519PrivateKeyParameters(privateKey, 0);
        }

        
        #endregion
    }
}
