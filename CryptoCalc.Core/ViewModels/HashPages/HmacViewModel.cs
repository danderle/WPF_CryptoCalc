using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the <see cref="HmacControl.xaml"/>
    /// </summary>
    public class HmacViewModel : BaseViewModel
    {
        #region Private Members

        private string hmacKey = string.Empty;

        #endregion

        #region Public Properties

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
        public HmacViewModel()
        {
            //Initialize commands
            KeyFormatSelectionChangedCommand = new RelayCommand(KeyFormatSelectionChanged);

            // Adds the formats to the lists
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
