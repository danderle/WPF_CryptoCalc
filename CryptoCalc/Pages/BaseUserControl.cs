using System.Windows.Controls;
using System.Windows;
using System.Threading.Tasks;
using CryptoCalc.Core;
using System.ComponentModel;

namespace CryptoCalc
{
    /// <summary>
    /// A base page for all pages to gain base functionality
    /// </summary>
    public class BaseUserControl : UserControl
    {
        #region Public Properties

        /// <summary>
        /// The animation to play when a page is loaded
        /// </summary>
        public PageAnimation PageLoadAnimation { get; set; } = PageAnimation.SlideAndFadeInFromRight;

        /// <summary>
        /// The animation to play when a page is unloaded
        /// </summary>
        public PageAnimation PageUnloadAnimation { get; set; } = PageAnimation.SlideAndFadeOutToLeft;

        /// <summary>
        /// The time slide animations need to complete
        /// </summary>
        public float SlideSeconds { get; set; } = 0.4f;

        /// <summary>
        /// A flag indicating if this page should animate out on load
        /// Useful for when moving a page to another frame
        /// </summary>
        public bool ShouldAnimateOut { get; set; }

        #endregion

        #region Constructor

        public BaseUserControl()
        {
            //If no animation is set, dont show anything
            if (PageLoadAnimation != PageAnimation.None)
            {
                Visibility = Visibility.Collapsed;
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                //Listen for the page loading
                Loaded += BasePage_LoadedAsync;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Animation Load / Unload

        /// <summary>
        /// Listens for the page being loaded tp then do animation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BasePage_LoadedAsync(object sender, RoutedEventArgs e)
        {
            //If we are setup to animate out on load
            if(ShouldAnimateOut)
            {
                //Animate out
                await AnimateOutAsync();
            }
            else
            {
                // Animate in
                await AnimateInAsync();
            }
        }

        /// <summary>
        /// Animates the page in
        /// </summary>
        /// <returns></returns>
        public async Task AnimateInAsync()
        {
            ///Make sure that an animation is set
            if (PageLoadAnimation == PageAnimation.None)
            {
                return;
            }

            switch (PageLoadAnimation)
            {
                case PageAnimation.SlideAndFadeInFromRight:

                    //Start the animation
                    await this.SlideAndFadeInFromRightAsync(SlideSeconds, width: (int)Application.Current.MainWindow.Width);
                    break;
                case PageAnimation.SlideAndFadeOutToLeft:

                    break;

            }
        }

        /// <summary>
        /// Animates the page out
        /// </summary>
        /// <returns></returns>
        public async Task AnimateOutAsync()
        {
            ///Make sure that an animation is set
            if (PageUnloadAnimation == PageAnimation.None)
            {
                return;
            }

            switch (PageUnloadAnimation)
            {
                case PageAnimation.SlideAndFadeInFromRight:
                    //Start the animation
                    await this.SlideAndFadeInFromRightAsync(SlideSeconds, width: (int)Application.Current.MainWindow.Width);
                    break;
                case PageAnimation.SlideAndFadeOutToLeft:
                    //Sart the animation
                    await this.SlideAndFadeOutToLeftAsync(SlideSeconds, width: (int)Application.Current.MainWindow.Width);
                    break;

            }
        }

        #endregion
    }
}
