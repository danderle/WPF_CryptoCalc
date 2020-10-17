using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The ECDsa algorithim provided by the MSDN library
    /// </summary>
    public class MsdnECDsa : BaseMsdnAsymmetric, IAsymmetricSignature, IECAlgorithims
    {
        #region Private Fields

        /// <summary>
        /// Holds all the available ec curves
        /// </summary>
        Dictionary<string, ECCurve> ecCurves = new Dictionary<string, ECCurve>();

        /// <summary>
        /// The cipher algorithim object for this class
        /// </summary>
        private ECDsa cipher { get; set; } = ECDsa.Create();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnECDsa()
        {
            ecCurves = GetEcCurves();
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
                EcCurveProvider.TELETRUST.ToString(),
                EcCurveProvider.NIST.ToString(),
            };
        }

        /// <summary>
        /// Ges a list ofr all available ec curves
        /// </summary>
        /// <returns>the list of all ec curves</returns>
        public ObservableCollection<string> GetEcCurves(EcCurveProvider provider)
        {
            var list = new ObservableCollection<string>();
            switch(provider)
            {
                case EcCurveProvider.NIST:
                    foreach (var key in ecCurves.Keys)
                    {
                        if (key.Contains("nist"))
                        {
                            list.Add(key);
                        }
                    }
                    break;
                case EcCurveProvider.TELETRUST:
                    foreach(var key in ecCurves.Keys)
                    {
                        if (key.Contains("brain"))
                        {
                            list.Add(key);
                        }
                    }
                    break;
            }
            return list;
        }

        /// <summary>
        /// Create a key pair by using a given ec curve
        /// </summary>
        /// <param name="curveName">the curve to use for key creation</param>
        public void CreateKeyPair(string curveName)
        {
            ECCurve curve;
            ecCurves.TryGetValue(curveName, out curve);
            cipher = ECDsa.Create(curve);
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

        /// <summary>
        /// Signs the passed in data with a private key
        /// </summary>
        /// <param name="privateKey">the private key used to create the signature</param>
        /// <param name="data">The data to sign</param>
        /// <returns>the signature as a byte array</returns>
        public byte[] Sign(byte[] privKey, byte[] data)
        {
            int bytesRead;
            try
            {
                //import private key bytes
                cipher.ImportPkcs8PrivateKey(privKey, out bytesRead);
            }
            catch (CryptographicException exception)
            {
                string message = "Signature Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source do not represent an ASN.1-BER-encoded PKCS#8 private key structure.\n" +
                    "-or- The contents of source indicate the key is for an algorithm other than the algorithm represented by this instance.\n" +
                    "-or- The contents of source represent the key in a format that is not supported.\n" +
                    "-or- The algorithm-specific key import failed.\n";
                throw new CryptographicException(message, exception);
            }
            catch (PlatformNotSupportedException exception)
            {
                string message = "Signature Failed!\n" +
                    $"{exception.Message}\n" +
                    "The public key is corrupted.\n" +
                    "Verify the public key.";
                throw new CryptographicException(message, exception);
            }
            catch (Exception exception)
            {
                string message = "Signature Failed!\n" +
                        $"{exception.Message}\n" +
                        "Contact developer.";
                throw new CryptographicException(message, exception);
            }

            //hash data
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);

            //sign hash
            return cipher.SignHash(hash);
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
            try
            {
                //Import public key bytes
                cipher.ImportSubjectPublicKeyInfo(pubKey, out bytesRead);
            }
            catch(CryptographicException exception)
            {
                string message = "Signature Verification Failed!\n" +
                    $"{exception.Message}\n" +
                    "The contents of source do not represent an ASN.1-DER-encoded X.509 public key structure.\n" +
                    "-or- The contents of source indicate the key is for an algorithm other than the algorithm represented by this instance.\n" +
                    "-or- The contents of source represent the key in a format that is not supported.\n" +
                    "-or- The algorithm-specific key import failed.\n";
                throw new CryptographicException(message, exception);
            }
            catch(PlatformNotSupportedException exception)
            {
                string message = "Signature Verification Failed!\n" +
                    $"{exception.Message}\n" +
                    "The public key is corrupted.\n" +
                    "Verify the public key.";
                throw new CryptographicException(message, exception);
            }
            catch(Exception exception)
            {
                string message = "Signature Verification Failed!\n" +
                        $"{exception.Message}\n" +
                        "Contact developer.";
                throw new CryptographicException(message, exception);
            }

            //hash data
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);

            //verify signature
            return cipher.VerifyHash(hash, originalSignature);
        }

        #endregion
    }
}
