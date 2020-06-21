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
    /// The view model for the symmetric cipher page
    /// </summary>
    public class MsdnSymmetricViewModel : BaseViewModel
    {
        #region Private fields

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
        /// The encrypted text
        /// </summary>
        public string EncryptedText { get; set; }

        /// <summary>
        /// The decrypted file path
        /// </summary>
        public string DecryptedFilePath { get; set; }

        /// <summary>
        /// The decrypted text
        /// </summary>
        public string DecryptedText { get; set; }

        /// <summary>
        /// The password which will be hashed to set the secret key and iv
        /// </summary>
        public string Password { get; set; }

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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnSymmetricViewModel()
        {
            //Initialize the commands
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            DropCommand = new RelayParameterizedCommand(Drop);
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            ChangedApiCommand = new RelayCommand(ChangedApi);

            CryptoApiOptions = Enum.GetValues(typeof(CryptographyApi)).Cast<CryptographyApi>().Select(t => t.ToString()).ToList();
            Algorithims = Enum.GetValues(typeof(SymmetricMsdnCipher)).Cast<SymmetricMsdnCipher>().Select(t => t.ToString()).ToList();
            SelectedAlgorithim = (int)SymmetricMsdnCipher.RC2;
            KeySizes = SymmetricCipher.GetKeySizes(SelectedCryptoApi, SelectedAlgorithim);

            DataFormatOptions.Add(Format.File.ToString());
            DataFormatOptions.Add(Format.TextString.ToString());
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
                    Algorithims = SymmetricCipher.GetMSDNAlgorthims();
                    break;
                case CryptographyApi.BouncyCastle:
                    Algorithims = SymmetricCipher.GetBouncyCastleAlgorithims();
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
            KeySizes = SymmetricCipher.GetKeySizes(SelectedCryptoApi, SelectedAlgorithim);
            KeySizeIndex = 0;
        }

        /// <summary>
        /// The command method for dropping files
        /// </summary>
        private void Drop(object obj)
        {
            string[] paths = (string[])obj;
            if (File.Exists(paths[0]))
            {
                FilePath = paths[0];
            }
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
                    var decryptedBytes = SymmetricCipher.DecryptToBytes(SelectedCryptoApi, SelectedAlgorithim, KeySizes[KeySizeIndex], password, encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    DecryptedText = SymmetricCipher.DecryptToText(SelectedCryptoApi, SelectedAlgorithim, KeySizes[KeySizeIndex], password, encrypted);
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
                        var encryptedBytes = SymmetricCipher.EncryptBytes(SelectedCryptoApi, SelectedAlgorithim, KeySizes[KeySizeIndex], password, plainBytes);
                        var extension = Path.GetExtension(FilePath);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(FilePath).ToString(), Path.GetFileNameWithoutExtension(FilePath) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                break;
                case Format.TextString:
                    encrypted = SymmetricCipher.EncryptText(SelectedCryptoApi, SelectedAlgorithim, KeySizes[KeySizeIndex], password, PlainText);
                    EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    break;
            }
        }

        #endregion

        #region Private Methods

        
        #endregion
    }
}
