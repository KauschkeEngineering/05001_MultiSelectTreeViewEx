// *********************************************************************************************************
// *
// *  Author:       Maximilian Burkhardt
// *  Disclaimer:   Copyright (c) 2024 - Kauschke Engineering Systems GmbH
// * 				        All rights reserved - Unauthorized deobfuscation and making copies of this file,
// * 				        via any medium is strictly prohibited.
// *
// *********************************************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace System.Windows.Controls.Core
{
    public class DummyConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.WriteLine("DummyConverter->Convert: " + value);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
