using System.Windows;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        #region Private Fields

        /// <summary>
        /// The view model for this window
        /// </summary>
        private DialogWindowViewModel viewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// the view model for this window
        /// </summary>
        public DialogWindowViewModel ViewModel
        {
            get => viewModel;
            set
            {
                //Set the new view model
                viewModel = value;

                //update the data context
                DataContext = viewModel;
            }
        }

        #endregion

        #region Constructor

        public DialogWindow()
        {
            InitializeComponent();

            DataContext = new DialogWindowViewModel(this);
        } 

        #endregion
    }
}
