using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for MsdnPkKeyExchangePage.xaml
    /// </summary>
    public partial class MsdnPkKeyExchangePage : BasePage<AsymmetricViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnPkKeyExchangePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public MsdnPkKeyExchangePage(AsymmetricViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
