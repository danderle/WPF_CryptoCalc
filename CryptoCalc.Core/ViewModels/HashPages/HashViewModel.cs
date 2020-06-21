using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the Msdn Hash page
    /// </summary>
    public class HashViewModel : BaseViewModel
    {
        #region Private Fields

        private bool useBouncyApi = false;

        #endregion

        #region Properties

        /// <summary>
        /// The view model to setup the hashing process
        /// </summary>
        public DataFormatViewModel DataFormatSetup { get; set; } = Ioc.Get<DataFormatViewModel>();

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
            CalculateCommand = new RelayCommand(Calculate);
            CloseCommand = new RelayCommand(Close);
            HelpCommand = new RelayCommand(HelpAsync);
        }

        public HashViewModel(bool bouncyApi)
            : base()
        {
            useBouncyApi = bouncyApi;
            if(useBouncyApi)
            {
                HashList = new HashItemListViewModel(useBouncyApi);
            }
            else
            {
                HashList = new HashItemListViewModel(false);
            }
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Show the Help dialog window
        /// </summary>
        private async void HelpAsync()
        {
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

            var data = GetBytesAccordingToFormatSelected(DataFormatSetup.DataFormatSelected, DataFormatSetup.Data);

            //Check which hash options are checked and then calculate
            foreach(var item in HashList.Items)
            {
                if(item.IsChecked)
                {
                    item.CalculateHash(data, key, useBouncyApi);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private byte[] GetBytesAccordingToFormatSelected(Format format, string data)
        {
            byte[] bytes = null;
            switch (DataFormatSetup.DataFormatSelected)
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
    }
}
