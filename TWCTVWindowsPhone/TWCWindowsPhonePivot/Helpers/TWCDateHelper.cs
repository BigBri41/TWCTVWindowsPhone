using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TWCWindowsPhonePivot.Helpers
{
    public class TWCDateHelper
    {
        static string dateFormat = "yyyy-MM-ddTHH:MM:ss.fffZ";

        public static string ConvertToTWCDate(DateTime convertDate)
        {
            string returnString = "";

            returnString = convertDate.ToString(dateFormat);

            return returnString;
        }


        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}
