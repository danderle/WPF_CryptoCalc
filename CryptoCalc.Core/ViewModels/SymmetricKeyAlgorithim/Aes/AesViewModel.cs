using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model for the aes page
    /// </summary>
    public class AesViewModel : BaseViewModel
    {
        #region Private fields

        #endregion

        #region Public Properties

        public string FilePath { get; set; }
        public string PlainText { get; set; }
        public string EncryptedFilePath { get; set; }
        public string EncryptedText { get; set; }
        public string DecryptedFilePath { get; set; }
        public string DecryptedText { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Currently selected data format
        /// </summary>
        public Format DataFormatSelected { get; set; }

        /// <summary>
        /// Holds the data format options
        /// </summary>
        public List<string> DataFormatOptions { get; set; } = new List<string>();

        public Aes AesCrypt { get; set; } = SymmetricCypher.AesCrypt;

        public int KeySizeIndex { get; set; }

        public List<int> KeySizes { get; set; } = new List<int>();

        #endregion

        #region Commands

        /// <summary>
        /// The command to encrypt
        /// </summary>
        public ICommand EncryptCommand { get; set; }

        /// <summary>
        /// The command to decrypt
        /// </summary>
        public ICommand DecryptCommand { get; set; }

        /// <summary>
        /// The command to drop files
        /// </summary>
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
            foreach(var legalkeySize in AesCrypt.LegalKeySizes)
            {
                int keySize = legalkeySize.MinSize;
                while(keySize <= legalkeySize.MaxSize)
                {
                    KeySizes.Add(keySize);
                    keySize += legalkeySize.SkipSize;
                }
            }
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// The command method for dropping files
        /// </summary>
        private void Drop(object obj)
        {
            string[] paths = (string[])obj;
            if (File.Exists(paths[0]))
            {
                FilePath = paths[0];
            }
        }

        /// <summary>
        /// The command method to decrypt the given data
        /// </summary>
        private void Decrypt()
        {
            byte[] encrypted  = null;
            var password = ByteConvert.StringToBytes(Password);
            switch (DataFormatSelected)
            {
                case Format.File:
                    encrypted = ByteConvert.FileToBytes(EncryptedFilePath);
                    var decryptedBytes = SymmetricCypher.AesDecryptToByte(password, KeySizes[KeySizeIndex], encrypted);
                    var extension = Path.GetExtension(EncryptedFilePath);
                    DecryptedFilePath = Path.Combine(Directory.GetParent(EncryptedFilePath).ToString(), Path.GetFileNameWithoutExtension(EncryptedFilePath) + ".Decrypted" + extension);
                    File.WriteAllBytes(DecryptedFilePath, decryptedBytes);
                    break;
                case Format.TextString:
                    encrypted = ByteConvert.HexStringToBytes(EncryptedText);
                    DecryptedText = SymmetricCypher.AesDecryptToText(password, KeySizes[KeySizeIndex], encrypted);
                    break;
            }
        }

        /// <summary>
        /// The command method to encrypt the given data
        /// </summary>
        private void Encrypt()
        {
            byte[] encrypted;
            var password = ByteConvert.StringToBytes(Password);
            switch (DataFormatSelected)
            {
                case Format.File:
                    if (File.Exists(FilePath))
                    {
                        var plainBytes = File.ReadAllBytes(FilePath);
                        var encryptedBytes = SymmetricCypher.AesEncrypt(password, KeySizes[KeySizeIndex], plainBytes);
                        var extension = Path.GetExtension(FilePath);
                        EncryptedFilePath = Path.Combine(Directory.GetParent(FilePath).ToString(), Path.GetFileNameWithoutExtension(FilePath) + ".Encrypted" + extension);
                        File.WriteAllBytes(EncryptedFilePath, encryptedBytes);
                    }
                break;
                case Format.TextString:
                    encrypted = SymmetricCypher.AesEncrypt(password, KeySizes[KeySizeIndex], PlainText);
                    EncryptedText = ByteConvert.BytesToString(encrypted);
                    break;
            }
        }

        #endregion
    }
}
