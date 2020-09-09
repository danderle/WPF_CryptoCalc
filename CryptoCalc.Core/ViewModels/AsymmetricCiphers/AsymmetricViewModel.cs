using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the Asymmetric cipher page
    /// </summary>
    public class AsymmetricViewModel : BaseViewModel
    {
        #region Private fields

        public string pKeyPath = @"C:\\Users\\beach\\Desktop\\Encryption tests";

        #endregion

        #region Public Properties

        public bool DataCorrect { get; set; }

        public bool ReadyForEncryption => DataCorrect;

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
        /// The path to the private key file
        /// </summary>
        public string PrivateKeyPath { get; set; } = string.Empty;

        /// <summary>
        /// The file path to the public key
        /// </summary>
        public string PublicKeyPath { get; set; } = string.Empty;

        /// <summary>
        /// The file path to the private key
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// The other parties public key used for key exchange
        /// </summary>
        public string OtherPartyPublicKey { get; set; } = string.Empty;

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
        /// The command to drop files
        /// </summary>
        public ICommand DropCommand { get; set; }

        /// <summary>
        /// The command to change operation
        /// </summary>
        public ICommand ChangedOperationCommand { get; set; }

        /// <summary>
        /// The command to save a key pair
        /// </summary>
        public ICommand SaveKeyPairCommand { get; set; }

        /// <summary>
        /// The command to load a key pair
        /// </summary>
        public ICommand LoadPrivateKeyCommand { get; set; }

        /// <summary>
        /// The command to delete a key pair
        /// </summary>
        public ICommand DeleteKeyPairCommand { get; set; }

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

            //Initialize lists
            InitializeLists();
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

            KeyPairSetup = new KeyPairSetupViewModel(api, operation);

            //Initialize lists
            InitializeLists();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to load a private key from file if it existss
        /// </summary>
        private void LoadPrivateKey()
        {
            if (File.Exists(PrivateKeyPath))
            {
                var privateKey = File.ReadAllBytes(PrivateKeyPath);
                PrivateKey = ByteConvert.BytesToHexString(privateKey);
            }
        }

        /// <summary>
        /// The command method to derived a shared secret key from the other party public key and our own keys
        /// </summary>
        private void DeriveKey()
        {
            var otherPartyPublicKey = ByteConvert.HexStringToBytes(OtherPartyPublicKey.Replace(" ", string.Empty));
            var privateKey = File.ReadAllBytes(PrivateKeyPath);
            var derivedKey = ((IAsymmetricKeyExchange)KeyPairSetup.SelectedCipher).DeriveKey(privateKey, KeyPairSetup.KeySizes[KeyPairSetup.KeySizeIndex], otherPartyPublicKey);
            DerivedKey = ByteConvert.BytesToHexString(derivedKey);
        }

        /// <summary>
        /// The command method to verify a signature
        /// </summary>
        private void Verify()
        {
            var pubKey = File.ReadAllBytes(PublicKeyPath);
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
            var privKey = File.ReadAllBytes(PrivateKeyPath);
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
        /// the command method to delete a key pair
        /// </summary>
        private void DeleteKeyPair()
        {
            if (File.Exists(PrivateKeyPath))
            {
                File.Delete(PrivateKeyPath);
                PrivateKeyPath = string.Empty;
            }
            if (File.Exists(PublicKeyPath))
            {
                File.Delete(PublicKeyPath);
                PublicKeyPath = string.Empty;
            }
        }

        /// <summary>
        /// The command method to save a key pair
        /// </summary>
        private void SaveKeyPair()
        {
            if(File.Exists(PrivateKeyPath) && File.Exists(PublicKeyPath))
            {
                File.WriteAllBytes(PrivateKeyPath, KeyPairSetup.SelectedCipher.GetPrivateKey());
                File.WriteAllBytes(PublicKeyPath, KeyPairSetup.SelectedCipher.GetPublicKey());
            }
        }

        
        /// <summary>
        /// The command method to decrypt the given data
        /// </summary>
        private void Decrypt()
        {
            byte[] encrypted;
            
            var privateKey = File.ReadAllBytes(PrivateKeyPath);
            switch (DataInput.DataFormatSelected)
            {
                case Format.File:
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);
                    var decryptedBytes = ((IAsymmetricEncryption)KeyPairSetup.SelectedCipher).DecryptToBytes(privateKey, encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    DecryptedText = ((IAsymmetricEncryption)KeyPairSetup.SelectedCipher).DecryptToText(privateKey, encrypted);
                    break;
            }
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            byte[] encrypted;
            var pubKey = File.ReadAllBytes(PublicKeyPath);
            switch (DataInput.DataFormatSelected)
            {
                case Format.File:
                    if (File.Exists(DataInput.Data))
                    {
                        var plainBytes = File.ReadAllBytes(DataInput.Data);
                        var encryptedBytes = ((IAsymmetricEncryption)KeyPairSetup.SelectedCipher).EncryptBytes(pubKey, plainBytes);
                        var extension = Path.GetExtension(DataInput.Data);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(DataInput.Data).ToString(), Path.GetFileNameWithoutExtension(DataInput.Data) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                break;
                case Format.TextString:
                    encrypted = ((IAsymmetricEncryption)KeyPairSetup.SelectedCipher).EncryptText(pubKey, DataInput.Data);
                    EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize any lists
        /// </summary>
        private void InitializeLists()
        {
        }

        /// <summary>
        /// Initialize any commands
        /// </summary>
        private void InitializeCommands()
        {
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            SaveKeyPairCommand = new RelayCommand(SaveKeyPair);
            DeleteKeyPairCommand = new RelayCommand(DeleteKeyPair);
            SignCommand = new RelayCommand(Sign);
            VerifyCommand = new RelayCommand(Verify);
            DeriveKeyCommand = new RelayCommand(DeriveKey);
            LoadPrivateKeyCommand = new RelayCommand(LoadPrivateKey);
        }

        #endregion
    }
}
