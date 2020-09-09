using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Matches the width of all the comboboxes inside this panel
    /// </summary>
    public class ControlComboBoxWidthMatcherProperty : BaseAttachedProperty<ControlComboBoxWidthMatcherProperty, bool>
    {
        //The maximum width to set for all the comboboxes
        double maxWidth = 0;
        List<ComboBox> comboBoxes = new List<ComboBox>();

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
            var panel = (Panel)sender;
            //Cycle through all comboboxes and get the maximum width
            foreach (UIElement child in panel.Children)
            {
                if (child is DataInputControl inputControl)
                {
                    foreach(var controlChild in inputControl.StackPanelWithComboBox.Children)
                    {
                        if(controlChild is ComboBox comboBox)
                        {
                            //set combobox width and max
                            SetCombBoxWidthAndSaveMax(comboBox);
                        }
                    }
                }
                else if(child is KeyPairSetupControl keyPairSetupControl)
                {
                    foreach (var controlChild in keyPairSetupControl.StackPanelWithComboBox.Children)
                    {
                        if (controlChild is ComboBox comboBox)
                        {
                            //set combobox width and max
                            SetCombBoxWidthAndSaveMax(comboBox);
                        }
                        else if(controlChild is StackPanel stackPanel)
                        {
                            foreach(var stackPanelChild in stackPanel.Children)
                            {
                                if (stackPanelChild is ComboBox combobox)
                                {
                                    //set combobox width and max
                                    SetCombBoxWidthAndSaveMax(combobox);
                                }
                            }
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            //cycle through all the saved comboboxes and set the max width
            foreach(var comboBox in comboBoxes)
            {
                comboBox.Width = maxWidth;
            }
        }

        /// <summary>
        /// Set the max width of the items inside the combobox and save the max width of the combobox
        /// </summary>
        /// <param name="comboBox"></param>
        private void SetCombBoxWidthAndSaveMax(ComboBox comboBox)
        {
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

            //save the maximum combobox width
            maxWidth = comboBox.Width > maxWidth ? comboBox.Width : maxWidth;

            //add the combobox to the list
            comboBoxes.Add(comboBox);
        }
    }
}