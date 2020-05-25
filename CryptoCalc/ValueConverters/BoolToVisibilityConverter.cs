using System;
using System.Globalization;
using System.Windows;

namespace CryptoCalc
{
    /// <summary>
    /// Converts a bool to a visibiliy
    /// </summary>
    public class BoolToVisibility : BaseValueConverter<BoolToVisibility>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            bool retVal = (bool)value;
            if(parameter != null)
            {
                retVal &= (bool)parameter;
            }
            return retVal ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
