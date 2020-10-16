using System;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace CryptoCalc.Core
{
    /// <summary>
    /// EC Diffie Hellman key exchange class
    /// </summary>
    public class MsdnECDH : IAsymmetricKeyExchange, IECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// Holds all the available ec curves
        /// </summary>
        private Dictionary<string, ECCurve> ecCurves = new Dictionary<string, ECCurve>();

        /// <summary>
        /// The cipher object for this class
        /// </summary>
        private ECDiffieHellman cipher = ECDiffieHellman.Create();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnECDH()
        {
            ecCurves = IECAlgorithims.GetAllAvailableMsdnEcCurves();
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
            var list = new ObservableCollection<string>();
            string keyName = string.Empty;
            switch (provider)
            {
                case EcCurveProvider.NIST:
                    keyName = "nist";
                    break;
                case EcCurveProvider.TELETRUST:
                    keyName = "brain";
                    break;
            }

            //extract the keys containing the key name
            foreach (var key in ecCurves.Keys)
            {
                if (key.Contains(keyName))
                {
                    list.Add(key);
                }
            }
            return list;
        }

        /// <summary>
        /// Create a key pair by using a given ec curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            ecCurves.TryGetValue(curveName, out ECCurve curve);
            cipher = ECDiffieHellman.Create(curve);
        }

        /// <summary>
        /// Derives a shared key using the our private and anothers public keys
        /// </summary>
        /// <param name="myPrivateKey"></param>
        /// <param name="otherPartyPublicKey"></param>
        /// <returns></returns>
        public byte[] DeriveKey(byte[] myPrivateKey, byte[] otherPartyPublicKey)
        {
            var myDiffie = ECDiffieHellman.Create();
            ECCurve curve;
            try
            {
                myDiffie.ImportPkcs8PrivateKey(myPrivateKey, out _);
                curve = myDiffie.ExportParameters(false).Curve;
                
            }
            catch(CryptographicException exception)
            {
                string message = "Private Key Import Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source do not represent an ASN.1 - BER - encoded PKCS#8 PrivateKeyInfo structure.\n" +
                    "-or- The contents of source indicate the key is for an other algorithm.\n" +
                    "-or- The contents of source represent the key in a format that is not supported.\n" +
                    "-or- The algorithm-specific key import failed.\n" +
                    "Verify that both keys for";
                throw new CryptographicException(message, exception); 
            }

            ECDiffieHellman otherCipher = null;
            try
            {
                otherCipher = ECDiffieHellman.Create(curve);
                otherCipher.ImportSubjectPublicKeyInfo(otherPartyPublicKey, out _);
            }
            catch (PlatformNotSupportedException exception)
            {
                string message = "Public Keys Import Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source do not represent an ASN.1 - DER - encoded X.509 SubjectPublicKeyInfo structure.\n" +
                    "- or - The contents of source indicate the key is for an algorithm other than the algorithm represented by this instance.\n" +
                    "- or - The contents of source represent the key in a format that is not supported.\n" +
                    "- or - The algorithm - specific key import failed.";
                  throw new CryptographicException(message, exception);
            }
            catch (CryptographicException exception)
            {
                string message = "Public Keys Import Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source do not represent an ASN.1 - DER - encoded X.509 SubjectPublicKeyInfo structure.\n" +
                    "- or - The contents of source indicate the key is for an algorithm other than the algorithm represented by this instance.\n" +
                    "- or - The contents of source represent the key in a format that is not supported.\n" +
                    "- or - The algorithm - specific key import failed.";
                throw new CryptographicException(message, exception);
            }

            byte[] derivedKey = null;
            try
            {
                derivedKey = myDiffie.DeriveKeyMaterial(otherCipher.PublicKey);
            }
            catch(ArgumentException exception)
            {
                string message = "Key Deriviation Failed!\n" +
                    $"{exception.Message}\n" +
                    "The used key sizes are different.\n" +
                    "-or- The keys were created with different EC curves.";
                throw new CryptographicException(message, exception);
            }
            return derivedKey;
        }

        /// <summary>
        /// Encrypt a plain text m,essage using the public key and deriving a shared key
        /// ONLY A TEST FUNCTION
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        private byte[] EncryptText(byte[] publicKey, string plainText)
        {
            var cipher2 = ECDiffieHellman.Create();
            cipher2.KeySize = cipher.KeySize;
            var pubKey2 = cipher2.ExportSubjectPublicKeyInfo();
            var cipher3 = ECDiffieHellman.Create();
            cipher3.KeySize = cipher.KeySize;
            cipher3.ImportSubjectPublicKeyInfo(publicKey, out _);
            var cipher2Key = cipher2.DeriveKeyMaterial(cipher3.PublicKey);
            byte[] encryptedMessage;
            byte[] iv;
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = cipher2Key;
                iv = aes.IV;

                // Encrypt the message
                using MemoryStream ciphertext = new MemoryStream();
                using CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write);
                var plaintextMessage = Encoding.UTF8.GetBytes(plainText);
                cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                cs.Close();
                encryptedMessage = ciphertext.ToArray();
            }
            using (Aes aes = new AesCryptoServiceProvider())
            {
                cipher3.ImportSubjectPublicKeyInfo(pubKey2, out _);
                aes.Key = cipher.DeriveKeyMaterial(cipher3.PublicKey);
                aes.IV = iv;

                // Decrypt the message
                using MemoryStream plaintext = new MemoryStream();
                using CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                cs.Close();
                string message = Encoding.UTF8.GetString(plaintext.ToArray());
                Console.WriteLine(message);
            }
            return iv;
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
