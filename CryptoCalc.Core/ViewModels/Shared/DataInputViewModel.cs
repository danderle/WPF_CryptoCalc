using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace CryptoCalc.Core
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
                if(data != value)
                {
                    data = value;
                    DataIsCorrectlyFormatted = CheckIfCorrectlyFormatted(DataFormatSelected, Data);
                    OnDataChanged();
                }
            }
        }

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; } = Format.TextString;

        /// <summary>
        /// A delegate event handler function for the ny data changes
        /// </summary>
        /// <param name="obj">the object from which event is triggered</param>
        /// <param name="args">The event arguments</param>
        public delegate void DataChangedEventHandler(object obj, EventArgs args);

        /// <summary>
        /// The data changed event
        /// </summary>
        public event DataChangedEventHandler DataChanged;

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
        /// The command to exectue when a different format is selected
        /// </summary>
        public ICommand ChangedFormatCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataInputViewModel()
        {
            // Adds the formats to the lists
            DataFormatOptions = Enum.GetValues(typeof(Format)).Cast<Format>().Select(t => t.ToString()).ToList();

            OpenFolderDialogCommand = new RelayCommand(OpenFolderDialogAsync);
            ChangedFormatCommand = new RelayCommand(ChangedFormat);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to execute when the data format is changed
        /// </summary>
        private void ChangedFormat()
        {
            Data = string.Empty;
            OnDataChanged();
        }

        /// <summary>
        /// The command method to show a Folder dialog window and saves the selected file path
        /// </summary>
        private void OpenFolderDialogAsync()
        {
            //Opens a pop up window folder browser dialog
            Dialog.OpenFolderBrowserAsync();

            //Saves the selected path
            Data = Ioc.Application.FilePathFromDialogSelection;
        }

        #endregion

        /// <summary>
        /// Gets the bytes from a file
        /// </summary>
        /// <returns>File bytes</returns>
        public byte[] GetBytesFromFile()
        {
            return File.ReadAllBytes(Data);
        }

        /// <summary>
        /// Create an encrypted file path from the plain file path
        /// </summary>
        /// <returns></returns>
        public string GetEncryptedFilePath()
        {
            var extension = Path.GetExtension(Data);
            return Path.Combine(Directory.GetParent(Data).ToString(), Path.GetFileNameWithoutExtension(Data) + ".Encrypted" + extension);
        }

        /// <summary>
        /// Create an decrypted file path from the given encrypted file path
        /// </summary>
        /// <param name="encryptedFilePath"></param>
        /// <returns></returns>
        public string GetDecryptedFilePath(string encryptedFilePath)
        {
            var extension = Path.GetExtension(encryptedFilePath);
            return Path.Combine(Directory.GetParent(encryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(encryptedFilePath) + ".Decrypted" + extension);
        }
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

        /// <summary>
        /// The function to be called when the <see cref="Data"/> gets changed
        /// </summary>
        protected virtual void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }

}
