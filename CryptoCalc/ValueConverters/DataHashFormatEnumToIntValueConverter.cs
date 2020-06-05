using CryptoCalc.Core;
using System;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="Format"/>enum value to an int value
    /// </summary>
    public class DataHashFormatEnumToInt : BaseValueConverter<DataHashFormatEnumToInt>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)(Format)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Format)value;
        }
    }
}
