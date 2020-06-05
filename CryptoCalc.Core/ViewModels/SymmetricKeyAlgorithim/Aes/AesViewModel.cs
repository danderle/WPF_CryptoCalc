using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the aes page
    /// </summary>
    public class AesViewModel : BaseViewModel
    {
        #region Public Properties

        public string FilePath { get; set; }
        public string PlainText { get; set; }
        public string EncryptedFilePath { get; set; }
        public string EncryptedText { get; set; }
        public string DecryptedFilePath { get; set; }
        public string DecryptedText { get; set; }
        public string SecretKey { get; set; }

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; }

        /// <summary>
        /// Holds the data format options
        /// </summary>
        public List<string> DataFormatOptions { get; set; } = new List<string>();

        #endregion

        #region Commands

        public ICommand EncryptCommand { get; set; }

        public ICommand DecryptCommand { get; set; }

        public ICommand DropCommand { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AesViewModel()
        {
            //Initialize the commands
            EncryptCommand = new RelayCommand(Encrypt);
            DecryptCommand = new RelayCommand(Decrypt);
            DropCommand = new RelayParameterizedCommand(Drop);

            DataFormatOptions.Add(Format.File.ToString());
            DataFormatOptions.Add(Format.TextString.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        private void Drop(object obj)
        {
            string[] paths = (string[])obj;
            if (File.Exists(paths[0]))
            {
                FilePath = paths[0];
            }
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method to decrypt the given data
        /// </summary>
        private void Decrypt()
        {
            byte[] encrypted  = null;
            switch(DataFormatSelected)
            {
                case Format.File:
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);
                    var decryptedBytes = SymmetricCypher.AesDecryptToByte(encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    DecryptedText = SymmetricCypher.AesDecryptToText(encrypted);
                    break;
            }
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            byte[] encrypted;
            switch (DataFormatSelected)
            {
                case Format.File:
                    var plainBytes = File.ReadAllBytes(FilePath);
                    var encryptedBytes = SymmetricCypher.AesEncrypt(plainBytes);
                    var extension = Path.GetExtension(FilePath);
                    EncryptedFilePath =  Path.Combine(Directory.GetParent(FilePath).ToString(), Path.GetFileNameWithoutExtension(FilePath) + ".Encrypted" + extension);
                    File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = SymmetricCypher.AesEncrypt(PlainText);
                    EncryptedText = ByteConvert.BytesToString(encrypted);
                    break;
            }
        }

        #endregion
    }
}
