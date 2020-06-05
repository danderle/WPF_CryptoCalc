using CryptoCalc.Core;
using System;
using System.Globalization;
using System.Windows;

namespace CryptoCalc
{
    /// <summary>
    /// Converts a <see cref="Format"/> to a visibiliy
    /// </summary>
    public class DataFormatsToVisibility : BaseValueConverter<DataFormatsToVisibility>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            var retVal = ((Core.Format)value).ToString() == (string)parameter;
            return retVal ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
