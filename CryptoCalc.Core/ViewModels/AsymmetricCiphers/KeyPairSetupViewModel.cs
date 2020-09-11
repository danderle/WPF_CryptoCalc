using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        /// Flag to let us know if the Private key is loaded
        /// </summary>
        public bool PrivateKeyLoaded { get; set; }

        /// <summary>
        /// Flag to let us know if the public key is loaded
        /// </summary>
        public bool PublicKeyLoaded { get; set; }

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
        /// The private key
        /// </summary>
        public byte[] PrivateKey { get; set; }

        /// <summary>
        /// The public key
        /// </summary>
        public byte[] PublicKey { get; set; }

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
        /// The command to when a cipher algorithim is selected
        /// </summary>
        public ICommand ChangedAlgorithimCommand { get; set; }

        /// <summary>
        /// The command to change the provider
        /// </summary>
        public ICommand ChangedProviderCommand { get; set; }

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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public KeyPairSetupViewModel()
        {
            //Initialize the commands
            InitializeCommands();

            Api = CryptographyApi.MSDN;
            SelectedOperation = AsymmetricOperation.Encryption;

            //Initialize lists must be after setting api and operation
            InitializeLists();

            ChangedAlgorithim();

            KeyDirectoryPath = pKeyPath;
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

            Api = api;
            SelectedOperation = operation;
            
            //Initialize lists must be after setting api and operation
            InitializeLists();

            ChangedAlgorithim();

            KeyDirectoryPath = pKeyPath;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to change a provider
        /// </summary>
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
            OnKeyLoad();
        }

        /// <summary>
        /// the command method to delete a key pair
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

        /// <summary>
        /// Encrypt plain bytes
        /// </summary>
        /// <param name="plainBytes">The plain bytes to encrypt</param>
        /// <returns>The encrypted bytes</returns>
        public byte[] Encrypt(byte[] plainBytes)
        {
            return ((IAsymmetricEncryption)SelectedCipher).EncryptBytes(PublicKey, plainBytes);
        }

        /// <summary>
        /// Encrypt plain text
        /// </summary>
        /// <param name="plainText">The plain text to encrypt</param>
        /// <returns>The encrypted bytes</returns>
        public byte[] Encrypt(string plainText)
        {
            return ((IAsymmetricEncryption)SelectedCipher).EncryptText(PublicKey, plainText);
        }

        /// <summary>
        /// Decrypt to plain bytes
        /// </summary>
        /// <param name="encrypted">encrypted bytes</param>
        /// <returns>Plain bytes</returns>
        public byte[] Decrypt(byte[] encrypted)
        {
            return ((IAsymmetricEncryption)SelectedCipher).DecryptToBytes(PrivateKey, encrypted);
        }

        /// <summary>
        /// Decrypt to text
        /// </summary>
        /// <param name="encrypted">encrypted bytes</param>
        /// <returns>plain text</returns>
        public string DecryptToText(byte[] encrypted)
        {
            return ((IAsymmetricEncryption)SelectedCipher).DecryptToText(PrivateKey, encrypted);
        }

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
        }

        /// <summary>
        /// Initialize any commands
        /// </summary>
        private void InitializeCommands()
        {
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            CreateKeyPairCommand = new RelayCommand(CreateKeyPair);
            ChangedProviderCommand = new RelayCommand(ChangedProvider);
            LoadKeyCommand = new RelayCommand(LoadKey);
            DeleteKeyCommand = new RelayCommand(DeleteKey);
        }

        /// <summary>
        /// Executes the Keys loaded event
        /// </summary>
        private void OnKeyLoad()
        {
            KeysLoaded?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
