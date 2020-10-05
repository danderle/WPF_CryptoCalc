using CryptoCalc.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Media;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="DialogType"/>enum value to a Brush
    /// </summary>
    public class DialogTypeToFontAwesome: BaseValueConverter<DialogTypeToFontAwesome>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (WindowDialogType)value;
            string font = string.Empty;
            switch(type)
            {
                case WindowDialogType.FolderBrowser:
                    font = "\uf002";
                    break;
                case WindowDialogType.Warning:
                    font = "\uf071";
                    break;
                case WindowDialogType.Error:
                    font = "\uf057";
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            return font;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
