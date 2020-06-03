using CryptoCalc.Core;
using System;
using System.Diagnostics;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts the <see cref="TreeItemType"/>enum value to an image path
    /// </summary>
    public class TreeItemTypeToImagePath : BaseValueConverter<TreeItemTypeToImagePath>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string imagePath = "\\Resources\\Images\\";
            switch ((TreeItemType)value)
            {
                case TreeItemType.LogicalDrive:
                    imagePath += "drive.png";
                    break;
                case TreeItemType.Directory:
                    imagePath += "folder.png";
                    break;
                case TreeItemType.File:
                    imagePath += "file.png";
                    break;
                default:
                    //Some type of error
                    Debugger.Break();
                    break;
            }
            return imagePath;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (DataHashFormat)value;
        }
    }
}
