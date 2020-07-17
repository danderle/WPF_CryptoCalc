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
    /// The view model for the Asymmetric cipher page
    /// </summary>
    public class AsymmetricViewModel : BaseViewModel
    {
        #region Private fields

        public string pKeyPath = @"C:\\Users\\beach\\Desktop\\Encryption tests";

        #endregion

        #region Public Properties

        public bool UsesEcCurves { get; set; }

        /// <summary>
        /// The data to be hashed <see cref="Format"/> for hash data format options
        /// </summary>
        public string Data { get; set; } = string.Empty;

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
        public string KeyName { get; set; }

        /// <summary>
        /// The path to the private key file
        /// </summary>
        public string PrivateKeyPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PublicKeyPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OtherPartyPublicKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DerivedKey { get; set; }

        /// <summary>
        /// The currently selected key size index
        /// </summary>
        public int KeySizeIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int EcCurveIndex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ProviderIndex { get; set; }

        /// <summary>
        /// Currently selected algorithim
        /// </summary>
        public int SelectedAlgorithimIndex { get; set; } = 0;

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; }

        /// <summary>
        /// The Public key operation to do
        /// </summary>
        public AsymmetricOperation SelectedOperation { get; set; }

        /// <summary>
        /// The selectred crypto library
        /// </summary>
        public CryptographyApi Api { get; set; }

        /// <summary>
        /// The list off all symmetric algorithims
        /// </summary>
        public List<string> Algorithims { get; set; } = new List<string>();
        
        /// <summary>
        /// Holds the data format options
        /// </summary>
        public List<string> DataFormatOptions { get; set; } = new List<string>();

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<string> Providers { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<string> EcCurves { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The list of all the different key size options
        /// </summary>
        public ObservableCollection<int> KeySizes { get; set; } = new ObservableCollection<int>();

        /// <summary>
        /// The selected crypto cipher algorithim
        /// </summary>
        public IAsymmetricCipher SelectedCipher { get; set; }

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
        public ICommand ChangedOperationCommand { get; set; }
        public ICommand ChangedProviderCommand { get; set; }
        public ICommand CreateKeyPairCommand { get; set; }
        public ICommand SaveKeyPairCommand { get; set; }
        public ICommand LoadPrivateKeyCommand { get; set; }
        public ICommand DeleteKeyPairCommand { get; set; }
        public ICommand SignCommand { get; set; }
        public ICommand VerifyCommand { get; set; }
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
            
            Api = api;
            SelectedOperation = operation;
            switch (Api)
            {
                case CryptographyApi.MSDN:
                    Algorithims = IAsymmetricCipher.GetMsdnAlgorthims(SelectedOperation);
                    break;
                case CryptographyApi.BouncyCastle:
                    Algorithims = IAsymmetricCipher.GetBouncyAlgorthims(SelectedOperation);
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            ChangedAlgorithim();

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
            var derivedKey = ((IAsymmetricKeyExchange)SelectedCipher).DeriveKey(privateKey, KeySizes[KeySizeIndex], otherPartyPublicKey);
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
            switch(DataFormatSelected)
            {
                case Format.File:
                    data = File.ReadAllBytes(Data);
                    break;
                case Format.TextString:
                    data = ByteConvert.StringToAsciiBytes(Data);
                    break;
            }
            SignatureVerified = ((IAsymmetricSignature)SelectedCipher).Verify(signature, pubKey, data);
        }

        /// <summary>
        /// The command method to sign some data
        /// </summary>
        private void Sign()
        {
            var privKey = File.ReadAllBytes(PrivateKeyPath);
            byte[] data = null;
            switch (DataFormatSelected)
            {
                case Format.File:
                    data = File.ReadAllBytes(Data);
                    break;
                case Format.TextString:
                    data = ByteConvert.StringToAsciiBytes(Data);
                    break;
            }
            var signature = ((IAsymmetricSignature)SelectedCipher).Sign(privKey, data);
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
            }
            if (File.Exists(PublicKeyPath))
            {
                File.Delete(PublicKeyPath);
            }
        }

        /// <summary>
        /// The command method to save a key pair
        /// </summary>
        private void SaveKeyPair()
        {
            File.WriteAllBytes(PrivateKeyPath, SelectedCipher.GetPrivateKey());
            File.WriteAllBytes(PublicKeyPath, SelectedCipher.GetPublicKey());
        }

        /// <summary>
        /// The command method to create a key pair
        /// </summary>
        private void CreateKeyPair()
        {
            PrivateKeyPath = pKeyPath + "\\" + KeyName + "_PrivateKey.pkcs1";
            PublicKeyPath = pKeyPath + "\\" + KeyName + "_PublicKey.pkcs1";
            
            if(SelectedCipher.UsesEcCurves)
            {
                ((IECAlgorithims)SelectedCipher).CreateKeyPair(EcCurves[EcCurveIndex]);
            }
            else
            {
                ((INonECAlgorithims)SelectedCipher).CreateKeyPair(KeySizes[KeySizeIndex]);
            }
        }

        private void ChangedProvider()
        {
            var provider = (EcCurveProvider)Enum.Parse(typeof(EcCurveProvider), Providers[ProviderIndex]);
            EcCurves = ((IECAlgorithims)SelectedCipher).GetEcCurves(provider);
            EcCurveIndex = 0;
        }

        /// <summary>
        /// The command method when a different algrithim is selected
        /// </summary>
        private void ChangedAlgorithim()
        {
            switch(Api)
            {
                case CryptographyApi.MSDN:
                     SelectedCipher = IAsymmetricCipher.GetMsdnCipher(Algorithims[SelectedAlgorithimIndex]);
                    break;
                case CryptographyApi.BouncyCastle:
                    SelectedCipher = IAsymmetricCipher.GetBouncyCipher(Algorithims[SelectedAlgorithimIndex]);
                    break;
            }
            UsesEcCurves = SelectedCipher.UsesEcCurves;
            if (UsesEcCurves)
            {
                Providers = ((IECAlgorithims)SelectedCipher).GetEcProviders();
                var provider = (EcCurveProvider)Enum.Parse(typeof(EcCurveProvider), Providers[ProviderIndex]);
                EcCurves = ((IECAlgorithims)SelectedCipher).GetEcCurves(provider);
                EcCurveIndex = 0;
            }
            else
            {
                KeySizes = ((INonECAlgorithims)SelectedCipher).GetKeySizes();
                KeySizeIndex = 0;
            }
        }

        /// <summary>
        /// The command method to decrypt the given data
        /// </summary>
        private void Decrypt()
        {
            byte[] encrypted;
            
            var privateKey = File.ReadAllBytes(PrivateKeyPath);
            switch (DataFormatSelected)
            {
                case Format.File:
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);
                    var decryptedBytes = ((IAsymmetricEncryption)SelectedCipher).DecryptToBytes(privateKey, encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    DecryptedText = ((IAsymmetricEncryption)SelectedCipher).DecryptToText(privateKey, encrypted);
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
            switch (DataFormatSelected)
            {
                case Format.File:
                    if (File.Exists(Data))
                    {
                        var plainBytes = File.ReadAllBytes(Data);
                        var encryptedBytes = ((IAsymmetricEncryption)SelectedCipher).EncryptBytes(pubKey, plainBytes);
                        var extension = Path.GetExtension(Data);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(Data).ToString(), Path.GetFileNameWithoutExtension(Data) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                break;
                case Format.TextString:
                    encrypted = ((IAsymmetricEncryption)SelectedCipher).EncryptText(pubKey, Data);
                    EncryptedText = ByteConvert.BytesToHexString(encrypted);
                    break;
            }
        }

        #endregion

        #region Private Methods

        private void InitializeLists()
        {
            //Set the data format options
            DataFormatOptions.Add(Format.File.ToString());
            DataFormatOptions.Add(Format.TextString.ToString());
        }

        private void InitializeCommands()
        {
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            CreateKeyPairCommand = new RelayCommand(CreateKeyPair);
            SaveKeyPairCommand = new RelayCommand(SaveKeyPair);
            DeleteKeyPairCommand = new RelayCommand(DeleteKeyPair);
            SignCommand = new RelayCommand(Sign);
            VerifyCommand = new RelayCommand(Verify);
            DeriveKeyCommand = new RelayCommand(DeriveKey);
            LoadPrivateKeyCommand = new RelayCommand(LoadPrivateKey);
            ChangedProviderCommand = new RelayCommand(ChangedProvider);
        }

        

        #endregion
    }
}
