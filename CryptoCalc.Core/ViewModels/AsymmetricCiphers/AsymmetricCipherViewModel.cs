using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the Asymmetric cipher page
    /// </summary>
    public class AsymmetricCipherViewModel : BaseViewModel
    {
        #region Private fields

        public string pKeyPath = @"C:\\Users\\beach\\Desktop\\Encryption tests";

        #endregion

        #region Public Properties

        /// <summary>
        /// The file path to encrypt
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The plain text to encrypt
        /// </summary>
        public string PlainText { get; set; }

        /// <summary>
        /// The encrypted file path
        /// </summary>
        public string EncryptedFilePath { get; set; }

        /// <summary>
        /// The decrypted file path
        /// </summary>
        public string DecryptedFilePath { get; set; }

        /// <summary>
        /// The encrypted text
        /// </summary>
        public string EncryptedText { get; set; }

        /// <summary>
        /// The decrypted text
        /// </summary>
        public string DecryptedText { get; set; }

        /// <summary>
        /// The original signature
        /// </summary>
        public string OriginalSignature { get; set; }

        /// <summary>
        /// The verified signature
        /// </summary>
        public bool SignatureVerified { get; set; }


        /// <summary>
        /// The password which will be hashed to set the secret key and iv
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string KeyName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PrivateKeyPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PublicKeyPath { get; set; }

        /// <summary>
        /// The currently selected key size index
        /// </summary>
        public int KeySizeIndex { get; set; }

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; }

        /// <summary>
        /// Currently selected algorithim
        /// </summary>
        public int SelectedAlgorithim { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CryptographyApi SelectedCryptoApi { get; set; }

        /// <summary>
        /// The list off all symmetric algorithims
        /// </summary>
        public List<string> Algorithims { get; set; } = new List<string>();
        
        /// <summary>
        /// Holds the data format options
        /// </summary>
        public List<string> DataFormatOptions { get; set; } = new List<string>();

        /// <summary>
        /// Holds the crypto api options
        /// </summary>
        public List<string> CryptoApiOptions { get; set; } = new List<string>();

        /// <summary>
        /// The list of all the different key size options
        /// </summary>
        public ObservableCollection<int> KeySizes { get; set; } = new ObservableCollection<int>();

        public IAsymmetricCipher Cipher { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to encrypt
        /// </summary>
        public ICommand EncryptCommand { get; set; }

        /// <summary>
        /// The command to decrypt
        /// </summary>
        public ICommand DecryptCommand { get; set; }

        /// <summary>
        /// The command to drop files
        /// </summary>
        public ICommand DropCommand { get; set; }

        /// <summary>
        /// The command to when a cipher algorithim is selected
        /// </summary>
        public ICommand ChangedAlgorithimCommand { get; set; }

        /// <summary>
        /// The command to when the api is changed
        /// </summary>
        public ICommand ChangedApiCommand { get; set; }

        public ICommand CreateKeyPairCommand { get; set; }
        public ICommand SaveKeyPairCommand { get; set; }
        public ICommand DeleteKeyPairCommand { get; set; }
        public ICommand SignCommand { get; set; }
        public ICommand VerifyCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AsymmetricCipherViewModel()
        {
            //Initialize the commands
            InitializeCommands();

            CryptoApiOptions = Enum.GetValues(typeof(CryptographyApi)).Cast<CryptographyApi>().Select(t => t.ToString()).ToList();
            Algorithims = IAsymmetricCipher.GetMsdnAlgorthims();
            SelectedAlgorithim = (int)AsymmetricMsdnCiphers.RSA;
            Cipher = IAsymmetricCipher.GetCipher(AsymmetricMsdnCiphers.RSA);
            KeySizes = Cipher.GetKeySizes(SelectedAlgorithim);

            DataFormatOptions.Add(Format.File.ToString());
            DataFormatOptions.Add(Format.TextString.ToString());
            DataFormatSelected = Format.TextString;
        }

        private void InitializeCommands()
        {
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            ChangedApiCommand = new RelayCommand(ChangedApi);
            CreateKeyPairCommand = new RelayCommand(CreateKeyPair);
            SaveKeyPairCommand = new RelayCommand(SaveKeyPair);
            DeleteKeyPairCommand = new RelayCommand(DeleteKeyPair);
            SignCommand = new RelayCommand(Sign);
            VerifyCommand = new RelayCommand(Verify);
        }

        private void Verify()
        {
            var pubKey = File.ReadAllBytes(PublicKeyPath);
            var data = ByteConvert.StringToAsciiBytes(PlainText);
            var signature = ByteConvert.HexStringToBytes(OriginalSignature);
            SignatureVerified = Cipher.Verify(signature, pubKey, data);
        }

        private void Sign()
        {
            var privKey = File.ReadAllBytes(PrivateKeyPath);
            var data = ByteConvert.StringToAsciiBytes(PlainText);
            var signature = Cipher.Sign(privKey, data);
            OriginalSignature = ByteConvert.BytesToHexString(signature);
        }

        private void DeleteKeyPair()
        {
            if(File.Exists(PrivateKeyPath))
            {
                File.Delete(PrivateKeyPath);
            }
            if (File.Exists(PublicKeyPath))
            {
                File.Delete(PublicKeyPath);
            }
        }

        private void SaveKeyPair()
        {
            File.WriteAllBytes(PrivateKeyPath, Cipher.PrivateKey);
            File.WriteAllBytes(PublicKeyPath, Cipher.PublicKey);
        }

        private void CreateKeyPair()
        {
            PrivateKeyPath = pKeyPath + "\\" + KeyName + "_PrivateKey.pkcs1";
            PublicKeyPath = pKeyPath + "\\" + KeyName + "_PublicKey.pkcs1";
            Cipher.CreateKeyPair(KeySizes[KeySizeIndex]);
            
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// 
        /// </summary>
        private void ChangedApi()
        {
            switch(SelectedCryptoApi)
            {
                case CryptographyApi.MSDN:
                    Algorithims = IAsymmetricCipher.GetMsdnAlgorthims();
                    break;
                case CryptographyApi.BouncyCastle:
                    Algorithims = IAsymmetricCipher.GetMsdnAlgorthims();
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            SelectedAlgorithim = 0;
        }

        /// <summary>
        /// The command method when a different algrithim is selected
        /// </summary>
        private void ChangedAlgorithim()
        {
            Cipher = IAsymmetricCipher.GetCipher((AsymmetricMsdnCiphers)SelectedAlgorithim);
            KeySizes = Cipher.GetKeySizes(SelectedAlgorithim);
            KeySizeIndex = 0;
        }

        /// <summary>
        /// The command method to decrypt the given data
        /// </summary>
        private void Decrypt()
        {
            byte[] encrypted;
            var password = ByteConvert.StringToAsciiBytes(Password);
            
            switch (DataFormatSelected)
            {
                case Format.File:
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);
                    var decryptedBytes = Cipher.DecryptToBytes(SelectedAlgorithim, KeySizes[KeySizeIndex], password, encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    var privateKey = File.ReadAllBytes(PrivateKeyPath);
                    DecryptedText = Cipher.DecryptToText(privateKey, encrypted);
                    break;
            }
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            byte[] encrypted;
            var password = ByteConvert.StringToAsciiBytes(Password);
            
            switch (DataFormatSelected)
            {
                case Format.File:
                    if (File.Exists(FilePath))
                    {
                        var plainBytes = File.ReadAllBytes(FilePath);

                        var encryptedBytes = Cipher.EncryptBytes(SelectedAlgorithim, KeySizes[KeySizeIndex], password, plainBytes);
                        var extension = Path.GetExtension(FilePath);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(FilePath).ToString(), Path.GetFileNameWithoutExtension(FilePath) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                break;
                case Format.TextString:
                    var pubKey = File.ReadAllBytes(PublicKeyPath);
                    encrypted = Cipher.EncryptText(pubKey, PlainText);
                    EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    break;
            }
        }

        #endregion

        #region Private Methods

        
        #endregion
    }
}
