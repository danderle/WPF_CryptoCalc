using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for MsdnPkSignaturePage.xaml
    /// </summary>
    public partial class MsdnPkSignaturePage : BasePage<AsymmetricViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MsdnPkSignaturePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public MsdnPkSignaturePage(AsymmetricViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
