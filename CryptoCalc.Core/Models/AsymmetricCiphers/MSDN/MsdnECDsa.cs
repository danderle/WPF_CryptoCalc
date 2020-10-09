using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The ECDsa algorithim provided by the MSDN library
    /// </summary>
    public class MsdnECDsa : IAsymmetricSignature, IECAlgorithims
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

        #region Public Properties

        

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnECDsa()
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
            cipher.ImportPkcs8PrivateKey(privKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);
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
            cipher.ImportSubjectPublicKeyInfo(pubKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);
            return cipher.VerifyHash(hash, originalSignature);
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
