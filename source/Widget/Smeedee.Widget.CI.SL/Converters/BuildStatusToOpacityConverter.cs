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

#endregion

using System;
using System.Globalization;
using System.Windows.Data;

namespace Smeedee.Widget.CI.SL.Converters
{
    public class BuildStatusToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var lowOpacity = 0.2;
            var highOpacity = 1.0;

            
            if (!(value is BuildStatus))
            {
                return lowOpacity;
            }

            var buildValue = (BuildStatus)value;

            if (parameter.ToString().Equals("Red") && buildValue == BuildStatus.Failed)
            {
                return highOpacity;
            }
            if (parameter.ToString().Equals("Yellow") && buildValue == BuildStatus.Building)
            {
                return highOpacity;
            }

            if (parameter.ToString().Equals("Green") && buildValue == BuildStatus.Successful)
            {
                return highOpacity;
            }

            if (parameter.ToString().Equals("Multi")&& buildValue != BuildStatus.Unknown)
            {
                return highOpacity;
            }

            return lowOpacity;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
    }
}
