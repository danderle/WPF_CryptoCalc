using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for MsdnPkEncryptionPage.xaml
    /// </summary>
    public partial class MsdnPkEncryptionPage : BasePage<AsymmetricViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnPkEncryptionPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public MsdnPkEncryptionPage(AsymmetricViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
