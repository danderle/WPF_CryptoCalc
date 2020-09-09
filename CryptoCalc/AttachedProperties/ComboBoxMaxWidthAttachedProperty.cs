using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Resizes the ComboBox to the largest item size plus the dropwdown arrow width
    /// </summary>
    /// <typeparam name="Parent"></typeparam>
    public class ComboBoxMaxWidthAttachedProperty : BaseAttachedProperty<ComboBoxMaxWidthAttachedProperty, double>
    {
        double max = 0;
        List<ComboBox> comboBoxes = new List<ComboBox>();

        #region Public Properties

        /// <summary>
        /// A flag indicating if this is the first time this property has been loaded
        /// </summary>
        public bool FirstLoad { get; set; } = true;

        #endregion

        /// <summary>
        /// Fires when value is set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueUpdated(DependencyObject sender, object value)
        {
            //make sure we have a comboBox
            if (!(sender is UserControl control))
            {
                return;
            }

            // Dont fire if the value doesnt changed
            if (sender.GetValue(ValueProperty) == value && !FirstLoad)
            {
                return;
            }

            //On first load
            if (FirstLoad)
            {
                //Create a single self-unhookable event
                //for the elements loaded event
                RoutedEventHandler OnLoaded = null;
                OnLoaded = (ss, ee) =>
                {
                    //Unhook
                    control.Loaded -= OnLoaded;

                    //No longer first load
                    FirstLoad = false;

                    LoadBoxes(control);
                };

                //Hook into the loaded event of the element
                control.Loaded += OnLoaded;
            }
            else
            {
                var newValue = (double)value;
                if (newValue > max)
                {
                    max = newValue;
                    foreach (var comboBox in comboBoxes)
                    {
                        comboBox.Width = max;
                    }
                    SetControlCustomProperty(control);
                }
            }
        }

        private void SetControlCustomProperty(UserControl control)
        {
            if (control is DataInputControl input)
            {
                input.ComboBoxMaxWidth = max;
            }
            else if (control is KeyPairSetupControl keyPair)
            {
                keyPair.ComboBoxMaxWidth = max;
            }
        }

        /// <summary>
        /// Fires when the control is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadBoxes(UserControl sender)
        {
            if (sender is UserControl control)
            {
                string name = "ComboBox";
                int number = 1;
                ComboBox comboBox = (ComboBox)control.FindName($"{name}{number}");

                while(comboBox != null)
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
                        double width = label.ActualWidth + 20;

                        //save the maximum item width
                        max = width > max ? width : max;

                    }
                    //TODO figure out how to get the size of dropdown arrow or size when empty
                    comboBox.Width = max;

                    comboBoxes.Add(comboBox);

                    number++;

                    comboBox = (ComboBox)control.FindName($"{name}{number}");
                }
                SetControlCustomProperty(control);
            }
        }
    }
}
