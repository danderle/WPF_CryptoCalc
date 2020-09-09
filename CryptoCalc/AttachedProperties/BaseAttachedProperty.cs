using System;
using System.Windows;

namespace CryptoCalc
{
    /// <summary>
    /// A base attached property to replace the repetative WPF attached property
    /// </summary>
    /// <typeparam name="Parent">The parent class to be the attached property</typeparam>
    /// <typeparam name="Property">The type of this attached property</typeparam>
    public abstract class BaseAttachedProperty<Parent, Property>
        where Parent : BaseAttachedProperty<Parent, Property>, new()
    {
        #region Public Events

        /// <summary>
        /// Fired when the value is changed
        /// </summary>
        public event Action<DependencyObject, DependencyPropertyChangedEventArgs> ValueChanged = (sender, evnt) => { };

        /// <summary>
        /// Fired when the value is changes, even if the value is the same
        /// </summary>
        public event Action<DependencyObject, object> ValueUpdated = (sender, evnt) => { };
        
        #endregion

        #region Public Properties

        /// <summary>
        /// A singleton instance of our parent clas
        /// </summary>
        public static Parent Instance { get; private set; } = new Parent();

        #endregion

        #region Attched Property Definition

        /// <summary>
        /// The attached property for this class
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached(
            "Value",
            typeof(Property),
            typeof(BaseAttachedProperty<Parent, Property>),
            new UIPropertyMetadata(
                default(Property),
                new PropertyChangedCallback(OnValuePropertyChanged),
                new CoerceValueCallback(OnValuePropertyUpdated)
                ));

        /// <summary>
        /// The callback event when the <see cref="ValueProperty"/> is changed even if it is the same
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static object OnValuePropertyUpdated(DependencyObject d, object value)
        {
            //Call the parent function
            Instance.OnValueUpdated(d, value);

            //Call the event listerners
            Instance.ValueUpdated(d, value);

            return value;
        }

        /// <summary>
        /// Thew callback event when the <see cref="ValueProperty"/> is changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Call the parent function
            Instance.OnValueChanged(d, e);

            //Call event listeners
            Instance.ValueChanged(d, e);
        }


        /// <summary>
        /// Gets the attached property
        /// </summary>
        /// <param name="d">The element to get the property from</param>
        /// <returns></returns>
        public static Property GetValue(DependencyObject d) => (Property)d.GetValue(ValueProperty);

        /// <summary>
        /// Sets the attached property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetValue(DependencyObject d, Property value) => d.SetValue(ValueProperty, value);

        #endregion

        #region Event Methods

        /// <summary>
        /// The method that is called when any attached property of this type is changed
        /// </summary>
        /// <param name="sender">The UI element that this property was changed for</param>
        /// <param name="evnt">The arguments for this event</param>
        public virtual void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs evnt) { }

        /// <summary>
        /// The method that is called when any attached property of this type is changed
        /// </summary>
        /// <param name="sender">The UI element that this property was changed for</param>
        /// <param name="value">the value which has changed</param>
        public virtual void OnValueUpdated(DependencyObject sender, object value) { }


        #endregion
    }
}
