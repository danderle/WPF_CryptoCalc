using CryptoCalc.Core;
using System.Windows;
using System.Windows.Input;

namespace CryptoCalc
{
    /// <summary>
    /// The view model for the custom window
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        #region Private Members
        
        /// <summary>
        /// the window this viewmodel controls
        /// </summary>
        private Window window;

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        private int outerMarginSize = 10;

        /// <summary>
        /// The radius around the window
        /// </summary>
        private int windowRadius = 10;

        /// <summary>
        /// The last known dockposition
        /// </summary>
        private WindowDockPosition dockPosition = WindowDockPosition.Undocked;

        #endregion

        #region Public Properties

        /// <summary>
        /// The minimum width of the window
        /// </summary>
        public double MinimumWidth => 800;

        /// <summary>
        /// The minimum height of the window
        /// </summary>
        public double MinimumHeight => 500;

        /// <summary>
        /// True if the window should be borderless
        /// </summary>
        public bool Borderless => window.WindowState == WindowState.Maximized || dockPosition != WindowDockPosition.Undocked;

        /// <summary>
        /// The size of the resize border around the window
        /// </summary>
        public int ResizeBorder => Borderless ? 0 : 6;

        /// <summary>
        /// Size of th resize border around the window, taking int account the outer margin
        /// </summary>
        public Thickness ResizeBorderThickness => new Thickness(ResizeBorder);

        /// <summary>
        /// The padding to the inner content
        /// </summary>
        public Thickness InnerContentPresenterPadding => new Thickness(0);
        /// <summary>
        /// The radius around the window
        /// </summary>
        public int WindowRadius
        {
            get
            {
                return window.WindowState == WindowState.Maximized ? 0 : windowRadius;
            }
            set
            {
                windowRadius = value;
            }
        }

        /// <summary>
        /// The radius around the window
        /// </summary>
        public CornerRadius WindowCornerRadius => new CornerRadius(WindowRadius);
        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public int OuterMarginSize
        {
            get
            {
                return window.WindowState == WindowState.Maximized ? 0 : outerMarginSize;
            }
            set
            {
                outerMarginSize = value;
            }
        }

        /// <summary>
        /// The margin around the window to allow for a drop shadow
        /// </summary>
        public Thickness OuterMarginSizeThickness => new Thickness(OuterMarginSize);

        /// <summary>
        /// Height of the title bar 
        /// </summary>
        public int TitleHeight { get; set; } = 42;

        /// <summary>
        /// Height of the title bar 
        /// </summary>
        public GridLength TitleHeightGridLength => new GridLength(TitleHeight - OuterMarginSize);

        #endregion

        #region Commands

        /// <summary>
        /// Command to minimize the window
        /// </summary>
        public ICommand MinimizeCommand { get; set; }

        /// <summary>
        /// the command to maximize the window
        /// </summary>
        public ICommand MaximizeCommand { get; set; }

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to show the menu of the window
        /// </summary>
        public ICommand MenuCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public WindowViewModel(Window _window)
        {
            window = _window;

            //Listen for the window resizing
            window.StateChanged += (sender, evnt) =>
            {
                //fire events that are affected by a window resize
                OnPropertyChanged(nameof(ResizeBorderThickness));
                OnPropertyChanged(nameof(WindowRadius));
                OnPropertyChanged(nameof(WindowCornerRadius));
                OnPropertyChanged(nameof(OuterMarginSize));
                OnPropertyChanged(nameof(OuterMarginSizeThickness));
            };

            //Create commands
            MinimizeCommand = new RelayCommand(() => window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => window.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(window, GetMousePosition()));

            //Fix window resize
            var resize = new WindowResizer(window);
        }

        
        #endregion

        #region Private Helpers

        /// <summary>
        /// Gets the current mouse position
        /// </summary>
        /// <returns></returns>
        private Point GetMousePosition()
        {
            /// mouse position relative to the window
            var position = Mouse.GetPosition(window);
            return new Point(position.X + window.Left, position.Y + window.Top);
        }

        #endregion
    }
}
