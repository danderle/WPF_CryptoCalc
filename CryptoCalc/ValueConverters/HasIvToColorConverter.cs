using System;
using System.Globalization;
using System.Windows.Media;

namespace CryptoCalc
{
    /// <summary>
    /// Converts a bool to a color
    /// </summary>
    public class HasIvToColor : BaseValueConverter<HasIvToColor>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            return (bool)value ? new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0xff)) : new SolidColorBrush(Color.FromRgb(0xee, 0xee, 0xee));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
