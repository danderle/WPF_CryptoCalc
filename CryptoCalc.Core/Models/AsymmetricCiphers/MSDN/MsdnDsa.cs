﻿using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    /// <summary>
    /// Classs for signing and verifying using DSA asymmetric keys
    /// </summary>
    public class MsdnDsa : IAsymmetricSignature, INonECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// The cipher object to use for this class
        /// </summary>
        public DSACryptoServiceProvider cipher { get; set; } = new DSACryptoServiceProvider();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnDsa()
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
            foreach (var legalkeySize in cipher.LegalKeySizes)
            {
                int keySize = legalkeySize.MinSize;
                while (keySize <= legalkeySize.MaxSize)
                {
                    keySizes.Add(keySize);
                    if (legalkeySize.SkipSize == 0)
                        break;
                    keySize += legalkeySize.SkipSize;
                }
            }
            return keySizes;
        }

        /// <summary>
        /// Create a key pair for according to the key size
        /// </summary>
        /// <param name="keySize">the key size in bits</param>
        public void CreateKeyPair(int keySize)
        {
            cipher = new DSACryptoServiceProvider(keySize);
        }

        /// <summary>
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportPkcs8PrivateKey(privKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.SignHash(hash, HashAlgorithmName.SHA1.ToString());
        }

        /// <summary>
        /// Verifies a signature to be authentic
        /// </summary>
        /// <param name="originalSignature">The signature which is be verified</param>
        /// <param name="publicKey">the public key used for the verification</param>
        /// <param name="data">the data which is signed</param>
        /// <returns>true if signature is authentic, false if not</returns>
        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportSubjectPublicKeyInfo(pubKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA1, data);
            return cipher.VerifyHash(hash,  HashAlgorithmName.SHA1.ToString(), originalSignature);
        }

        /// <summary>
        /// Returns the private key
        /// </summary>
        /// <returns>private key in bytes</returns>
        public byte[] GetPrivateKey()
        {
            return cipher.ExportPkcs8PrivateKey();
        }

        /// <summary>
        /// Returns the public key
        /// </summary>
        /// <returns>the public key in bytes</returns>
        public byte[] GetPublicKey()
        {
            return cipher.ExportSubjectPublicKeyInfo();
        }

        #endregion
    }
}
