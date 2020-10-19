using CryptoCalc.Core;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for PageHost.xaml
    /// </summary>
    public partial class PageHost : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// The current page to show in the page host
        /// </summary>
        public ApplicationPage CurrentPage
        {
            get => (ApplicationPage)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        /// <summary>
        /// Registers <see cref="CurrentPage"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(ApplicationPage), typeof(PageHost), new UIPropertyMetadata(default(ApplicationPage), null, CurrentPagePropertyChanged));

        /// <summary>
        /// The current view model used for the current page
        /// </summary>
        public BasePage CurrentPageViewModel
        {
            get => (BasePage)GetValue(CurrentPageViewModelProperty);
            set => SetValue(CurrentPageViewModelProperty, value);
        }

        /// <summary>
        /// Registers <see cref="CurrentPageViewModel"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageViewModelProperty =
            DependencyProperty.Register(nameof(CurrentPageViewModel), typeof(BaseViewModel), typeof(PageHost), new UIPropertyMetadata());

        #endregion

        #region Constructor

        public PageHost()
        {
            InitializeComponent();

            //if in design mode, show the current page,
            //as the dependency property does not fire
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                NewPage.Content = DI.Application.CurrentPage.ToBasePage();
            }
        }

        #endregion

        #region Property Changed events

        /// <summary>
        /// Called when the <see cref="CurrentPage"/> value has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static object CurrentPagePropertyChanged(DependencyObject d, object value)
        {
            // Get current values
            var currentPage = (ApplicationPage)d.GetValue(CurrentPageProperty);
            var currentPageViewModel = d.GetValue(CurrentPageViewModelProperty);

            //Get the frames
            var newPageFrame = (d as PageHost).NewPage;
            var oldPageFrame = (d as PageHost).OldPage;

            // If the current page hasnt changed
            //just update the view model
            if (newPageFrame.Content is BasePage page
                 && page.ToApplicationPage() == currentPage )
            {
                // Just update the view model
                page.ViewModelObject = currentPageViewModel;

                return value;
            }

            

            //Store the current page content as the old page
            var oldPageContent = newPageFrame.Content;

            //Remove the current page from the new page frame
            newPageFrame.Content = null;

            //Move the previous page int the old page frame
            oldPageFrame.Content = oldPageContent;

            //Animate out previous page when the loaded event is fired
            //Right after thic call due to moving frames
            if (oldPageContent is BasePage oldPage)
            {
                oldPage.ShouldAnimateOut = true;

                //Once it is done , remove it
                Task.Delay((int)(oldPage.SlideSeconds * 1000)).ContinueWith((t) =>
                {
                    //Remove old page go back to UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        oldPageFrame.Content = null;
                    });
                });
            }

            //Set the new page content
            newPageFrame.Content = currentPage.ToBasePage(currentPageViewModel);
            return value;
        }

        #endregion
    }
}
