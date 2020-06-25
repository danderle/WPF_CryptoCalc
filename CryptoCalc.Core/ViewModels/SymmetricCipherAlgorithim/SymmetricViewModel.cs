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

        public bool HasIv => IvSize > 0;

        /// <summary>
        /// The data to be hashed <see cref="Format"/> for hash data format options
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// The secret key
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// The initial value
        /// </summary>
        public string IV { get; set; } = string.Empty;

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
        /// The iv size in bits
        /// </summary>
        public int IvSize { get; set; }

        /// <summary>
        /// Currently selected algorithim
        /// </summary>
        public int SelectedAlgorithim { get; set; } = 0;

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

        public ISymmetricCipher SelectedCipherApi { get; set; }

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

        /// <summary>
        /// Default Constructor
        /// </summary>
        public SymmetricViewModel()
        {
            //Initialize the commands
            InitializeCommands();
        }
        
        /// <summary>
        /// Overloaded constructor
        /// </summary>
        public SymmetricViewModel(CryptographyApi api)
        {
            //Initialize the commands
            InitializeCommands();
            Api = api;
            switch(Api)
            {
                case CryptographyApi.MSDN:
                    SelectedCipherApi = new MsdnSymmetricCipher();
                    break;
                case CryptographyApi.BouncyCastle:
                    SelectedCipherApi = new BouncySymmetricCipher();
                    break;
                default:
                    Debugger.Break();
                    break;
            }

            Algorithims = SelectedCipherApi.GetAlgorthims();
            KeySizes = SelectedCipherApi.GetKeySizes(SelectedAlgorithim);
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);

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
            KeySizes = SelectedCipherApi.GetKeySizes(SelectedAlgorithim);
            KeySizeIndex = 0;
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            byte[] encrypted;
            var secretKey = ByteConvert.HexStringToBytes(SecretKey);
            var iv = ByteConvert.HexStringToBytes(IV);

            switch (DataFormatSelected)
            {
                case Format.File:
                    if (File.Exists(Data))
                    {
                        var plainBytes = File.ReadAllBytes(Data);
                        var encryptedBytes = SelectedCipherApi.EncryptBytes(SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, iv, plainBytes);
                        var extension = Path.GetExtension(Data);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(Data).ToString(), Path.GetFileNameWithoutExtension(Data) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                    break;
                case Format.TextString:
                    encrypted = SelectedCipherApi.EncryptText(SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, iv, Data);
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
            var iv = ByteConvert.HexStringToBytes(IV);

            switch (DataFormatSelected)
            {
                case Format.File:
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);
                    var decryptedBytes = SelectedCipherApi.DecryptToBytes(SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, iv, encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    DecryptedText = SelectedCipherApi.DecryptToText(SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, iv, encrypted);
                    break;
            }
        }

        #endregion

        #region MyRegion

        private void InitializeCommands()
        {
            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogAsync);
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
        }

        #endregion
    }
}
