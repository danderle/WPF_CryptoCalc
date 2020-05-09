using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CryptoCalc
{
    /// <summary>
    /// Helpers to animate pages in specific ways
    /// </summary>
    public static class PageAnimations
    {
        /// <summary>
        /// Slides a page in from the right
        /// </summary>
        /// <param name="page">the page to animate</param>
        /// <param name="seconds">the time the animation takes</param>
        /// <returns></returns>
        public static async Task SlideAndFadeInFromRightAsync(this Page page, float seconds)
        {
            //Create the storyboard
            var sb = new Storyboard();

            //Add the slide from right
            sb.AddSlideFromRight(seconds, page.WindowWidth);

            //Adds the fade in
            sb.AddsFadeIn(seconds);

            //Star animation
            sb.Begin(page);

            // Make page visible
            page.Visibility = System.Windows.Visibility.Visible;

            //Wait for the animation to finish
            await Task.Delay((int)(seconds * 1000));
        }

        /// <summary>
        /// Slides a page out to the left
        /// </summary>
        /// <param name="page">the page to animate</param>
        /// <param name="seconds">the time the animation takes</param>
        /// <returns></returns>
        public static async Task SlideAndFadeOutToLeftAsync(this Page page, float seconds)
        {
            //Create the storyboard
            var sb = new Storyboard();

            //Add custom animation
            sb.AddSlideOutLeft(seconds, page.WindowWidth);

            //Adds the fade out
            sb.AddsFadeOut(seconds);

            //Star animation
            sb.Begin(page);

            // Make page visible
            page.Visibility = System.Windows.Visibility.Visible;

            //Wait for the animation to finish
            await Task.Delay((int)(seconds * 1000));
        }
    }
}
