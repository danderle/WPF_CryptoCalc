using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
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
    public class BouncyRsa : IAsymmetricEncryption, IAsymmetricSignature, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The cipher object for this class
        /// </summary>
        private IAsymmetricBlockCipher cipher = new RsaEngine();
        
        /// <summary>
        /// The generated key pair object for this class
        /// </summary>
        private AsymmetricCipherKeyPair keyPair;

        #endregion

        #region Public Properties

        /// <summary>
        /// A flag for knowing if the algorithim uses elliptical curves
        /// </summary>
        public bool UsesCurves => false;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyRsa()
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
            int maxStrength = 16384;
            for(int strength = 384; strength < maxStrength; strength += 512)
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
            var parameters = new KeyGenerationParameters(new SecureRandom(), keySize);
            var keyGenerator = new RsaKeyPairGenerator();
            keyGenerator.Init(parameters);
            keyPair = keyGenerator.GenerateKeyPair();
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            var exponent = ((RsaKeyParameters)keyPair.Private).Exponent.ToByteArrayUnsigned();
            var modulus = ((RsaKeyParameters)keyPair.Private).Modulus.ToByteArrayUnsigned();
            var privateKey = new List<byte>();
            privateKey.AddRange(exponent);
            privateKey.AddRange(modulus);
            return privateKey.ToArray();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            var exponent = ((RsaKeyParameters)keyPair.Public).Exponent.ToByteArrayUnsigned();
            var modulus = ((RsaKeyParameters)keyPair.Public).Modulus.ToByteArrayUnsigned();
            var publicKey = new List<byte>();
            publicKey.AddRange(exponent);
            publicKey.AddRange(modulus);
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
            cipher.Init(true, pubKey);
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
            return cipher.ProcessBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privateKey, byte[] data)
        {
            var signer = new RsaDigestSigner(new Sha1Digest());
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
            var signer = new RsaDigestSigner(new Sha1Digest());
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
        private RsaKeyParameters CreatePublicKeyParameterFromBytes(byte[] publicKey)
        {
            //exponent contains only the first 3 bytes
            var e = new byte[3];
            Array.Copy(publicKey, e, 3);
            var m = new byte[publicKey.Length - 3];
            Array.Copy(publicKey, 3, m, 0, publicKey.Length - 3);
            var exponent = new BigInteger(1, e);
            var modulus = new BigInteger(1, m);
            return new RsaKeyParameters(false, modulus, exponent);
        }

        /// <summary>
        /// Creates a private key <see cref="RsaKeyParameters"/> from a byte array containing the exponent and modulus
        /// </summary>
        /// <param name="publicKey">the byte array conatining the exponent and the modulus</param>
        /// <returns>The private RSA key parameter object</returns>
        private RsaKeyParameters CreatePrivateKeyParameterFromBytes(byte[] privateKey)
        {
            //the exponent and modulus are equal in length
            var e = new byte[privateKey.Length / 2];
            Array.Copy(privateKey, e, e.Length);
            var m = new byte[privateKey.Length / 2];
            Array.Copy(privateKey, e.Length, m, 0, m.Length);
            var modulus = new BigInteger(1, m);
            var exponent = new BigInteger(1, e);
            return new RsaKeyParameters(true, modulus, exponent);
        }

        #endregion
    }
}
