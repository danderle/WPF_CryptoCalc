using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CryptoCalc
{
    /// <summary>
    /// Match the item width of all hash item controls inside this panel
    /// </summary>
    public class CheckBoxWidthMatcherProperty : BaseAttachedProperty<CheckBoxWidthMatcherProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Get the panel (grid typically)
            var panel = (sender as Panel);

            //Set widths only in design time
            if(DesignerProperties.GetIsInDesignMode(panel))
                SetWidths(panel);

            // Wait for panel to load
            RoutedEventHandler onLoaded = null;
            onLoaded = (s, ee) =>
            {
                // Unhook
                panel.Loaded -= onLoaded;

                // Set widths
                SetWidths(panel);

                // Loop each child
                foreach (ContentPresenter child in panel.Children)
                {
                    //Get the visual control of the content presenter
                    var someControl = VisualTreeHelper.GetChild(child, 0);

                    //cast to the Hash item control
                    var hashItemControl = (someControl as HashItemControl);

                    // Ignore any non Hash item controls
                    if (!(hashItemControl is HashItemControl))
                        continue;

                    // Get the checkbox item from the hash item control
                    var checkbox = hashItemControl.CheckBoxItem;

                    checkbox.SizeChanged += (ss, eee) =>
                    {
                        // Update widths
                        SetWidths(panel);
                    };
                }
            };

            // Hook into the Loaded event
            panel.Loaded += onLoaded;
        }

        /// <summary>
        /// Update all child hash item controls so their widths match the largest width of the group
        /// </summary>
        /// <param name="panel">The panel containing the hash item controls</param>
        private void SetWidths(Panel panel)
        {
            // Keep track of the maximum width
            var maxSize = 0d;

            // For each child...
            foreach (ContentPresenter child in panel.Children)
            {
                //Get the visual control of the content presenter
                var someControl = VisualTreeHelper.GetChild(child, 0);

                //cast to the Hash item control
                var hashItemControl = (someControl as HashItemControl);

                // Ignore any non Hash item controls
                if (!(hashItemControl is HashItemControl))
                    continue;

                // Get the label from the text entry or password entry
                var label = hashItemControl.CheckBoxItem;

                // Find if this value is larger than the other controls
                maxSize = Math.Max(maxSize, label.RenderSize.Width + label.Margin.Left + label.Margin.Right);
            }

            // Create a grid length converter
            var gridLength = (GridLength)new GridLengthConverter().ConvertFromString(maxSize.ToString());

            // For each child...
            foreach (ContentPresenter child in panel.Children)
            {
                //Get the visual control of the content presenter
                var someControl = VisualTreeHelper.GetChild(child, 0);

                //cast to the Hash item control
                var hashItemControl = (someControl as HashItemControl);

                // Ignore any non Hash item controls
                if (hashItemControl is HashItemControl)
                    // Set each controls CheckBoxWidth value to the max size
                    hashItemControl.CheckBoxWidth = gridLength;
            }

        }
    }
}