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
    public class SymmetricViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The data to be hashed <see cref="Format"/> for hash data format options
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// The secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

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
        /// The currently selected key size index
        /// </summary>
        public int KeySizeIndex { get; set; }

        /// <summary>
        /// Currently selected algorithim
        /// </summary>
        public int SelectedAlgorithim { get; set; }

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; }

        /// <summary>
        /// The Cryptography api to use
        /// </summary>
        public CryptographyApi Api { get; set; }

        /// <summary>
        /// Holds the data format options
        /// </summary>
        public List<string> DataFormatOptions { get; set; } = new List<string>();

        /// <summary>
        /// The list off all symmetric algorithims
        /// </summary>
        public List<string> Algorithims { get; set; } = new List<string>();

        /// <summary>
        /// The list of all the different key size options
        /// </summary>
        public ObservableCollection<int> KeySizes { get; set; } = new ObservableCollection<int>();

        #endregion

        #region Commands

        /// <summary>
        /// The command to open a folder dialog window
        /// </summary>
        public ICommand OpenFolderDialogCommand { get; set; }

        /// <summary>
        /// The command to when a cipher algorithim is selected
        /// </summary>
        public ICommand ChangedAlgorithimCommand { get; set; }

        /// <summary>
        /// The command to encrypt
        /// </summary>
        public ICommand EncryptCommand { get; set; }

        /// <summary>
        /// The command to decrypt
        /// </summary>
        public ICommand DecryptCommand { get; set; }

        #endregion

        #region Constructor
        public SymmetricViewModel()
        {
            //Initialize commands
            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogAsync);
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);

        }
        /// <summary>
        /// Default constructor
        /// </summary>
        public SymmetricViewModel(CryptographyApi api)
            : base()
        {
            Api = api;
            switch(Api)
            {
                case CryptographyApi.MSDN:
                    Algorithims = Enum.GetValues(typeof(SymmetricMsdnCipher)).Cast<SymmetricMsdnCipher>().Select(t => t.ToString()).ToList();
                    SelectedAlgorithim = (int)SymmetricMsdnCipher.RC2;
                    KeySizes = SymmetricCipher.GetKeySizes(CryptographyApi.MSDN, SelectedAlgorithim);
                    break;
                case CryptographyApi.BouncyCastle:
                    Algorithims = Enum.GetValues(typeof(SymmetricBouncyCastleCipher)).Cast<SymmetricBouncyCastleCipher>().Select(t => t.ToString()).ToList();
                    SelectedAlgorithim = (int)SymmetricBouncyCastleCipher.THREEFISH_1024;
                    KeySizes = SymmetricCipher.GetKeySizes(CryptographyApi.BouncyCastle, SelectedAlgorithim);
                    break;
                default:
                    Debugger.Break();
                    break;
            }

            // Adds the formats to the lists
            DataFormatOptions.Add(Format.File.ToString());
            DataFormatOptions.Add(Format.TextString.ToString());
        }


        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to show a Folder dialog window and saves the selected file path
        /// </summary>
        private async void OpenFolderDialogAsync()
        {
            await Ioc.UI.ShowFolderDialog(new FolderBrowserDialogViewModel());
            Data = Ioc.Application.FilePathFromDialogSelection;

        }

        /// <summary>
        /// The command method when a different algrithim is selected
        /// </summary>
        private void ChangedAlgorithim()
        {
            KeySizes = SymmetricCipher.GetKeySizes(Api, SelectedAlgorithim);
            KeySizeIndex = 0;
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            byte[] encrypted;
            var secretKey = ByteConvert.HexStringToBytes(SecretKey);

            switch (DataFormatSelected)
            {
                case Format.File:
                    if (File.Exists(Data))
                    {
                        var plainBytes = File.ReadAllBytes(Data);
                        var encryptedBytes = SymmetricCipher.EncryptBytes(Api, SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, plainBytes);
                        var extension = Path.GetExtension(Data);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(Data).ToString(), Path.GetFileNameWithoutExtension(Data) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                    break;
                case Format.TextString:
                    encrypted = SymmetricCipher.EncryptText(Api, SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, Data);
                    EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    break;
            }
        }

        /// <summary>
        /// The command method to decrypt the given data
        /// </summary>
        private void Decrypt()
        {
            byte[] encrypted;
            var secretKey = ByteConvert.HexStringToBytes(SecretKey);

            switch (DataFormatSelected)
            {
                case Format.File:
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);
                    var decryptedBytes = SymmetricCipher.DecryptToBytes(Api, SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    DecryptedText = SymmetricCipher.DecryptToText(Api, SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, encrypted);
                    break;
            }
        }

        #endregion
    }
}
