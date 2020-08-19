using System.Diagnostics;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the Msdn Hash page
    /// </summary>
    public class HashViewModel : BaseViewModel
    {
        #region Private Fields

        private CryptographyApi crpytoApi;

        #endregion

        #region Properties

        /// <summary>
        /// The view model to setup the hashing process
        /// </summary>
        public DataFormatViewModel DataFormatSetup { get; set; } = new DataFormatViewModel();

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
        /// The command to close the application
        /// </summary>
        public ICommand CloseCommand { get; set; }

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
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Show the Help dialog window
        /// </summary>
        private async void HelpAsync()
        {
            //TODO Help dialog window
            await Ioc.UI.ShowMessage(new MessageBoxDialogViewModel
            {
                Message = "Hello this is a pop up message",
                Title = "First dialog message",
                OkText = "Press ok to continue",
            });
        }

        /// <summary>
        /// The command method to close the application
        /// </summary>
        private void Close()
        {
            Ioc.Application.Close();
        }

        /// <summary>
        /// The command method to calculate the hash values according to the selected data format
        /// </summary>
        private void Calculate()
        {
            byte[] key = null;
            if (DataFormatSetup.HmacChecked)
            {
                key = GetBytesAccordingToFormatSelected(DataFormatSetup.KeyFormatSelected, DataFormatSetup.Key);
            }

            var data = GetBytesAccordingToFormatSelected(DataFormatSetup.DataSetup.DataFormatSelected, DataFormatSetup.DataSetup.Data);

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
            switch (DataFormatSetup.DataSetup.DataFormatSelected)
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

        #region MyRegion

        /// <summary>
        /// Initializes all the commands
        /// </summary>
        private void InitializeCommands()
        {
            CalculateCommand = new RelayCommand(Calculate);
            CloseCommand = new RelayCommand(Close);
            HelpCommand = new RelayCommand(HelpAsync);
        }

        #endregion
    }
}
