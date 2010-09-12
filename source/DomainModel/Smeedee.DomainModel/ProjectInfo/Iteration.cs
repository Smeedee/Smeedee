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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Jan Fredrik Drabløs</author>
// <email>jaffe88@hotmail.com</email>
// <date>2009-08-13</date>

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Smeedee.DomainModel.Holidays;


namespace Smeedee.DomainModel.ProjectInfo
{
    public class Iteration
    {
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
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
            : this(DateTime.Now, DateTime.Now.AddDays(14)) { }
        

        public Iteration(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
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

        public virtual List<DateTime> GetWorkingDays(IEnumerable<Holiday> holidays)
        {
            List<DateTime> workingDays = new List<DateTime>();

            for (var sprintDay = StartDate; sprintDay < EndDate; sprintDay = sprintDay.AddDays(1))
            {
                if (!holidays.Any( hd => hd.Date == sprintDay.Date))
                {
                    workingDays.Add(sprintDay);
                }
            }

            return workingDays;
        }

        public virtual TimeSpan CalculateWorkingdaysLeft(DateTime dayToCalculateFrom, IEnumerable<Holiday> holidays)
        {
            if (IsOvertime(dayToCalculateFrom))
                return dayToCalculateFrom.Subtract(EndDate);

            int workingDaysLeft = 0;
            for (var sprintDay = dayToCalculateFrom; sprintDay <= EndDate; sprintDay = sprintDay.AddDays(1))
            {
                if (!holidays.Any(hd => hd.Date == sprintDay.Date))
                {
                    workingDaysLeft++;
                }
            }

            return new TimeSpan(workingDaysLeft, 0,0,0);
        }

        public virtual TimeSpan CalculateWorkingdaysLeft(DateTime dayToCalculateFrom, 
                                                         IEnumerable<Holiday> holidays,
                                                         IEnumerable<DayOfWeek> nonWorkingDays)
        {
            return CalculateWorkingdaysLeft(dayToCalculateFrom, Merge(holidays, nonWorkingDays));
        }

        private List<Holiday> Merge(IEnumerable<Holiday> holidays, IEnumerable<DayOfWeek> nonWorkingDays)
        {
            var holidaysDateSet = new HashSet<DateTime>(holidays.Select(holiday => holiday.Date));
            var holidaysCopy = new List<Holiday>(holidays);
            for (var day = StartDate; day <= EndDate; day = day.AddDays(1))
            {
                if (nonWorkingDays.Contains(day.DayOfWeek) && !holidaysDateSet.Contains(day))
                {
                    holidaysCopy.Add(new Holiday { Date = day, Description = day.DayOfWeek.ToString() });
                }
            }
            return holidaysCopy;
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