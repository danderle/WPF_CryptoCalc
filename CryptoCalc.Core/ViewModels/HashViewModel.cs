﻿using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace CryptoCalc.Core
{
    /// <summary>
    /// The view model forthe hash page
    /// </summary>
    public class HashViewModel : BaseViewModel
    {

        #region Properties

        /// <summary>
        /// The view model to setup the hashing process
        /// </summary>
        public DataFormatViewModel DataFormatSetup { get; set; } = new DataFormatViewModel();

        /// <summary>
        /// The view model that holds all the hashing methods
        /// </summary>
        public HashItemListViewModel HashList { get; set; } = new HashItemListViewModel();

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
            CalculateCommand = new RelayCommand(Calculate);
            CloseCommand = new RelayCommand(Close);
            HelpCommand = new RelayCommand(Help);

        }

        #endregion

        #region Command Methods


        private void Help()
        {
            throw new NotImplementedException();
        }

        private void Close()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The command method to calculate the hash values
        /// </summary>
        private void Calculate()
        {
            byte[] data = null;
            switch((DataHashFormat)DataFormatSetup.DataFormatSelected)
            {
                case DataHashFormat.File:
                    break;
                case DataHashFormat.TextString:
                    data = Encoding.ASCII.GetBytes(DataFormatSetup.Data);
                    break;
                case DataHashFormat.HexString:
                    break;
                default:
                    Debugger.Break();
                    break;
            }

            foreach(var item in HashList.Items)
            {
                if(item.IsChecked)
                {
                    item.CalculateHash(data);
                }
            }
        }

        #endregion
    }
}
