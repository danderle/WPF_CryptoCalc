using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoCalc
{
    /// <summary>
    /// Calls a command method when the selection of a combobox is changed
    /// </summary>
    public class SelectionChangedProperty : BaseAttachedProperty<SelectionChangedProperty, ICommand>
    {
        /// <summary>
        /// Fires when a new command is set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.SelectionChanged += (ss, ee) =>
            {
                ICommand command = GetValue(comboBox);

                command.Execute(e);
            };
        }
    }
}
