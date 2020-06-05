using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for AesPage.xaml
    /// </summary>
    public partial class AesPage : BasePage<AesViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AesPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public AesPage(AesViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
