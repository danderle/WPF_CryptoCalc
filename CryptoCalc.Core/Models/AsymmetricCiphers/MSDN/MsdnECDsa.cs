using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace CryptoCalc.Core
{
    public class MsdnECDsa : IAsymmetricSignature
    {
        #region Private Fields

        /// <summary>
        /// The cipher algorithim for this class
        /// </summary>
        public ECDsa cipher { get; set; } = ECDsa.Create();

        #endregion

        #region Public Properties

        public byte[] PublicKey { get; set; }
        public byte[] PrivateKey { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnECDsa()
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


        public void CreateKeyPair(int keySize)
        {
            cipher = ECDsa.Create("ECDsa");
            cipher.KeySize = keySize;
        }

        public byte[] Sign(byte[] privKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportPkcs8PrivateKey(privKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);
            return cipher.SignHash(hash);
        }

        public bool Verify(byte[] originalSignature, byte[] pubKey, byte[] data)
        {
            int bytesRead;
            cipher.ImportSubjectPublicKeyInfo(pubKey, out bytesRead);
            var hash = MsdnHash.Compute(MsdnHashAlgorithim.SHA512, data);
            return cipher.VerifyHash(hash, originalSignature);
        }

        public byte[] GetPrivateKey()
        {
            return cipher.ExportPkcs8PrivateKey();
        }

        public byte[] GetPublicKey()
        {
            return cipher.ExportSubjectPublicKeyInfo();
        }

        #endregion
    }
}
