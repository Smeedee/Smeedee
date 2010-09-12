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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://www.smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.Holidays;
using Smeedee.DomainModel.ProjectInfo;

namespace Smeedee.Widget.ProjectInfo.ViewModels
{
    public partial class WorkingDaysLeftViewModel
    {
        public bool IsOnOvertime 
        { 
            get
            {
                return DateTime.Now.Date > EndDate;
            } 
        }
        public bool IsLoading { get; set; }

        public void OnInitialize()
        {
            _EndDate = DateTime.Now.Date.AddMonths(1);
            Holidays = new List<Holiday>();
            DaysRemaining = string.Empty;
            HasInformationToShow = false;

        }
        
        private string daysRemaining;
        public string DaysRemaining
        {
            get { return daysRemaining; }
            set
            {
                if (value != daysRemaining)
                {
                    daysRemaining = value;
                    TriggerPropertyChanged("DaysRemainingTextSmall");
                    TriggerPropertyChanged("DaysRemainingTextLarge");
                    TriggerPropertyChanged("DaysRemaining");
                }
            }
        }

        public void UpdateDaysRemaining(IEnumerable<DayOfWeek> nonWorkingDays)
        {
            var today = DateTime.Now.Date;
            int calculatedDaysRemaning = new Iteration(today, EndDate).CalculateWorkingdaysLeft(today, Holidays, nonWorkingDays).Days;
            DaysRemaining = calculatedDaysRemaning.ToString();
        }
        
        public string DaysRemainingTextLarge
        {
            get
            {
                if( IsLoading )
                {
                    return MessageStrings.LOADING_DATA;
                }

                if (!HasInformationToShow)
                {
                    return MessageStrings.NO_INFORMATION_STRING_LARGE;
                }

                if (DaysRemaining.Equals("1"))
                {
                    return IsOnOvertime
                               ? MessageStrings.DAYS_ON_OVERTIME_SINGULAR_STRING
                               : MessageStrings.WORKING_DAYS_LEFT_SINGULAR_STRING;
                }
                else
                {
                    return IsOnOvertime
                               ? MessageStrings.DAYS_ON_OVERTIME_STRING
                               : MessageStrings.WORKING_DAYS_LEFT_STRING;
                }
            }
        }

        public string DaysRemainingTextSmall
        {
            get
            {
                if( IsLoading )
                {
                    return MessageStrings.LOADING_DATA;
                }

                if (!HasInformationToShow)
                {
                    return MessageStrings.NO_INFORMATION_STRING;
                }
                if (IsOnOvertime)
                {
                    return MessageStrings.DAYS_ON_OVERTIME_STRING;
                }
                return MessageStrings.DAYS_LEFT_STRING;
            }
        }
    }

    public class MessageStrings
    {
        public const string LOADING_DATA = "Loading...";
        public const string NO_INFORMATION_STRING_LARGE = "No end date set. Please check the widget settings.";
        public const string NO_INFORMATION_STRING = "No info/config";
        public const string WORKING_DAYS_LEFT_STRING = " working days left";
        public const string WORKING_DAYS_LEFT_SINGULAR_STRING = " working day left";
        public const string DAYS_ON_OVERTIME_STRING = " days on overtime";
        public const string DAYS_ON_OVERTIME_SINGULAR_STRING = " day on overtime";
        public const string DAYS_LEFT_STRING = "Days left: ";
        public const string CONNECTION_ERROR = "Problems with loading the data";
    }
}