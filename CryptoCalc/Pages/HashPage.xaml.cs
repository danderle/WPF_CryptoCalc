using CryptoCalc.Core;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for HashPage.xaml
    /// </summary>
    public partial class HashPage : BasePage<HashViewModel>
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with a specific view model
        /// </summary>
        public HashPage(HashViewModel specificViewModel) : base(specificViewModel)
        {
            InitializeComponent();
        }

        #endregion
    }
}
