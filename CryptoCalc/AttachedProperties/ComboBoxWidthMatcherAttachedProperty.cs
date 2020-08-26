using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Matches the width of all the comboboxes inside this panel
    /// </summary>
    public class ComboBoxWidthMatcherProperty : BaseAttachedProperty<ComboBoxWidthMatcherProperty, bool>
    {
        //The maximum width to set for all the comboboxes
        double maxWidth = 0;

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
            panel.SizeChanged += Panel_SizeChanged;
        }

        /// <summary>
        /// The execute the panel sizeChanged event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Cycle through all comboboxes and get the maximum width
            foreach (UIElement child in ((Panel)sender).Children)
            {
                //cast to the Hash item control
                var comboBox = (child as ComboBox);

                // Ignore any non ComboBoxes
                if (!(comboBox is ComboBox))
                    continue;

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

                //set the maximum dropdown width on combobox
                //TODO figure out how to get the size of dropdown arrow or size when empty
                comboBox.Width = max + 20;

                // save the maximum combobox width
                maxWidth = comboBox.Width > maxWidth ? comboBox.Width : maxWidth;
            }

            //Cycle through all the comboboxes and set the maximum width
            foreach (UIElement child in ((Panel)sender).Children)
            {
                //cast to the Hash item control
                var comboBox = (child as ComboBox);

                // Ignore any non ComboBoxes
                if (!(comboBox is ComboBox))
                    continue;

                // Set the max width on all comboboxes
                comboBox.Width = maxWidth;
            }
        }

    }
}