#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://www.smeedee.org
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Smeedee.Client.Framework.Converters
{
    public class TimeSpanToPrettyStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && targetType == typeof(string))
            {
                TimeSpan parsedTimeSpan = TimeSpan.Parse(value as string);
                value = parsedTimeSpan;
            }

            if (value is TimeSpan && targetType == typeof(string))
            {
                var timeObject = (TimeSpan)value;
                string resultString = ConvertTimeSpanToString(timeObject);
                return resultString;
            }
            throw new NotImplementedException();
        }

        private string ConvertTimeSpanToString(TimeSpan span)
        {
            if (span.Ticks == 0)
            {
                return string.Empty;
            }

            string result = string.Empty;

            if (span.Ticks < 0)
            {
                span = new TimeSpan(Math.Abs(span.Ticks));
                result += "minus ";
            }
            var hours = (int)span.TotalHours;
            int minutes = span.Minutes;
            int seconds = span.Seconds;


            if (hours > 0)
            {
                result += FormatNumberWithPlural(hours, "hour");
                if (minutes > 0)
                {
                    result += " and " + FormatNumberWithPlural(minutes, "minute");
                }
            }
            else if (minutes > 0)
            {
                result += FormatNumberWithPlural(minutes, "minute");

                if (seconds > 0)
                {
                    result += " and " + FormatNumberWithPlural(seconds, "second");
                }
            }
            else
            {
                result += FormatNumberWithPlural(seconds, "second");
            }

            return result;
        }


        private string FormatNumberWithPlural(int value, string singularForm)
        {
            return value + " "+ singularForm + (value != 1 ? "s" : "");
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }



    }
}
