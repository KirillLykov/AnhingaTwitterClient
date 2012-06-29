using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Anhinga.Converters
{
    public class NullableBoolToBool : IValueConverter
    {
        #region IValueConverter Members
        //TODO Perhaps embeded converter exists? 
        object IValueConverter.Convert(object value,
            Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool?))
            {
                return null;
            }

            bool? dateTime = value as bool?;
            if (dateTime == null || dateTime == false)
                return false;

            return true;
        }

        object IValueConverter.ConvertBack(object value,
            Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool?))
            {
                return null;
            }

            return value;
        }

        #endregion
    }
}
