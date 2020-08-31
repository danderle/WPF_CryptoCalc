using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for HmacControl.xaml
    /// </summary>
    public partial class HmacControl : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// Dependency property for determining if the hmac checkbox is selected
        /// </summary>
        public bool HmacSelected
        {
            get => (bool)GetValue(HmacSelectedProperty);
            set => SetValue(HmacSelectedProperty, value);
        }

        /// <summary>
        /// Registers <see cref="HmacSelected"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty HmacSelectedProperty =
            DependencyProperty.Register(nameof(HmacSelected), typeof(bool), typeof(HmacControl), new PropertyMetadata(false));

        #endregion

        #region Constructor

        public HmacControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
