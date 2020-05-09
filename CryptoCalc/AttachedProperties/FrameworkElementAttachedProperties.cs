using System.Windows;

namespace CryptoCalc
{
    /// <summary>
    /// A base class to run any animation method when a bool is set to true
    /// and reverse animation when set to false
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public abstract class AnimateBaseProperty<Parent> : BaseAttachedProperty<Parent, bool>
        where Parent : BaseAttachedProperty<Parent, bool>, new()
    {

        #region Public Properties

        /// <summary>
        /// A flag indicating if this is the first time this property has been loaded
        /// </summary>
        public bool FirstLoad { get; set; } = true;

        #endregion


        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            // Get the framework element
            if(!(sender is FrameworkElement element))
            {
                return;
            }

            // Dont fire if the value doesnt changed
            if(sender.GetValue(ValueProperty) == value && !FirstLoad)
            {
                return;
            }

            //On first load
            if(FirstLoad)
            {
                //Create a single self-unhookable event
                //for the elements loaded event
                RoutedEventHandler onLoaded = null;
                onLoaded = (ss, ee) =>
                {
                    //Unhook
                    element.Loaded -= onLoaded;

                    //Do the animation
                    DoAnimation(element, (bool)value);

                    //No longer first load
                    FirstLoad = false;
                };

                //Hook into the loaded event of the element
                element.Loaded += onLoaded;
            }
            else
            {
                //Animation
                DoAnimation(element, (bool)value);
            }
        }

        /// <summary>
        /// The animation method that is fired when the value changes
        /// </summary>
        /// <param name="element">the element</param>
        /// <param name="value">The new value</param>
        protected virtual void DoAnimation(FrameworkElement element, bool value) { }
    }

    public class AnimateSlideInFromLeftProperty : AnimateBaseProperty<AnimateSlideInFromLeftProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if(value)
            {
                // Animate in
                await element.SlideAndFadeInFromLeftAsync(FirstLoad ? 0 : 0.3f, keepMargin: false);
            }
            else
            {
                // Animate out
                await element.SlideAndFadeOutToLeftAsync(FirstLoad ? 0 : 0.3f, keepMargin: false);
            }
        }
    }
    public class AnimateSlideInFromTopProperty : AnimateBaseProperty<AnimateSlideInFromTopProperty>
    {
        protected override async void DoAnimation(FrameworkElement element, bool value)
        {
            if (value)
            {
                // Animate in
                await element.SlideAndFadeInFromTopAsync(FirstLoad ? 0 : 0.3f, keepMargin: false);
            }
            else
            {
                // Animate out
                await element.SlideAndFadeOutToBottomAsync(FirstLoad ? 0 : 0.3f, keepMargin: false);
            }
        }
    }
}
