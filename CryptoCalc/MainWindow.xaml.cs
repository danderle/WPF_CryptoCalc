using System.Windows;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this);
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            //show overlay if we lose focuse
            (DataContext as WindowViewModel).DimmableOverlayVisible = false;
        }

        private void Window_Deactivated(object sender, System.EventArgs e)
        {
            //Hide the overlay when we are focused
            (DataContext as WindowViewModel).DimmableOverlayVisible = true;
        }
    }
}
