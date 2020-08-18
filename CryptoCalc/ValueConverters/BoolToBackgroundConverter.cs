using System;
using System.Globalization;
using System.Windows.Media;

namespace CryptoCalc
{
    /// <summary>
    /// Converts a bool to a background color
    /// </summary>
    public class BoolToBackground : BaseValueConverter<BoolToBackground>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            return (bool)value ? new SolidColorBrush(Color.FromRgb(0x9A, 0xFF, 0x9A)) : new SolidColorBrush(Color.FromRgb(0xFA, 0x80, 0x72));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
