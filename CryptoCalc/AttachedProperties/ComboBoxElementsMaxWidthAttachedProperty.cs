using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Resizes the ComboBox to the largest item size plus the dropwdown arrow width
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public class ComboBoxElementsMaxWidthProperty : BaseAttachedProperty<ComboBoxElementsMaxWidthProperty, bool>
    {
        /// <summary>
        /// Fires when value is set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //make sure we have a comboBox
            if (!(sender is ComboBox comboBox))
            {
                return;
            }

            //Adjust size when loaded
            comboBox.Loaded += ComboBox_Loaded;
        }

        /// <summary>
        /// Fires when the comboBox is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            //verify that we have a combobox
            if (!(sender is ComboBox comboBox))
            {
                return;
            }

            // Ignore any non ComboBoxes
            if (!(comboBox is ComboBox))
                return;

            //the maximum length of the items inside the combobox
            double max = 0;

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
                max = width > max ? width : max;
            }

            //TODO figure out how to get the size of dropdown arrow or size when empty
            comboBox.Width = max + 20;
        }
    }
}
