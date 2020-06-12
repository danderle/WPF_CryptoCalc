using CryptoCalc.Core;
using System;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="CryptographyApi"/>enum value to an int value
    /// </summary>
    public class CryptographyApiEnumToInt : BaseValueConverter<CryptographyApiEnumToInt>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)(CryptographyApi)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (CryptographyApi)value;
        }
    }
}
