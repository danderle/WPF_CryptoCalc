using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for AsymmetricCipherPage.xaml
    /// </summary>
    public partial class AsymmetricCipherPage : BasePage<AsymmetricCipherViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AsymmetricCipherPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public AsymmetricCipherPage(AsymmetricCipherViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
