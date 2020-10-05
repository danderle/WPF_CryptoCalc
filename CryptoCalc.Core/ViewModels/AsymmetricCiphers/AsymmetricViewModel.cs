using System.IO;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the Asymmetric cipher page
    /// </summary>
    public class AsymmetricViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Flag to let us know if the private key is loaded
        /// </summary>
        public bool PrivateKeyLoaded { get; set; }
        
        /// <summary>
        /// Flag to let us know if the public key is loaded
        /// </summary>
        public bool PublicKeyLoaded { get; set; }

        /// <summary>
        /// Flag to let us know if the data format is correct
        /// </summary>
        public bool DataFormatCorrect { get; set; }

        /// <summary>
        /// A flag to let us know if we are ready for encryption
        /// </summary>
        public bool ReadyForEncryption => DataFormatCorrect && PublicKeyLoaded;

        /// <summary>
        /// A flag to let us know if we are ready for decryption
        /// </summary>
        public bool ReadyForDecryption => PrivateKeyLoaded && (EncryptedText.HasOnlyHex() || File.Exists(EncryptedFilePath));

        /// <summary>
        /// Flag to let us know if we can sign data
        /// </summary>
        public bool ReadyToSign => ReadyForEncryption;

        /// <summary>
        /// Flag to let us know if we can verify a signature
        /// </summary>
        public bool ReadyToVerify => PrivateKeyLoaded && OriginalSignature.HasOnlyHex() && DataFormatCorrect;

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
        /// The name to save the public and private key pair to
        /// </summary>
        public string KeyName { get; set; } = string.Empty;

        /// <summary>
        /// The derived key when using key exchange
        /// </summary>
        public string DerivedKey { get; set; } = string.Empty;

        /// <summary>
        /// The View model for the data input control 
        /// </summary>
        public DataInputViewModel DataInput { get; set; } = new DataInputViewModel();

        /// <summary>
        /// The view model for the key pair setup control
        /// </summary>
        public KeyPairSetupViewModel KeyPairSetup { get; set; } = new KeyPairSetupViewModel();

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
        /// The command to create a signature
        /// </summary>
        public ICommand SignCommand { get; set; }

        /// <summary>
        /// The command to verify a signature
        /// </summary>
        public ICommand VerifyCommand { get; set; }

        /// <summary>
        /// The command to derive a shared key during a key exchange
        /// </summary>
        public ICommand DeriveKeyCommand { get; set; }

        
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AsymmetricViewModel()
        {
            //Initialize the commands
            InitializeCommands();

            //Initialize properties
            InitializeProperties(CryptographyApi.MSDN, AsymmetricOperation.Encryption);
        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="api">the selected crypto library</param>
        /// <param name="operation">The selected crypto operation</param>
        public AsymmetricViewModel(CryptographyApi api, AsymmetricOperation operation)
        {
            //Initialize the commands
            InitializeCommands();

            //Initialize properties
            InitializeProperties(api, operation);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to derive a shared secret key from the other party public key and our own keys
        /// </summary>
        private void DeriveKey()
        {
            var derivedKey = KeyPairSetup.DeriveKey();
            DerivedKey = ByteConvert.BytesToHexString(derivedKey);
        }

        /// <summary>
        /// The command method to verify a signature
        /// </summary>
        private void Verify()
        {
            var pubKey = File.ReadAllBytes(KeyPairSetup.PublicKeyFilePath);
            var signature = ByteConvert.HexStringToBytes(OriginalSignature);
            byte[] data = null;
            switch(DataInput.DataFormatSelected)
            {
                case Format.File:
                    data = File.ReadAllBytes(DataInput.Data);
                    break;
                case Format.TextString:
                    data = ByteConvert.StringToAsciiBytes(DataInput.Data);
                    break;
            }
            SignatureVerified = ((IAsymmetricSignature)KeyPairSetup.SelectedCipher).Verify(signature, pubKey, data);
        }

        /// <summary>
        /// The command method to sign some data
        /// </summary>
        private void Sign()
        {
            var privKey = File.ReadAllBytes(KeyPairSetup.PrivateKeyFilePath);
            byte[] data = null;
            switch (DataInput.DataFormatSelected)
            {
                case Format.File:
                    data = File.ReadAllBytes(DataInput.Data);
                    break;
                case Format.TextString:
                    data = ByteConvert.StringToAsciiBytes(DataInput.Data);
                    break;
            }
            var signature = ((IAsymmetricSignature)KeyPairSetup.SelectedCipher).Sign(privKey, data);
            OriginalSignature = ByteConvert.BytesToHexString(signature);
        }

        /// <summary>
        /// The command method to decrypt the given data
        /// </summary>
        private void Decrypt()
        {
            byte[] encrypted;
            byte[] decrypted;

            //Decryption sequence differs according to the selected data format
            switch (DataInput.DataFormatSelected)
            {
                //Decrypt file
                case Format.File:
                    //Get the encrypted file bytes
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);

                    //Decrypt
                    decrypted = KeyPairSetup.Decrypt(encrypted);

                    //Create an encrypted file path
                    DecryptedFilePath = DataInput.GetDecryptedFilePath(EncryptedFilePath);

                    //Write decrypted bytes to file
                    File.WriteAllBytes(DecryptedFilePath, decrypted);
                    break;

                //Decrypt a text
                case Format.TextString:
                    //Convert the encrypted hex string to bytes
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);

                    //Decrypt as text
                    DecryptedText = KeyPairSetup.DecryptToText(encrypted);
                    break;

                //Decrypt a hex value
                case Format.HexString:
                    //Convert the encrypted hex string to bytes
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);

                    //Decrypt
                    decrypted = KeyPairSetup.Decrypt(encrypted);

                    //Convert decrypted bytes to hex string
                    DecryptedText = ByteConvert.BytesToHexString(decrypted);
                    break;
            }
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            byte[] encrypted;
            byte[] plainBytes;

            //Encryption sequence differs according to the selected data format
            switch (DataInput.DataFormatSelected)
            {
                //Encrypt file
                case Format.File:
                    //Get plain file bytes
                    plainBytes = DataInput.GetBytesFromFile();

                    //Encrypt
                    encrypted = KeyPairSetup.Encrypt(plainBytes);

                    //Verify that the encryption is successfull
                    if(encrypted != null)
                    {
                        //Create encrypted file path
                        EncryptedFilePath = DataInput.GetEncryptedFilePath();

                        //Write the encrypted bytes to the new file path
                        File.WriteAllBytes(EncryptedFilePath, encrypted);
                    }
                break;

                //Encrypt text
                case Format.TextString:
                    //Encrypt
                    encrypted = KeyPairSetup.Encrypt(DataInput.Data);

                    //Verify that the encryption is successfull
                    if (encrypted != null)
                    {
                        //Comvert the encrypted bytes to hex string
                        EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    }
                    break;

                //Encrypt hex string
                case Format.HexString:
                    //Convert hex string to plain bytes
                    plainBytes = ByteConvert.HexStringToBytes(DataInput.Data);

                    //Encrypt
                    encrypted = KeyPairSetup.Encrypt(plainBytes);

                    //Verify that the encryption is successfull
                    if (encrypted != null)
                    {
                        //Convert encrypted bytes to hex string
                        EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    }
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize any properties
        /// </summary>
        /// <param name="api"></param>
        /// <param name="operation"></param>
        private void InitializeProperties(CryptographyApi api, AsymmetricOperation operation)
        {
            DataInput.DataChanged += DataInput_DataChanged;
            KeyPairSetup = new KeyPairSetupViewModel(api, operation);
            KeyPairSetup.KeysLoaded += KeyPairSetup_KeysLoaded;
        }

        /// <summary>
        /// Hook into the KeysLoaded event to let us know if a private/public key 
        /// has been loaded
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void KeyPairSetup_KeysLoaded(object obj, System.EventArgs args)
        {
            PrivateKeyLoaded = KeyPairSetup.PrivateKeyLoaded;
            PublicKeyLoaded = KeyPairSetup.PublicKeyLoaded;
        }

        /// <summary>
        /// Function which is hooked into the DataChangedEvent, lets us know if the Data 
        /// is correctly formatted and ready for processing
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void DataInput_DataChanged(object obj, System.EventArgs args)
        {
            DataFormatCorrect = DataInput.DataIsCorrectlyFormatted;
            EncryptedFilePath = string.Empty;
            EncryptedText = string.Empty;
        }

        /// <summary>
        /// Initialize any commands
        /// </summary>
        private void InitializeCommands()
        {
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            SignCommand = new RelayCommand(Sign);
            VerifyCommand = new RelayCommand(Verify);
            DeriveKeyCommand = new RelayCommand(DeriveKey);
        }

        #endregion
    }
}
