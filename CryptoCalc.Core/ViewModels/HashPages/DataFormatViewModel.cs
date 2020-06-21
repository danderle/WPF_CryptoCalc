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
        #region Properties

        /// <summary>
        /// Keyed-Hash Message Authentication Code
        /// </summary>
        public bool HmacChecked { get; set; } = false;

        /// <summary>
        /// The data to be hashed <see cref="Format"/> for hash data format options
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// The Hmac key
        /// </summary>
        public string Key { get; set; } = string.Empty;

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
        /// The command to execute a drag and drop
        /// </summary>
        public ICommand DropCommand { get; set; }

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
            DropCommand = new RelayParameterizedCommand(Drop);
            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogAsync);

            // Adds the formats to the lists
            DataFormatOptions = Enum.GetValues(typeof(Format)).Cast<Format>().Select(t => t.ToString()).ToList();
            KeyFormat.Add(Format.TextString.ToString());
            KeyFormat.Add(Format.HexString.ToString());
        }


        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to show a help document
        /// </summary>
        private async void OpenFolderDialogAsync()
        {
            await Ioc.UI.ShowFolderDialog(new FolderBrowserDialogViewModel());
        }

        /// <summary>
        /// The commands method to evaluate the dropped file path
        /// </summary>
        /// <param name="obj"></param>
        private void Drop(object obj)
        {
            string[] paths = (string[])obj;
            if (File.Exists(paths[0]))
            {
                Data = paths[0];
            }
        }

        #endregion
    }
}
