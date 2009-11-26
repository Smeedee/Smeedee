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
using System.Linq;

namespace APD.DomainModel.ProjectInfo
{
    public class Iteration
    {
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual HolidayProvider HolidayProvider { get; set; }
        public virtual string Name { get; set;}
        public virtual string SystemId { get; set; }
        public virtual Project Project { get; set; }

        private IList<Task> internalTasks;
        public virtual IEnumerable<Task> Tasks
        {
            get
            {
                return internalTasks;
            }   
            set
            {
                internalTasks = value.ToList();
                foreach (Task task in value)
                {
                    task.Iteration = this;
                }
            }
        }

        public Iteration()
            : this(DateTime.Now, DateTime.Now.AddDays(14), new HolidayProvider()) { }
        

        public Iteration(DateTime startDate, DateTime endDate, HolidayProvider holidayProvider)
        {
            StartDate = startDate;
            EndDate = endDate;
            HolidayProvider = holidayProvider;
            Tasks = new List<Task>();
            internalTasks = new List<Task>();
        }


        public virtual void AddTask(Task newTask)
        {
            newTask.Iteration = this;
            internalTasks.Add(newTask);
            Tasks = internalTasks;
        }

        public virtual int GetTaskCount()
        {
            return internalTasks.Count;
        }

        public virtual List<DateTime> GetWorkingDays()
        {
            List<DateTime> workingDays = new List<DateTime>();

            for (var sprintDay = StartDate; sprintDay < EndDate; sprintDay = sprintDay.AddDays(1))
            {
                if (IsWorkingDay(sprintDay))
                {
                    workingDays.Add(sprintDay);
                }
            }

            return workingDays;
        }

        public virtual void SetExtraWorkingDay(DateTime extraDay)
        {
            HolidayProvider.MakeHolidayWorkingDay(extraDay);
        }

        public virtual void SetExtraHoliday(DateTime nonWorkingDay)
        {
            HolidayProvider.MakeWorkingdayHoliday(nonWorkingDay);
        }

        public virtual TimeSpan CalculateWorkingdaysLeft(DateTime dayToCalculateFrom)
        {
            if (IsOvertime(dayToCalculateFrom))
                return dayToCalculateFrom.Subtract(EndDate);

            int workingDaysLeft = 0;
            for (var sprintDay = dayToCalculateFrom; sprintDay <= EndDate; sprintDay = sprintDay.AddDays(1))
            {
                if (IsWorkingDay(sprintDay))
                {
                    workingDaysLeft++;
                }
            }

            return new TimeSpan(workingDaysLeft, 0,0,0);
        }

        private bool IsWorkingDay(DateTime sprintDay)
        {
            return !HolidayProvider.IsHoliday(sprintDay);
        }

        public virtual bool IsOvertime(DateTime dayToCalculateFrom)
        {
            return DateTime.Compare(dayToCalculateFrom.Date, EndDate.Date) > 0;
        }

        public virtual float GetWorkEffortEstimateForTasks()
        {
            return Tasks.Sum(w => w.WorkEffortEstimate);
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}", SystemId, (Project != null) ? Project.SystemId : string.Empty).GetHashCode();

        }
    }
}