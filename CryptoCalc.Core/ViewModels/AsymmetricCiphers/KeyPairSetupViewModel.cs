using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the key pair setup control
    /// </summary>
    public class KeyPairSetupViewModel : BaseViewModel
    {
        #region Private fields

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
        /// The private key as a hex string
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;

        /// <summary>
        /// The public key as a hex string
        /// </summary>
        public string PublicKey { get; set; } = string.Empty;

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
            switch (Api)
            {
                case CryptographyApi.MSDN:
                    SelectedCipher = IAsymmetricCipher.GetMsdnCipher(Algorithims[SelectedAlgorithimIndex]);
                    break;
                case CryptographyApi.BouncyCastle:
                    SelectedCipher = IAsymmetricCipher.GetBouncyCipher(Algorithims[SelectedAlgorithimIndex]);
                    break;
            }
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
            if (SelectedCipher is IECAlgorithims ecCipher)
            {
                if (EcCurves.Count != 0)
                {
                    ecCipher.CreateKeyPair(EcCurves[EcCurveIndex]);
                }
            }
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
            PrivateKey = ByteConvert.BytesToHexString(SelectedCipher.GetPrivateKey());
            PublicKey = ByteConvert.BytesToHexString(SelectedCipher.GetPublicKey());
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
        }

        /// <summary>
        /// Initialize any commands
        /// </summary>
        private void InitializeCommands()
        {
            ChangedAlgorithimCommand = new RelayCommand(ChangedAlgorithim);
            CreateKeyPairCommand = new RelayCommand(CreateKeyPair);
            ChangedProviderCommand = new RelayCommand(ChangedProvider);
        }
        #endregion
    }
}
