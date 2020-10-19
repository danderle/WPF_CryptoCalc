using CryptoCalc.Core;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace CryptoCalc
{
    /// <summary>
    /// The view model for the Msdn Hash page
    /// </summary>
    public class HashViewModel : BaseViewModel
    {
        #region Private Fields

        private readonly CryptographyApi crpytoApi;

        #endregion

        #region Properties

        /// <summary>
        /// Flag for letting us know if the Data is correctly entered and ready for en-/decrypting
        /// </summary>
        public bool DataCorrect { get; set; }

        /// <summary>
        /// Flag letting us know if the data is ready for hashing
        /// </summary>
        public bool DataIsReadyForProcessing
        {
            get
            {
                if (HmacSetup.HmacChecked)
                {
                    return DataCorrect && HmacSetup.HmacKeyIsCorrectlyFormatted;
                }
                else
                {
                    return DataCorrect;
                }
            }
        }

        /// <summary>
        /// The view model for the data input control
        /// </summary>
        public DataInputViewModel DataInput { get; set; } = new DataInputViewModel();

        /// <summary>
        /// The view model to setup the hmac options
        /// </summary>
        public HmacViewModel HmacSetup { get; set; } = new HmacViewModel();

        /// <summary>
        /// The view model that holds all the hashing methods
        /// </summary>
        public HashItemListViewModel HashList { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to calculate the hash values
        /// </summary>
        public ICommand CalculateCommand { get; set; }

        /// <summary>
        /// The command to show help options
        /// </summary>
        public ICommand HelpCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashViewModel()
        {
            //Initialize the commands
            InitializeCommands();
        }

        
        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="api">The api to use</param>
        public HashViewModel(CryptographyApi api)
        {
            //Initialize the commands
            InitializeCommands();
            crpytoApi = api;

            //Create the hash list options according to the selected api
            HashList = new HashItemListViewModel(crpytoApi);

            ///Subscribe to the DataChanged event from the <see cref="DataInputViewModel"/>
            DataInput.DataChanged += DataChanged;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Show the Help dialog window
        /// </summary>
        private void HelpAsync()
        {
            //Opens a help message box dialog
            Dialog.OpenHelpMessageBoxAsync();
        }

        /// <summary>
        /// The command method to calculate the hash values according to the selected data format
        /// </summary>
        private void Calculate()
        {
            byte[] key = null;

            //if hmac is selected get the hmac key bytes
            if (HmacSetup.HmacChecked)
            {
                key = GetBytesAccordingToFormatSelected(HmacSetup.KeyFormatSelected, HmacSetup.Key);
            }

            //get the data bytes
            var data = GetBytesAccordingToFormatSelected(DataInput.DataFormatSelected, DataInput.Data);

            //Check which hash options are checked and then calculate
            foreach(var item in HashList.Items)
            {
                if(item.IsChecked)
                {
                    item.CalculateHash(data, key, crpytoApi);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// transforms the data to bytes according to the selected format option
        /// </summary>
        /// <param name="format">The format option to use for byte extraction</param>
        /// <param name="data">The data to extract the bytes from</param>
        /// <returns>the extracted bytes</returns>
        private byte[] GetBytesAccordingToFormatSelected(Format format, string data)
        {
            byte[] bytes = null;
            switch (format)
            {
                case Format.File:
                    bytes = ByteConvert.FileToBytes(data);
                    break;
                case Format.TextString:
                    bytes = ByteConvert.StringToAsciiBytes(data);
                    break;
                case Format.HexString:
                    bytes = ByteConvert.HexStringToBytes(data);
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            return bytes;
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

            //clear hash values on data change
            foreach(var item in HashList.Items)
            {
                item.HashValue = string.Empty;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes all the commands
        /// </summary>
        private void InitializeCommands()
        {
            CalculateCommand = new RelayCommand(Calculate);
            HelpCommand = new RelayCommand(HelpAsync);
        }

        #endregion
    }
}
