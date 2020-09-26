using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the symmetric cipher page 
    /// </summary>
    public class SymmetricViewModel : BaseViewModel
    {
        #region Private Fields

        private bool secretKeyLengthIsAcceptable => (SecretKey.Length % 2 == 0) && (SecretKey.Length / 2 * 8 == SelectedKeySize);
        private bool secretKeyIsOnlyHex => ByteConvert.OnlyHexInString(SecretKey);

        private bool ivLengthIsAcceptable => (IV.Length % 2 == 0) && (IV.Length / 2 * 8 == IvSize);
        private bool ivIsOnlyHex => ByteConvert.OnlyHexInString(IV);

        #endregion

        #region Public Properties

        /// <summary>
        /// Flag to let us know if the symmetric cipher has an IV
        /// </summary>
        public bool HasIv => IvSize > 0;

        /// <summary>
        /// Flag to let us know if the secret key is in an acceptable format
        /// </summary>
        public bool SecretKeyAcceptable => secretKeyLengthIsAcceptable && secretKeyIsOnlyHex;

        /// <summary>
        /// Flag to let us know if the iv is in an acceptable format
        /// </summary>
        public bool IvAcceptable => ivLengthIsAcceptable && ivIsOnlyHex;

        /// <summary>
        /// Flag for letting us know if the Data is correctly entered and ready for en-/decrypting
        /// </summary>
        public bool DataCorrect { get; set; }

        /// <summary>
        /// Flag letting us know if we are ready for encryption
        /// </summary>
        public bool ReadyForEncryption
        {
            get
            {
                bool ready = DataCorrect && SecretKeyAcceptable;
                if(ready && HasIv)
                {
                    return IvAcceptable;
                }
                else
                {
                    return ready;
                }
            }
        }

        /// <summary>
        /// Flag letting us know if we are ready for decryption
        /// </summary>
        public bool ReadyForDecryption
        {
            get
            {
                string encrypted = string.Empty;
                switch (CurrentFormat)
                {
                    case Format.File:
                        //Get the encrypted file to bytes
                        var bytes = ByteConvert.FileToBytes(EncryptedFilePath);
                        if (bytes != null)
                        {
                            encrypted = ByteConvert.BytesToHexString(bytes);
                        }
                        break;
                    case Format.HexString:
                        encrypted = EncryptedHex;
                        break;
                    case Format.TextString:
                        encrypted = EncryptedText;
                        break;
                }
                //true if the encrypted format matches the criteria
                if(encrypted.Length != 0)
                {
                    bool ready = ByteConvert.OnlyHexInString(encrypted) && SecretKeyAcceptable;
                    if(ready && HasIv)
                    {
                         return IvAcceptable;
                    }
                    else
                    {
                        return ready;
                    }
                }
                return false;
            }
        }
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
        public string EncryptedFilePath { get; set; } = string.Empty;

        /// <summary>
        /// The encrypted text
        /// </summary>
        public string EncryptedText { get; set; } = string.Empty;

        /// <summary>
        /// The encrypted hex
        /// </summary>
        public string EncryptedHex { get; set; } = string.Empty;

        /// <summary>
        /// The decrypted file path
        /// </summary>
        public string DecryptedFilePath { get; set; } = string.Empty;

        /// <summary>
        /// The decrypted text
        /// </summary>
        public string DecryptedText { get; set; } = string.Empty;

        /// <summary>
        /// The decrypted hex
        /// </summary>
        public string DecryptedHex { get; set; } = string.Empty;

        /// <summary>
        /// The iv size in bits
        /// </summary>
        public int IvSize { get; set; }

        /// <summary>
        /// Currently selected algorithim
        /// </summary>
        public int SelectedAlgorithim { get; set; } = 0;

        /// <summary>
        /// Get the currently selected key size
        /// </summary>
        public int SelectedKeySize { get; set; }

        /// <summary>
        /// The currently selected <see cref="Format"/>
        /// </summary>
        public Format CurrentFormat { get; set; }

        /// <summary>
        /// The Cryptography api to use
        /// </summary>
        public CryptographyApi Api { get; set; }

        /// <summary>
        /// The currently selected cipher api
        /// </summary>
        public ISymmetricCipher SelectedCipherApi { get; set; }

        /// <summary>
        /// The list off all symmetric algorithims
        /// </summary>
        public List<string> Algorithims { get; set; } = new List<string>();

        /// <summary>
        /// The list of all the different key size options
        /// </summary>
        public ObservableCollection<int> KeySizes { get; set; } = new ObservableCollection<int>();

        /// <summary>
        /// The View model for the data input control 
        /// </summary>
        public DataInputViewModel DataInput { get; set; } = new DataInputViewModel();
       

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

            //As default select the first item in the list
            SelectedKeySize = KeySizes[0];

            //Gets the iv size of the currently selected algorithim
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);

            ///Subscribe to the DataChanged event from the <see cref="DataInputViewModel"/>
            DataInput.DataChanged += DataChanged;
        }


        #endregion

        #region Command Methods

        /// <summary>
        /// The command methods to generate a random key
        /// </summary>
        private void GenerateKey()
        {
            //Gets a list of byte arrays containing the secret key and if available the iv
            var keyAndIv = SelectedCipherApi.GenerateKey(SelectedAlgorithim, SelectedKeySize);

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

            //Set default key size, first one in the list
            SelectedKeySize = KeySizes[0];

            //Gets the available iv size of the selected algorithim
            IvSize = SelectedCipherApi.GetIvSize(SelectedAlgorithim);
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            //Convert the string key and iv to bytes
            var secretKey = ByteConvert.HexStringToBytes(SecretKey);
            var iv = ByteConvert.HexStringToBytes(IV);

            //fields for saving the plain and encrypted byte arrays
            byte[] encrypted;
            byte[] plainBytes;
            
            //Get the data and encrypt it acording to the selected format
            switch (DataInput.DataFormatSelected)
            {
                //Encrypt a text string
                case Format.TextString:

                    //Encrypt with the selected cipher and return the encrypted byte array
                    encrypted = SelectedCipherApi.EncryptText(SelectedAlgorithim, SelectedKeySize, secretKey, iv, DataInput.Data);
                    
                    //Converts the byte array to a hex string
                    EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    break;
                //Encrypt a hex string
                case Format.HexString:

                    //Convert the plain hex string to byte array
                    plainBytes = ByteConvert.HexStringToBytes(DataInput.Data);

                    //Encrypt with the selected cipher and return the encrypted byte array
                    encrypted = SelectedCipherApi.EncryptBytes(SelectedAlgorithim, SelectedKeySize, secretKey, iv, plainBytes);
                    
                    //Converts the byte array to a hex string
                    EncryptedHex = ByteConvert.BytesToHexString(encrypted);
                    break;
                //Encrypt a file
                case Format.File:

                    //Gets the file as a byte array
                    plainBytes = File.ReadAllBytes(DataInput.Data);

                    //Encrypt with the selected cipher and return the encrypted byte array
                    encrypted = SelectedCipherApi.EncryptBytes(SelectedAlgorithim, SelectedKeySize, secretKey, iv, plainBytes);
                    
                    //Gets the file extension of the plain original file
                    var extension = Path.GetExtension(DataInput.Data);
                    
                    //Adds the "Encrypted" text to the encrypted file name
                    EncryptedFilePath = Path.Combine(Directory.GetParent(DataInput.Data).ToString(), Path.GetFileNameWithoutExtension(DataInput.Data) + ".Encrypted" + extension);
                    
                    //Writes all the bytes to the encrypted file
                    File.WriteAllBytes(EncryptedFilePath, encrypted);
                    break;
            }
        }

        /// <summary>
        /// The command method to decrypt the given data
        /// TODO wrap decrypt in try catch with pop up window if error occurs during decryption
        /// </summary>
        private void Decrypt()
        {
            //field for saving the encrypted and decrypted byte arrays
            byte[] encrypted;
            byte[] decryptedBytes;

            //Convert the secret key and iv to byte arrays
            var secretKey = ByteConvert.HexStringToBytes(SecretKey);
            var iv = ByteConvert.HexStringToBytes(IV);

            //Decrypt the data according to the selected file format
            switch (DataInput.DataFormatSelected)
            {
                //Decrypt to a regular string
                case Format.TextString:

                    //Convert the text string to a byte array
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    
                    //Decrypt the byte array to a text string
                    DecryptedText = SelectedCipherApi.DecryptToText(SelectedAlgorithim, SelectedKeySize, secretKey, iv, encrypted);
                    break;
                case Format.HexString:

                    //Convert the hex string to a byte array
                    encrypted = ByteConvert.HexStringToBytes(EncryptedHex);
                    
                    //Decrypt the byte array to a decrypted byte array
                    decryptedBytes = SelectedCipherApi.DecryptToBytes(SelectedAlgorithim, SelectedKeySize, secretKey, iv, encrypted);

                    //Convert the decrypted byte array to a hex string
                    DecryptedHex = ByteConvert.BytesToHexString(decryptedBytes);
                    break;
                case Format.File:

                    //Get the encrypted file as a byte array
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);

                    //Decrypt the byte array to a decrypted byte array
                    decryptedBytes = SelectedCipherApi.DecryptToBytes(SelectedAlgorithim, SelectedKeySize, secretKey, iv, encrypted);
                    
                    //Create a new file name with the encrypted file path and the "Decrypted" text
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileName(EncryptedFilePath).Replace("Encrypted","Decrypted"));
                    
                    //Write all byte to the decrypted file
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
            }
        }

        #endregion

        #region Event handler subscriptions

        /// <summary>
        /// Event subscription called when the input data changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void DataChanged(object obj, EventArgs args)
        {
            DataCorrect = DataInput.DataIsCorrectlyFormatted;
            if(CurrentFormat != DataInput.DataFormatSelected)
            {
                CurrentFormat = DataInput.DataFormatSelected;
                switch(CurrentFormat)
                {
                    case Format.File:
                        EncryptedHex = string.Empty;
                        DecryptedHex = string.Empty;
                        break;
                    case Format.HexString:
                        EncryptedText = string.Empty;
                        DecryptedText = string.Empty;
                        break;
                    case Format.TextString:
                        EncryptedFilePath = string.Empty;
                        DecryptedFilePath = string.Empty;
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes all the commands
        /// </summary>
        private void InitializeCommands()
        {
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            GenerateKeyCommand = new RelayCommand(GenerateKey);
        }

        #endregion
    }
}
