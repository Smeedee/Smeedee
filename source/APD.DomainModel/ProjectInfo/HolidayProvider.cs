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
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Jan Fredrik Drabløs</author>
// <email>jaffe88@hotmail.com</email>
// <date>2009-08-13</date>

#endregion

using System;
using System.Collections.Generic;

namespace APD.DomainModel.ProjectInfo
{
    public class HolidayProvider
    {
        public List<DateTime> ExplicitWorkingDays;
        public List<DateTime> Holidays;

        public HolidayProvider()
        {
            Holidays = new List<DateTime>();
            ExplicitWorkingDays = new List<DateTime>();
        }


        public bool IsHoliday(DateTime sprintDay)
        {
            bool isExplicitWorkingDay = ExplicitWorkingDays.Contains(sprintDay.Date);
            if (isExplicitWorkingDay)
                return false;

            bool isWeekend = IsWeekend(sprintDay);
            if (isWeekend)
                return true;

            return Holidays.Contains(sprintDay.Date);
        }

        public void MakeHolidayWorkingDay(DateTime sprintDay)
        {
            if(!ExplicitWorkingDays.Contains(sprintDay.Date))
                ExplicitWorkingDays.Add(sprintDay.Date);
        }

        public void MakeWorkingdayHoliday(DateTime sprintDay)
        {
            ExplicitWorkingDays.Remove(sprintDay.Date);
            if(!IsHoliday(sprintDay))
                Holidays.Add(sprintDay.Date);
        }

        private static bool IsWeekend(DateTime sprintDay)
        {
            return sprintDay.DayOfWeek == DayOfWeek.Saturday || sprintDay.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}