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

        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ComboBox control))
            {
                return;
            }

            //Adjust size when loaded
            control.Loaded += Control_Loaded;
        }


        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is ComboBox control))
            {
                return;
            }
            double max = 0;
            foreach (var item in control.Items)
            {
                Label label = new Label();
                label.Content = item.ToString();
                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                label.Arrange(new Rect(label.DesiredSize));
                double width = label.ActualWidth;
                max = width > max ? width : max;
            }

            //TODO figure out how to get the size of dropdown arrow or size when empty
            control.Width = max + 20;
        }
    }
}
