using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace CryptoCalc.Core.Models
{
    public class DataInputViewModel : BaseViewModel
    {
        #region Private Fields

        private string data = string.Empty;

        #endregion

        #region Public Properties

        /// <summary>
        /// Flag to know if the Data has the correct format
        /// </summary>
        public bool DataIsCorrectlyFormatted { get; set; }

        /// <summary>
        /// The data to be hashed <see cref="Format"/> for hash data format options
        /// </summary>
        public string Data
        {
            get => data;
            set
            {
                if (value != data)
                {
                    data = value;
                    DataIsCorrectlyFormatted = CheckIfCorrectlyFormatted(DataFormatSelected, data);
                }
            }
        }

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; } = Format.TextString;

        /// <summary>
        /// Holds the data format options
        /// </summary>
        public List<string> DataFormatOptions { get; set; } = new List<string>(); 

        #endregion

        #region Commands

        /// <summary>
        /// The command to open a folder dialog window
        /// </summary>
        public ICommand OpenFolderDialogCommand { get; set; }

        /// <summary>
        /// The command to execute when the data format is changed
        /// </summary>
        public ICommand DataFormatSelectionChangedCommand { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public DataInputViewModel()
        {
            // Adds the formats to the lists
            DataFormatOptions = Enum.GetValues(typeof(Format)).Cast<Format>().Select(t => t.ToString()).ToList();

            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogAsync);
            DataFormatSelectionChangedCommand = new RelayCommand(DataFormatSelectionChanged);

        }

        #region Command Methods

        /// <summary>
        /// The command method to show a Folder dialog window and saves the selected file path
        /// </summary>
        private async void OpenFolderDialogAsync()
        {
            //Opens a pop up window folder browser dialog
            await Ioc.UI.ShowFolderDialog(new FolderBrowserDialogViewModel());

            //Saves the selected path
            Data = Ioc.Application.FilePathFromDialogSelection;
        }

        /// <summary>
        /// The command method to verify that the new data format selected
        /// is correctly implemented on current key string
        /// </summary>
        private void DataFormatSelectionChanged()
        {
            DataIsCorrectlyFormatted = CheckIfCorrectlyFormatted(DataFormatSelected, Data);
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
