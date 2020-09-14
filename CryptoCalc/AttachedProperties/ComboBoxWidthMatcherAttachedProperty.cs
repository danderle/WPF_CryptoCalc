using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Matches the width of all the comboboxes inside this panel
    /// </summary>
    public class ComboBoxWidthMatcherProperty : BaseAttachedProperty<ComboBoxWidthMatcherProperty, bool>
    {
        #region Private fields

        /// <summary>
        /// The maximum width to set for all the comboboxes
        /// </summary>
        double maxWidth = 0;

        /// <summary>
        /// saves all the comboboxes found
        /// </summary>
        List<ComboBox> comboBoxes = new List<ComboBox>();

        #endregion

        /// <summary>
        /// Fires when the property is set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Get the panel (grid typically)
            var panel = (sender as Panel);

            // Hooks into the panel sizeChanged event
            panel.Loaded += Panel_Loaded;
        }

        /// <summary>
        /// The execute the panel sizeChanged event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_Loaded(object sender, RoutedEventArgs e)
        {
            CheckChildren((Panel)sender);

            //Cycle through all the comboboxes and set the maximum width
            foreach (var comboBox in comboBoxes)
            {
                comboBox.Width = maxWidth + 20;
            }
        }

        /// <summary>
        /// Checks thew children for stackpanels and comboboxes
        /// </summary>
        /// <param name="sender"></param>
        private void CheckChildren(Panel sender)
        {
            //Cycle through all comboboxes and get the maximum width
            foreach (UIElement child in sender.Children)
            {
                if (child is StackPanel stackPanel)
                {
                    CheckChildren(stackPanel);
                }

                if (child is ComboBox comboBox)
                {
                    //cycle through the combobox items
                    foreach (var item in comboBox.Items)
                    {
                        //Create a label for measuring the item text
                        Label label = new Label();

                        //set the combobox item as the label content 
                        label.Content = item.ToString();

                        //measure the text length
                        label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                        //arranges the uiElement size
                        label.Arrange(new Rect(label.DesiredSize));

                        //get the width of the label
                        double width = label.ActualWidth;

                        //save the maximum item width
                        maxWidth = width > maxWidth ? width : maxWidth;
                    }

                    comboBoxes.Add(comboBox);
                }
            }
        }
    }
}