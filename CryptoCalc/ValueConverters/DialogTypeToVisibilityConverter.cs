using CryptoCalc.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="DialogType"/>enum value to viusibility
    /// </summary>
    public class DialogTypeToVisibility: BaseValueConverter<DialogTypeToVisibility>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (WindowDialogType)value;
            var visbility = Visibility.Visible;
            switch(type)
            {
                case WindowDialogType.FolderBrowser:
                    return Visibility.Visible;
                case WindowDialogType.Warning:
                case WindowDialogType.Error:
                    return Visibility.Collapsed;
                default:
                    Debugger.Break();
                    break;
            }
            return visbility;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
