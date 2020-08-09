using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Interaction logic for HashItemControl.xaml
    /// </summary>
    public partial class HashItemControl : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// The width of the entire checkbox control
        /// </summary>
        public GridLength CheckBoxWidth
        {
            get => (GridLength)GetValue(CheckBoxWidthProperty);
            set => SetValue(CheckBoxWidthProperty, value);
        }

        /// <summary>
        /// Registers <see cref="CheckBoxWidth"/> as a dependancy property
        /// </summary>
        public static readonly DependencyProperty CheckBoxWidthProperty =
            DependencyProperty.Register(nameof(CheckBoxWidth), typeof(GridLength), typeof(HashItemControl), new PropertyMetadata(GridLength.Auto, CheckBoxWidthChangedCallback));


        /// <summary>
        /// Called when the checkbox width has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CheckBoxWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                // Set the column definition width to the new value
                (d as HashItemControl).CheckBoxColumn.Width = (GridLength)e.NewValue;
            }

            // Making ex available for developer on break
#pragma warning disable CS0168
            catch (Exception ex)
#pragma warning restore CS0168
            {
                // Make developer aware of potential issue
                Debugger.Break();

                (d as HashItemControl).CheckBoxColumn.Width = GridLength.Auto;
            }
        }

        #endregion


        #region Constructor

        public HashItemControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
