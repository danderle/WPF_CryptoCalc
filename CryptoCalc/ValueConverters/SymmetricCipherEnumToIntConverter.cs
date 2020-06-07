using CryptoCalc.Core;
using System;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="SymmetricCipherAlgorithim"/>enum value to an int value
    /// </summary>
    public class SymmetricCipherEnumToInt : BaseValueConverter<SymmetricCipherEnumToInt>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)(SymmetricCipherAlgorithim)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SymmetricCipherAlgorithim)value;
        }
    }
}
