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
    public class DialogTypeToBrush : BaseValueConverter<DialogTypeToBrush>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (WindowDialogType)value;
            var brush = new SolidColorBrush();
            switch(type)
            {
                case WindowDialogType.FolderBrowser:
                    brush.Color = Color.FromRgb(0x4d, 0x6e, 0x81);
                    break;
                case WindowDialogType.Warning:
                    brush.Color = Color.FromRgb(0xff, 0x80, 0x00);
                    break;
                case WindowDialogType.Error:
                    brush.Color = Color.FromRgb(0xff, 0x00, 0x00);
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            return brush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
