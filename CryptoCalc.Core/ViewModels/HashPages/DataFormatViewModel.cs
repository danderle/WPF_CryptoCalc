using CryptoCalc.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the <see cref="DataFormatControl.xaml"/>
    /// </summary>
    public class DataFormatViewModel : BaseViewModel
    {
        #region Private Members

        private string hmacKey = string.Empty;

        #endregion

        #region Public Properties

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
                if(HmacChecked)
                {
                    return DataCorrect && HmacKeyIsCorrectlyFormatted;
                }
                else
                {
                    return DataCorrect;
                }
            }
        }

        /// <summary>
        /// Flag to know if the HMAC key has the correct format
        /// </summary>
        public bool HmacKeyIsCorrectlyFormatted { get; set; }


        /// <summary>
        /// Keyed-Hash Message Authentication Code
        /// </summary>
        public bool HmacChecked { get; set; } = false;

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
                    HmacKeyIsCorrectlyFormatted = CheckIfCorrectlyFormatted((KeyFormatSelected), hmacKey);
                }
            }
        }

        public DataInputViewModel DataSetup { get; set; } = new DataInputViewModel();

        /// <summary>
        /// List for holding the key format options
        /// </summary>
        public List<string> KeyFormat { get; set; } = new List<string>();

        /// <summary>
        /// Currently selected key format
        /// </summary>
        public Format KeyFormatSelected { get; set; } = Format.TextString;

        #endregion

        #region Commands

        
        /// <summary>
        /// The command to execute when the hamc key format is changed
        /// </summary>
        public ICommand KeyFormatSelectionChangedCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataFormatViewModel()
        {
            //Initialize commands
            KeyFormatSelectionChangedCommand = new RelayCommand(KeyFormatSelectionChanged);

            // Adds the formats to the lists
            KeyFormat.Add(Format.TextString.ToString());
            KeyFormat.Add(Format.HexString.ToString());

            ///Subscribe to the DataChanged event from the <see cref="DataInputViewModel"/>
            DataSetup.DataChanged += DataChanged;
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

        #endregion

        #region Event handler subscriptions

        /// <summary>
        /// Event subscription called when the input data changes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void DataChanged(object obj, EventArgs args)
        {
            DataCorrect = DataSetup.DataIsCorrectlyFormatted;
        }

        #endregion

        #region Private Methods

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
                    return text.Length % 2 == 0 && ByteConvert.OnlyHexInString(text);
                default:
                    return text.Length > 0;
            }
        }
        #endregion
    }
}
