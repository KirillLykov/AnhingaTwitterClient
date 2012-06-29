using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Anhinga.Converters
{
    public class LengthToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value,
            Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return null;
            }

            return 140 - ((int)value);
        }

        object IValueConverter.ConvertBack(object value,
            Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
