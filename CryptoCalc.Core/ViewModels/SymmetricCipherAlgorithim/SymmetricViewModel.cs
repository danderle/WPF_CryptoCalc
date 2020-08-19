using CryptoCalc.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        /// Flag to let us know if the symmetric cipher has an IV
        /// </summary>
        public bool HasIv => IvSize > 0;


        public bool SecretKeyAcceptable { get; set; }

        public bool IvAcceptable { get; set; }

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
        /// The Cryptography api to use
        /// </summary>
        public CryptographyApi Api { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DataInputViewModel DataSetup { get; set; } = new DataInputViewModel();

        /// <summary>
        /// The list off all symmetric algorithims
        /// </summary>
        public List<string> Algorithims { get; set; } = new List<string>();

        /// <summary>
        /// The list of all the different key size options
        /// </summary>
        public ObservableCollection<int> KeySizes { get; set; } = new ObservableCollection<int>();

        /// <summary>
        /// The currently selected cipher api
        /// </summary>
        public ISymmetricCipher SelectedCipherApi { get; set; }

        #endregion

        #region Commands

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

        /// <summary>
        /// The command to generate a symmetric key
        /// </summary>
        public ICommand GenerateKeyCommand { get; set; }

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

            //according to the selected cipher api create a cipher object
            //TODO create a factory
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

            //Gets all the available algorithims from cipher api
            Algorithims = SelectedCipherApi.GetAlgorthims();

            //Gets all the available key sizes for the currently selected algorithim
            KeySizes = SelectedCipherApi.GetKeySizes(SelectedAlgorithim);

            //Gets the iv size of the currently selected algorithim
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);
        }


        #endregion

        #region Command Methods

        /// <summary>
        /// The command methods to generate a random key
        /// </summary>
        private void GenerateKey()
        {
            //Gets a list of byte arrays containing the secret key and if available the iv
            var keyAndIv = SelectedCipherApi.GenerateKey(SelectedAlgorithim, KeySizes[KeySizeIndex]);

            //Convert the byte array to a string
            SecretKey = ByteConvert.BytesToHexString(keyAndIv[0]);

            //true if an iv byte array exists
            if(keyAndIv.Count > 1)
            {
                //Convert the byte array to a string
                IV = ByteConvert.BytesToHexString(keyAndIv[1]);
            }
        }

        

        /// <summary>
        /// The command method when a different algrithim is selected
        /// </summary>
        private void ChangedAlgorithim()
        {
            //Gets the available key size of the selected algorithim
            KeySizes = SelectedCipherApi.GetKeySizes(SelectedAlgorithim);

            //GEts the available iv size of the selected algorithim
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);
            
            //sets the default key size
            KeySizeIndex = 0;
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            //Convert the string key and iv to bytes
            var secretKey = ByteConvert.HexStringToBytes(SecretKey);
            var iv = ByteConvert.HexStringToBytes(IV);

            byte[] encrypted;

            //Get the data
            switch (DataSetup.DataFormatSelected)
            {
                case Format.File:
                    if (File.Exists(DataSetup.Data))
                    {
                        var plainBytes = File.ReadAllBytes(DataSetup.Data);
                        var encryptedBytes = SelectedCipherApi.EncryptBytes(SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, iv, plainBytes);
                        var extension = Path.GetExtension(DataSetup.Data);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(DataSetup.Data).ToString(), Path.GetFileNameWithoutExtension(DataSetup.Data) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                    break;
                case Format.TextString:
                    encrypted = SelectedCipherApi.EncryptText(SelectedAlgorithim, KeySizes[KeySizeIndex], secretKey, iv, DataSetup.Data);
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

            switch (DataSetup.DataFormatSelected)
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

        #region Private Methods

        private void InitializeCommands()
        {
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            GenerateKeyCommand = new RelayCommand(GenerateKey);
        }

        /// <summary>
        /// Verifies if the correct format is implemented on given text
        /// </summary>
        /// <param name="format">The format used</param>
        /// <param name="text">The text entered by user</param>
        /// <returns>True if correct format</returns>
        private bool CheckIfCorrectlyFormatted(Format format, string text)
        {
            switch (format)
            {
                case Format.File:
                    return File.Exists(text);
                case Format.HexString:
                    return text.Length % 2 == 0 && ByteConvert.OnlyHexInString(text);
                default:
                    return text.Length > 0;
            }
        }
        #endregion
    }
}
