using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using TweetSharp.Twitter.Model;

namespace Anhinga.Converters
{
    public class DoublesToGeoLocationConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value,
            Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string))
            {
                return null;
            }
            TwitterGeoLocation loc = (TwitterGeoLocation)value;
            if (loc != null && loc.Coordinates != null && loc.Coordinates.Latitude != 0 && loc.Coordinates.Longitude != 0)
                return loc.Coordinates.Latitude.ToString() + " " + loc.Coordinates.Longitude.ToString();
            return "";
        }

        object IValueConverter.ConvertBack(object value,
            Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(TwitterGeoLocation))
            {
                return null;
            }
            string str = (string)value;
            string[] tokens = str.Split(' ');
            if (tokens.GetLength(1) != 2)
                return null;

            double lat = Convert.ToDouble(tokens[0]);
            double lon = Convert.ToDouble(tokens[1]);

            return new TwitterGeoLocation(lat, lon);
        }

        #endregion
    }
}
