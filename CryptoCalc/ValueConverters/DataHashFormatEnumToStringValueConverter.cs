using CryptoCalc.Core;
using System;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="DataHashFormat"/>enum value to a string value
    /// </summary>
    public class DataHashFormatEnumToString : BaseValueConverter<DataHashFormatEnumToString>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)(DataHashFormat)value).ToString();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DataHashFormat)Enum.Parse(typeof(DataHashFormat), value.ToString());
        }
    }
}
