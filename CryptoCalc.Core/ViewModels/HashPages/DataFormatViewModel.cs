using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Input;
using System.IO;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the <see cref="DataFormatControl.xaml"/>
    /// </summary>
    public class DataFormatViewModel : BaseViewModel
    {
        #region Private Members

        private string data = string.Empty;
        private string hmacKey = string.Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// Flag letting us know if the data is ready for hashing
        /// </summary>
        public bool DataIsReadyForProcessing
        {
            get
            {
                if(HmacChecked)
                {
                    return DataIsCorrectlyFormatted && HmacKeyIsCorrectlyFormatted;
                }
                else
                {
                    return DataIsCorrectlyFormatted;
                }
            }
        }

        /// <summary>
        /// Flag to know if the Data has the correct format
        /// </summary>
        public bool DataIsCorrectlyFormatted { get; set; }

        /// <summary>
        /// Flag to know if the HMAC key has the correct format
        /// </summary>
        public bool HmacKeyIsCorrectlyFormatted { get; set; }

        /// <summary>
        /// Keyed-Hash Message Authentication Code
        /// </summary>
        public bool HmacChecked { get; set; } = false;

        /// <summary>
        /// The data to be hashed <see cref="Format"/> for hash data format options
        /// </summary>
        public string Data 
        {
            get => data;
            set
            {
                if(value != data)
                {
                    data = value;
                    DataIsCorrectlyFormatted = CheckIfCorrectlyFormatted(DataFormatSelected, data);
                }
            }
        }

        /// <summary>
        /// The Hmac key
        /// </summary>
        public string Key
        {
            get => hmacKey;
            set
            {
                if (value != hmacKey)
                {
                    hmacKey = value;
                    HmacKeyIsCorrectlyFormatted = CheckIfCorrectlyFormatted((KeyFormatSelected + 1), hmacKey);
                }
            }
        }

        /// <summary>
        /// List for holding the key format options
        /// </summary>
        public List<string> KeyFormat { get; set; } = new List<string>();

        /// <summary>
        /// Holds the data format options
        /// </summary>
        public List<string> DataFormatOptions { get; set; } = new List<string>();

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; } = Format.File;

        /// <summary>
        /// Currently selected key format
        /// </summary>
        public Format KeyFormatSelected { get; set; } = Format.TextString;

        #endregion

        #region Commands

        /// <summary>
        /// The command to execute when the data format is changed
        /// </summary>
        public ICommand DataFormatSelectionChangedCommand { get; set; }

        /// <summary>
        /// The command to execute when the hamc key format is changed
        /// </summary>
        public ICommand KeyFormatSelectionChangedCommand { get; set; }

        /// <summary>
        /// The command to open a folder dialog window
        /// </summary>
        public ICommand OpenFolderDialogCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataFormatViewModel()
        {
            //Initialize commands
            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogAsync);
            DataFormatSelectionChangedCommand = new RelayCommand(DataFormatSelectionChanged);
            KeyFormatSelectionChangedCommand = new RelayCommand(KeyFormatSelectionChanged);

            // Adds the formats to the lists
            DataFormatOptions = Enum.GetValues(typeof(Format)).Cast<Format>().Select(t => t.ToString()).ToList();
            KeyFormat.Add(Format.TextString.ToString());
            KeyFormat.Add(Format.HexString.ToString());
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to verify that the new key format selected
        /// is correctly implemented on current key string
        /// </summary>
        private void KeyFormatSelectionChanged()
        {
            HmacKeyIsCorrectlyFormatted = CheckIfCorrectlyFormatted(KeyFormatSelected, Key);
        }

        /// <summary>
        /// The command method to verify that the new data format selected
        /// is correctly implemented on current key string
        /// </summary>
        private void DataFormatSelectionChanged()
        {
            DataIsCorrectlyFormatted = CheckIfCorrectlyFormatted(DataFormatSelected, Data);
        }

        /// <summary>
        /// The command method to show a help document
        /// </summary>
        private async void OpenFolderDialogAsync()
        {
            await Ioc.UI.ShowFolderDialog(new FolderBrowserDialogViewModel());
            Data = Ioc.Application.FilePathFromDialogSelection;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifies if only hex characters are used in given text
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if all chars are hex values</returns>
        private bool OnlyHexInString(string text)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        /// <summary>
        /// Verifies if the correct format is implemented on given text
        /// </summary>
        /// <param name="format">The format used</param>
        /// <param name="text">The text entered by user</param>
        /// <returns>True if correct format</returns>
        private bool CheckIfCorrectlyFormatted(Format format, string text)
        {
            switch (format)
            {
                case Format.File:
                    return File.Exists(text);
                case Format.HexString:
                    return text.Length % 2 == 0 && OnlyHexInString(text);
                default:
                    return text.Length > 0;
            }
        }
        #endregion
    }
}
