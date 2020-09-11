using System;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts a bool to a loaded text
    /// </summary>
    public class BoolToLoadedText : BaseValueConverter<BoolToLoadedText>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            return (bool)value ? " --> LOADED" : string.Empty;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
