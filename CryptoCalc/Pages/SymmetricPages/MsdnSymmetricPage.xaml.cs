﻿using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for MsdnSymmetricPage.xaml
    /// </summary>
    public partial class MsdnSymmetricPage : BasePage<SymmetricViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnSymmetricPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public MsdnSymmetricPage(SymmetricViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
