using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace FeedReed
{
    public class RssTextTrimmer : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            int maxLength = 200;
            int strLength = 0;
            string fixedString = "";

            //Remove HTML tags and what not;

            //Removes HTML tags
            fixedString = Regex.Replace(value.ToString(), "<[^>]+>", string.Empty);

            //Remove new lines
            fixedString = fixedString.Replace("\r", "").Replace("\n", "");

            //Remove encoded characters
            fixedString = HttpUtility.HtmlDecode(fixedString);

            strLength = fixedString.ToString().Length;

            if (strLength == 0)
            {
                return null;
            }

            else if (strLength >= maxLength)
            {
                fixedString = fixedString.Substring(0, maxLength);
                fixedString = fixedString.Substring(0, fixedString.LastIndexOf(" "));
            }
            fixedString += "...";

            return fixedString;
        }

        // This code sample does not use TwoWay binding, so we do not need to flesh out ConvertBack.  
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
