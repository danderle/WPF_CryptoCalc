using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for AesPage.xaml
    /// </summary>
    public partial class SymmetricCipherPage : BasePage<SymmetricCipherViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SymmetricCipherPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public SymmetricCipherPage(SymmetricCipherViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
