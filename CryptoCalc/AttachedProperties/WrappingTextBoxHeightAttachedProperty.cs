using System.Windows;
using System.Windows.Controls;

namespace CryptoCalc
{
    /// <summary>
    /// Adjusts the height of a textbox when wrapping until the set maximum lines
    /// </summary>
    public class WrappingTextBoxHeightProperty : BaseAttachedProperty<WrappingTextBoxHeightProperty, int>
    {
        #region Private Fields

        bool originalHeightSet = false;

        int maxLines = 0;

        double originalHeight = 0;

        double totalPadding = 0;

        #endregion

        #region Overriden OnValueChange method

        /// <summary>
        /// Called when the value of the attached property is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //make sure the object is a textbox
            if (sender is TextBox textBox)
            {
                //save the maximum number of lines
                maxLines = (int)e.NewValue;
                //set text wrapping
                textBox.TextWrapping = TextWrapping.Wrap;
                //hook into the text changed event
                textBox.TextChanged += TextBox_TextChanged;
            }
        }

        #endregion

        #region Private Method

        /// <summary>
        /// Gets the textbox height according to the number of lines
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private double GetTextBoxHeight(TextBox tb, int lines)
        {
            return (tb.FontSize * lines) + totalPadding;
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// The TextChanged event method, called everytime the text inside a textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //get the textbox
            var tb = (TextBox)sender;
            //get the lines in textbox
            int lines = tb.LineCount;
            //set verticasl scrollbar to hidden
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

            //Get the original set height on the first text changed event
            if (!originalHeightSet)
            {
                originalHeight = tb.Height;
                totalPadding = originalHeight - tb.FontSize;
                originalHeightSet = true;
            }

            if (lines == 1)
            {
                tb.Height = originalHeight;
            }
            else if (lines >= 2 && lines <= maxLines)
            {
                double height = GetTextBoxHeight(tb, lines);
                tb.Height = height;
            }
            else if (lines > maxLines)
            {
                double height = GetTextBoxHeight(tb, maxLines);
                tb.Height = height;
                tb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
        } 
        #endregion
    }
}