﻿using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for MsdnHashPage.xaml
    /// </summary>
    public partial class MsdnHashPage : BasePage<HashViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnHashPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public MsdnHashPage(HashViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
