﻿using CryptoCalc.Core;
using System;
using System.Diagnostics;
using System.Globalization;

namespace CryptoCalc
{
    /// <summary>
    /// Converts a int index <see cref="Format"/> to a tool tip
    /// </summary>
    public class IndexToToolTip : BaseValueConverter<IndexToToolTip>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            int index = (int)value >= 0 ? (int)value : 0;
            switch((Format)index)
            {
                case Format.TextString:
                    return "Enter a text to be hashed";
                case Format.HexString:
                    return "Enter a hexadecimal value to be hashed";
                case Format.File:
                    return "Enter a file path, to hash a file";
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