using CryptoCalc.Core;
using System;
using System.Diagnostics;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts a <see cref="Format"/> to a tool tip
    /// </summary>
    public class DataFormatsToDecryptToolTip : BaseValueConverter<DataFormatsToDecryptToolTip>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            switch((Format)value)
            {
                case Format.Text:
                    return "Enter an encrypted text to decrypt";
                case Format.Hex:
                    return "Enter an encrypted hexadecimal value to decrypt";
                case Format.File:
                    return "Enter a file path of encrypted file to decrypt";
                default:
                    Debugger.Break();
                    return null;
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
