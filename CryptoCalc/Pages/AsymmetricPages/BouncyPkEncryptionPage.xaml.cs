using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for BouncyPkEncryptionPage.xaml
    /// </summary>
    public partial class BouncyPkEncryptionPage : BasePage<AsymmetricViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BouncyPkEncryptionPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public BouncyPkEncryptionPage(AsymmetricViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
