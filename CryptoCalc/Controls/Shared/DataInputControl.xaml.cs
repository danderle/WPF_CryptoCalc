using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for DataInputControl.xaml
    /// </summary>
    public partial class DataInputControl : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// Dependency property for determining if the maxWidth of any comboBoxes
        /// </summary>
        public double ComboBoxMaxWidth
        {
            get => (double)GetValue(ComboBoxMaxWidthProperty);
            set => SetValue(ComboBoxMaxWidthProperty, value);
        }

        /// <summary>
        /// Registers <see cref="ComboBoxMaxWidth"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty ComboBoxMaxWidthProperty =
            DependencyProperty.Register(nameof(ComboBoxMaxWidth), typeof(double), typeof(DataInputControl), new PropertyMetadata(0.0));

        #endregion

        #region Constructor

        public DataInputControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
