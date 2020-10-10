using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the key pair setup control
    /// </summary>
    public class KeyPairSetupViewModel : BaseViewModel
    {
        #region Private fields

        public string pKeyPath = @"C:\Users\beach\Desktop\Encryption tests";

        #endregion

        #region Public Properties

        /// <summary>
        /// Flag to know if the selected algorithim uses Ec curves for creation
        /// </summary>
        public bool UsesEcCurves { get; set; }

        /// <summary>
        /// Flag to know if the selected algorithim uses key size for creation
        /// </summary>
        public bool UsesKeySize { get; set; }

        /// <summary>
        /// Flag to let us know if the Directory for saving the keys in exist
        /// </summary>
        public bool KeyDirectoryPathExists => Directory.Exists(KeyDirectoryPath);

        /// <summary>
        /// Flag to let us know if the private key file exists
        /// </summary>
        public bool PrivateKeyFilePathExists => File.Exists(PrivateKeyFilePath);

        /// <summary>
        /// Flag to let us know if the public key path exists
        /// </summary>
        public bool PublicKeyFilePathExists => File.Exists(PublicKeyFilePath);

        /// <summary>
        /// Flag to let us know if the other persons public key file path exists
        /// </summary>
        public bool OtherPartyPublicKeyFilePathExists => File.Exists(OtherPartyPublicKeyFilePath);

        /// <summary>
        /// Flag to let us know if the Private key is loaded
        /// </summary>
        public bool PrivateKeyLoaded { get; set; }

        /// <summary>
        /// Flag to let us know if the public key is loaded
        /// </summary>
        public bool PublicKeyLoaded { get; set; }

        /// <summary>
        /// Flag to let us know if the other public key is loaded
        /// </summary>
        public bool OtherPublicKeyLoaded { get; set; }

        /// <summary>
        /// Flag to let us know if any key exist
        /// </summary>
        public bool KeysExist => PrivateKeyFilePathExists || PublicKeyFilePathExists;

        /// <summary>
        /// The currently selected key size index
        /// </summary>
        public int KeySizeIndex { get; set; }

        /// <summary>
        /// The ec curve index used from the selected list
        /// </summary>
        public int EcCurveIndex { get; set; }

        /// <summary>
        /// The currently selected key size
        /// </summary>
        public int KeySize => KeySizes[KeySizeIndex];

        /// <summary>
        /// The provider index used from the selected list
        /// </summary>
        public int ProviderIndex { get; set; }

        /// <summary>
        /// Currently selected algorithim
        /// </summary>
        public int SelectedAlgorithimIndex { get; set; } = 0;

        /// <summary>
        /// The dirtectory path to create a new key pair in
        /// </summary>
        public string KeyDirectoryPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>
        /// The path to the private key file
        /// </summary>
        public string PrivateKeyFilePath { get; set; } = string.Empty;

        /// <summary>
        /// The file path to the public key
        /// </summary>
        public string PublicKeyFilePath { get; set; } = string.Empty;

        /// <summary>
        /// The other parties public key used for key exchange
        /// </summary>
        public string OtherPartyPublicKeyFilePath { get; set; } = string.Empty;

        /// <summary>
        /// The Public key operation to do
        /// </summary>
        public AsymmetricOperation SelectedOperation { get; set; }

        /// <summary>
        /// The selected crypto cipher algorithim
        /// </summary>
        public IAsymmetricCipher SelectedCipher { get; set; }

        /// <summary>
        /// The selectred crypto library
        /// </summary>
        public CryptographyApi Api { get; set; }

        /// <summary>
        /// The private key
        /// </summary>
        public byte[] PrivateKey { get; set; }

        /// <summary>
        /// The public key
        /// </summary>
        public byte[] PublicKey { get; set; }

        /// <summary>
        /// The Other Party Public key
        /// </summary>
        public byte[] OtherPartyPublicKey { get; set; }

        /// <summary>
        /// A delegate event handler when a key is loaded
        /// </summary>
        /// <param name="obj">the object from which event is triggered</param>
        /// <param name="args">The event arguments</param>
        public delegate void KeysLoadedEventHandler(object obj, EventArgs args);

        /// <summary>
        /// The data changed event
        /// </summary>
        public event KeysLoadedEventHandler KeysLoaded;

        /// <summary>
        /// A delegate event handler function for any key pair changes, like algorihtim, key size or provider changes
        /// </summary>
        /// <param name="obj">the object from which event is triggered</param>
        /// <param name="args">The event arguments</param>
        public delegate void KeyPairChangedEventHandler(object obj, EventArgs args);

        /// <summary>
        /// The key size changed event
        /// </summary>
        public event KeyPairChangedEventHandler KeyPairChanged;

        /// <summary>
        /// The list off all symmetric algorithims
        /// </summary>
        public List<string> Algorithims { get; set; } = new List<string>();
        
        /// <summary>
        /// The list of available provider
        /// </summary>
        public ObservableCollection<string> Providers { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The list of available ec curves
        /// </summary>
        public ObservableCollection<string> EcCurves { get; set; } = new ObservableCollection<string>();

        /// <summary>
        /// The list of all the different key size options
        /// </summary>
        public ObservableCollection<int> KeySizes { get; set; } = new ObservableCollection<int>();

        #endregion

        #region Commands

        /// <summary>
        /// The command to execute when a cipher algorithim is changed
        /// </summary>
        public ICommand ChangedAlgorithimCommand { get; set; }

        /// <summary>
        /// The command to execute when the provider has changed
        /// </summary>
        public ICommand ChangedProviderCommand { get; set; }

        /// <summary>
        /// The command to execute when the key size has changed
        /// </summary>
        public ICommand ChangedKeySizeCommand { get; set; }

        /// <summary>
        /// The command to execute when the ec curve has changed
        /// </summary>
        public ICommand ChangedEcCurveCommand { get; set; }

        /// <summary>
        /// The command to create a key pair
        /// </summary>
        public ICommand CreateKeyPairCommand { get; set; }

        /// <summary>
        /// The command to load a key/s from given key paths
        /// </summary>
        public ICommand LoadKeyCommand { get; set; }

        /// <summary>
        /// The command to delete keys
        /// </summary>
        public ICommand DeleteKeyCommand { get; set; }

        /// <summary>
        /// The command to get the private key from file browser
        /// </summary>
        public ICommand GetPrivateKeyFilePathCommand { get; set; }

        /// <summary>
        /// The command to get the private key from file browser
        /// </summary>
        public ICommand GetPublicKeyFilePathCommand { get; set; }

        /// <summary>
        /// The command to get the other person public key from file browser
        /// </summary>
        public ICommand GetOtherPublicKeyFilePathCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyPairSetupViewModel()
        {
            //Initialize the commands
            InitializeCommands();

            //Initialize propertiies
            InitializeProperties(CryptographyApi.MSDN, AsymmetricOperation.Encryption);

            //Initialize lists must be after setting api and operation
            InitializeLists();
        }

        /// <summary>
        /// Overloaded Constructor
        /// </summary>
        /// <param name="api">the selected crypto library</param>
        /// <param name="operation">The selected crypto operation</param>
        public KeyPairSetupViewModel(CryptographyApi api, AsymmetricOperation operation)
        {
            //Initialize the commands
            InitializeCommands();

            //Initialize properties
            InitializeProperties(api, operation);

            //Initialize lists must be after setting api and operation
            InitializeLists();
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Gets the file path of the other person public key
        /// </summary>
        private void GetOtherPublicKeyFilePath()
        {
            //Opens a pop up window folder browser dialog
            Dialog.OpenFolderBrowserAsync();

            //Saves the selected path
            OtherPartyPublicKeyFilePath = Ioc.Application.FilePathFromDialogSelection;
        }

        /// <summary>
        /// Gets the file path of the private key
        /// </summary>
        private void GetPrivateKeyFilePath()
        {
            //Opens a pop up window folder browser dialog
            Dialog.OpenFolderBrowserAsync();

            //Saves the selected path
            PrivateKeyFilePath = Ioc.Application.FilePathFromDialogSelection;

            //Key still needs to be loaded
            PrivateKeyLoaded = false;
        }

        /// <summary>
        /// Gets the file path of the public key
        /// </summary>
        private void GetPublicKeyFilePath()
        {
            //Opens a pop up window folder browser dialog
            Dialog.OpenFolderBrowserAsync();

            //Saves the selected path
            PublicKeyFilePath = Ioc.Application.FilePathFromDialogSelection;
            
            //Key still needs to be loaded
            PublicKeyLoaded = false;
        }

        /// <summary>
        /// The command method to change a provider
        /// </summary>
        private void ChangedProvider()
        {
            if(SelectedCipher is IECAlgorithims ecAlgorithim)
            {
                var provider = (EcCurveProvider)Enum.Parse(typeof(EcCurveProvider), Providers[ProviderIndex]);
                EcCurves = ecAlgorithim.GetEcCurves(provider);
                EcCurveIndex = 0;
            }
        }

        /// <summary>
        /// The command method when a different algrithim is selected
        /// </summary>
        public void ChangedAlgorithim()
        {
            //Find out which api is beeing used
            switch (Api)
            {
                //MSDN API
                case CryptographyApi.MSDN:
                    //Get the selected cipher algorithim
                    SelectedCipher = IAsymmetricCipher.GetMsdnCipher(Algorithims[SelectedAlgorithimIndex]);
                    break;

                //Bouncy castle api
                case CryptographyApi.BouncyCastle:
                    //Get the selected cipher algorithim
                    SelectedCipher = IAsymmetricCipher.GetBouncyCipher(Algorithims[SelectedAlgorithimIndex]);
                    break;
            }

            //True if the cipher uses ec curves
            if (SelectedCipher is IECAlgorithims ecCipher)
            {
                UsesKeySize = false;
                UsesEcCurves = ecCipher.UsesCurves;
                if (UsesEcCurves)
                {
                    Providers = ecCipher.GetEcProviders();
                    var provider = (EcCurveProvider)Enum.Parse(typeof(EcCurveProvider), Providers[ProviderIndex]);
                    EcCurves = ecCipher.GetEcCurves(provider);
                    EcCurveIndex = 0;
                }
            }

            //true if the cipher algorithims doesnt use ec curves
            else if (SelectedCipher is INonECAlgorithims nonEcCipher)
            {
                UsesEcCurves = false;
                UsesKeySize = nonEcCipher.UsesKeySize;
                KeySizes = nonEcCipher.GetKeySizes();
                KeySizeIndex = 0;
            }

            PrivateKeyFilePath = string.Empty;
            PublicKeyFilePath = string.Empty;
            OtherPartyPublicKeyFilePath = string.Empty;

            OnKeyPairChanged();
        }

        /// <summary>
        /// Command method to execute when the key size is changed
        /// </summary>
        private void ChangedKeySize()
        {
            UnloadKeyPair();
            OnKeyPairChanged();
        }

        /// <summary>
        /// Command method to execute when the ec curve has changed
        /// </summary>
        private void ChangedEcCurve()
        {
            UnloadKeyPair();
            OnKeyPairChanged();
        }

        /// <summary>
        /// The command method to create a key pair
        /// </summary>
        private void CreateKeyPair()
        {
            //True if the cipher uses ec curves
            if (SelectedCipher is IECAlgorithims ecCipher)
            {
                if (EcCurves.Count != 0)
                {
                    ecCipher.CreateKeyPair(EcCurves[EcCurveIndex]);
                }
            }
            //true if the cipher algorithims doesnt use ec curves
            else if (SelectedCipher is INonECAlgorithims nonEcCipher)
            {
                if (KeySizes.Count != 0)
                {
                    nonEcCipher.CreateKeyPair(KeySizes[KeySizeIndex]);
                }
            }
            else
            {
                return;
            }

            //Create the file paths
            PrivateKeyFilePath = $"{KeyDirectoryPath}\\PrivateKey.bin";
            PublicKeyFilePath = $"{KeyDirectoryPath}\\PublicKey.bin";

            //Write keys to the files
            File.WriteAllBytes(PrivateKeyFilePath, SelectedCipher.GetPrivateKey());
            File.WriteAllBytes(PublicKeyFilePath, SelectedCipher.GetPublicKey());

            //Update the affected properties
            OnPropertyChanged(nameof(PrivateKeyFilePathExists));
            OnPropertyChanged(nameof(PublicKeyFilePathExists));
            OnPropertyChanged(nameof(KeysExist));

            //Load the keys ready to use
            LoadKey();
        }

        /// <summary>
        /// The command method to load a key/s
        /// </summary>
        private void LoadKey()
        {
            if (PrivateKeyFilePathExists)
            {
                PrivateKey = File.ReadAllBytes(PrivateKeyFilePath);
                PrivateKeyLoaded = true;
            }
            if (PublicKeyFilePathExists)
            {
                PublicKey = File.ReadAllBytes(PublicKeyFilePath);
                PublicKeyLoaded = true;
            }
            if (OtherPartyPublicKeyFilePathExists)
            {
                OtherPartyPublicKey = File.ReadAllBytes(PublicKeyFilePath);
                OtherPublicKeyLoaded = true;
            }
            OnKeyLoad();
        }

        /// <summary>
        /// The command method to delete a key pair
        /// </summary>
        private void DeleteKey()
        {
            if (PrivateKeyFilePathExists)
            {
                File.Delete(PrivateKeyFilePath);
                PrivateKeyFilePath = string.Empty;
                PrivateKeyLoaded = false;
            }
            if (PublicKeyFilePathExists)
            {
                File.Delete(PublicKeyFilePath);
                PublicKeyFilePath = string.Empty;
                PublicKeyLoaded = false;
            }
            OnKeyLoad();
        }

        #endregion

        #region Public Methods

        #region En-/Decryption

        /// <summary>
        /// Encrypt plain bytes
        /// </summary>
        /// <param name="plainBytes">The plain bytes to encrypt</param>
        /// <returns>The encrypted bytes and null if failed</returns>
        public byte[] Encrypt(byte[] plainBytes)
        {
            byte[] encryption = null;
            try
            {
                encryption = ((IAsymmetricEncryption)SelectedCipher).EncryptBytes(PublicKey, plainBytes);
            }
            catch (CryptographicException msdnException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(msdnException, "Encryption Failure", WindowDialogType.Error);
            }
            catch (CryptoException bouncyException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(bouncyException, "Encryption Failure", WindowDialogType.Error);
            }
            return encryption;
        }

        /// <summary>
        /// Encrypt plain text
        /// </summary>
        /// <param name="plainText">The plain text to encrypt</param>
        /// <returns>The encrypted bytes and null if failed</returns>
        public byte[] Encrypt(string plainText)
        {
            byte[] encryption = null;
            try
            {
                //Encrypt the plain text
                encryption = ((IAsymmetricEncryption)SelectedCipher).EncryptText(PublicKey, plainText);
            }
            catch (CryptographicException msdnException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(msdnException, "Encryption Failure", WindowDialogType.Error);
            }
            catch (CryptoException bouncyException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(bouncyException, "Encryption Failure", WindowDialogType.Error);
            }
            return encryption;
        }

        /// <summary>
        /// Decrypt to plain bytes
        /// </summary>
        /// <param name="encrypted">encrypted bytes</param>
        /// <returns>Plain bytes</returns>
        public byte[] Decrypt(byte[] encrypted)
        {
            byte[] plain = null;
            try
            {
                plain = ((IAsymmetricEncryption)SelectedCipher).DecryptToBytes(PrivateKey, encrypted);
            }
            catch (CryptographicException msdnException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(msdnException, "Decryption Failure", WindowDialogType.Error);
            }
            catch (CryptoException bouncyException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(bouncyException, "Decryption Failure", WindowDialogType.Error);
            }
            return plain;
        }

        /// <summary>
        /// Decrypt to text
        /// </summary>
        /// <param name="encrypted">encrypted bytes</param>
        /// <returns>plain text</returns>
        public string DecryptToText(byte[] encrypted)
        {
            string decryptedText = string.Empty;
            try
            {
                return ((IAsymmetricEncryption)SelectedCipher).DecryptToText(PrivateKey, encrypted);
            }
            catch (CryptographicException msdnException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(msdnException, "Decryption Failure", WindowDialogType.Error);
            }
            catch (CryptoException bouncyException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(bouncyException, "Decryption Failure", WindowDialogType.Error);
            }
            return decryptedText;
        }


        #endregion

        #region Digital Signatures

        public byte[] Sign(byte[] privateKey, byte[] data)
        {
            byte[] signature = null;
            try
            {
                signature = ((IAsymmetricSignature)SelectedCipher).Sign(privateKey, data);
            }
            catch(CryptographicException msdnException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(msdnException, "Signature Failure", WindowDialogType.Error);
            }
            return signature;
        }

        /// <summary>
        /// Verifies a signature for a given data set
        /// </summary>
        /// <param name="signature">The signature to verify</param>
        /// <param name="publicKey">The public key to use for verification</param>
        /// <param name="data">The data which was signed</param>
        /// <returns></returns>
        public bool Verify(byte[] signature, byte[] publicKey, byte[] data)
        {
            bool verified = false;
            try
            {
                verified = ((IAsymmetricSignature)SelectedCipher).Verify(signature, publicKey, data);
            }
            catch (CryptographicException msdnException)
            {
                //Show error message box dialog to user
                Dialog.OpenErrorMessageBoxAsync(msdnException, "Signature Verification Failure", WindowDialogType.Error);
            }
            return verified;
        }

        #endregion

        /// <summary>
        /// Derives a shared key from a private key and another persons public key
        /// </summary>
        /// <returns></returns>
        public byte[] DeriveKey()
        {
            return ((IAsymmetricKeyExchange)SelectedCipher).DeriveKey(PrivateKey, KeySizes[KeySizeIndex], OtherPartyPublicKey);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize any lists
        /// </summary>
        private void InitializeLists()
        {
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
        }

        /// <summary>
        /// Initialize any commands
        /// </summary>
        private void InitializeCommands()
        {
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            ChangedProviderCommand = new RelayCommand(ChangedProvider);
            ChangedKeySizeCommand = new RelayCommand(ChangedKeySize);
            ChangedEcCurveCommand = new RelayCommand(ChangedEcCurve);
            CreateKeyPairCommand = new RelayCommand(CreateKeyPair);
            LoadKeyCommand = new RelayCommand(LoadKey);
            DeleteKeyCommand = new RelayCommand(DeleteKey);
            GetPrivateKeyFilePathCommand = new RelayCommand(GetPrivateKeyFilePath);
            GetPublicKeyFilePathCommand = new RelayCommand(GetPublicKeyFilePath);
            GetOtherPublicKeyFilePathCommand = new RelayCommand(GetOtherPublicKeyFilePath);
        }

        /// <summary>
        /// Initializes any properties
        /// </summary>
        /// <param name="api">the crypto api to use</param>
        /// <param name="operation">The operation to use</param>
        private void InitializeProperties(CryptographyApi api, AsymmetricOperation operation)
        {
            Api = api;
            SelectedOperation = operation;
            KeyDirectoryPath = pKeyPath;
        }

        /// <summary>
        /// Clears the file paths and sets key pair loaded flags to false
        /// </summary>
        private void UnloadKeyPair()
        {
            PrivateKeyFilePath = string.Empty;
            PublicKeyFilePath = string.Empty;
            PrivateKeyLoaded = false;
            PublicKeyLoaded = false;
            OnKeyLoad();
        }

        /// <summary>
        /// Triggers the Keys loaded event
        /// </summary>
        private void OnKeyLoad()
        {
            KeysLoaded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the key pair changed event
        /// </summary>
        private void OnKeyPairChanged()
        {
            KeyPairChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
