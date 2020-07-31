using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CryptoCalc.Core
{
    public class MsdnECDH : IAsymmetricCipher, IECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// Holds all the available ec curves
        /// </summary>
        Dictionary<string, ECCurve> ecCurves = new Dictionary<string, ECCurve>();

        /// <summary>
        /// The cipher object for this class
        /// </summary>
        public ECDiffieHellman cipher { get; set; } = ECDiffieHellman.Create();

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
        public MsdnECDH()
        {
            GetAllAvailableEcCurves();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the available ec curve providers
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<string> GetEcProviders()
        {
            return new ObservableCollection<string>
            {
                EcCurveProvider.NIST.ToString(),
                EcCurveProvider.TELETRUST.ToString(),
            };
        }

        /// <summary>
        /// Ges a list ofr all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves(EcCurveProvider provider)
        {
            return new ObservableCollection<string>(ecCurves.Keys);
        }

        /// <summary>
        /// Create a key pair by using a given ec curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            ECCurve curve;
            ecCurves.TryGetValue(curveName, out curve);
            cipher = ECDiffieHellman.Create(curve);
        }

        public byte[] DeriveKey(byte[] myPrivateKey, int cipherKeySize, byte[] otherPartyPublicKey)
        {
            var myDiffie = ECDiffieHellman.Create( );
            int bytesRead;
            myDiffie.KeySize = cipherKeySize;
            
            myDiffie.ImportPkcs8PrivateKey(myPrivateKey, out bytesRead);
            var otherCipher = ECDiffieHellman.Create();
            otherCipher.KeySize = cipherKeySize;
            otherCipher.ImportSubjectPublicKeyInfo(otherPartyPublicKey, out bytesRead);
            return myDiffie.DeriveKeyMaterial(otherCipher.PublicKey);
        }

        public byte[] EncryptText(byte[] publicKey, string plainText)
        {
            var cipher2 = ECDiffieHellman.Create();
            cipher2.KeySize = cipher.KeySize;
            var pubKey2 = cipher2.ExportSubjectPublicKeyInfo();
            var cipher3 = ECDiffieHellman.Create();
            cipher3.KeySize = cipher.KeySize;
            int bytesRead;
            cipher3.ImportSubjectPublicKeyInfo(publicKey, out bytesRead);
            var cipher2Key = cipher2.DeriveKeyMaterial(cipher3.PublicKey);
            byte[] encryptedMessage;
            byte[] iv;
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = cipher2Key;
                iv = aes.IV;
                // Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
            byte[] plaintextMessage = Encoding.UTF8.GetBytes(plainText);

                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    encryptedMessage = ciphertext.ToArray();
                }
            }
            using (Aes aes = new AesCryptoServiceProvider())
            {
                cipher3.ImportSubjectPublicKeyInfo(pubKey2, out bytesRead);
                aes.Key = cipher.DeriveKeyMaterial(cipher3.PublicKey);
                aes.IV = iv;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        string message = Encoding.UTF8.GetString(plaintext.ToArray());
                        Console.WriteLine(message);
                    }
                }
            }
            return iv;
        }

        public byte[] EncryptBytes(string selectedAlgorithim, int keySize, byte[] plainBytes)
        {
            throw new NotImplementedException();
        }

        public string DecryptToText(byte[] privateKey, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public byte[] DecryptToBytes(string selectedAlgorithim, int keySize, byte[] encrypted)
        {
            throw new NotImplementedException();
        }

        public byte[] Sign(byte[] privKey, byte[] data)
        {
            return null;
        }

        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            return false;
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

        #region Private Methods

        /// <summary>
        /// Creates a a dictionary for all the available ec curves
        /// </summary>
        private void GetAllAvailableEcCurves()
        {
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP160r1), ECCurve.NamedCurves.brainpoolP160r1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP160t1), ECCurve.NamedCurves.brainpoolP160t1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP192r1), ECCurve.NamedCurves.brainpoolP192r1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP192t1), ECCurve.NamedCurves.brainpoolP192t1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP224r1), ECCurve.NamedCurves.brainpoolP224r1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP224t1), ECCurve.NamedCurves.brainpoolP224t1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP256r1), ECCurve.NamedCurves.brainpoolP256r1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP256t1), ECCurve.NamedCurves.brainpoolP256t1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP320r1), ECCurve.NamedCurves.brainpoolP320r1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP320t1), ECCurve.NamedCurves.brainpoolP320t1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP384r1), ECCurve.NamedCurves.brainpoolP384r1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP384t1), ECCurve.NamedCurves.brainpoolP384t1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP512r1), ECCurve.NamedCurves.brainpoolP512r1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.brainpoolP512t1), ECCurve.NamedCurves.brainpoolP512t1);
            ecCurves.Add(nameof(ECCurve.NamedCurves.nistP256), ECCurve.NamedCurves.nistP256);
            ecCurves.Add(nameof(ECCurve.NamedCurves.nistP384), ECCurve.NamedCurves.nistP384);
            ecCurves.Add(nameof(ECCurve.NamedCurves.nistP521), ECCurve.NamedCurves.nistP521);
        }

        #endregion
    }
}
