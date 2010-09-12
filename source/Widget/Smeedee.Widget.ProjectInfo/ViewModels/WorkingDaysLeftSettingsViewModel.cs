using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Smeedee.Widget.ProjectInfo.ViewModels
{
    public partial class WorkingDaysLeftSettingsViewModel
    {
        
        public ObservableCollection<WorkingDay> NonWorkWeekDays { get { return _NonWorkWeekDays; } }
        private ObservableCollection<WorkingDay> _NonWorkWeekDays = new ObservableCollection<WorkingDay>
            {
                new WorkingDay(DayOfWeek.Monday, false),
                new WorkingDay(DayOfWeek.Tuesday, false),
                new WorkingDay(DayOfWeek.Wednesday, false),
                new WorkingDay(DayOfWeek.Thursday, false),
                new WorkingDay(DayOfWeek.Friday, false),
                new WorkingDay(DayOfWeek.Saturday, true),
                new WorkingDay(DayOfWeek.Sunday, true)
            };

        private DateTime _SelectedEndDateResetPoint;
        private List<string> _AvailableServersResetPoint;
        private string _SelectedServerResetPoint;
        private string _SelectedProjectResetPoint;
        private bool _IsManuallyConfiguredResetPoint;


      
        public void OnInitialize()
        {
            AvailableProjects = new List<string>();
            AvailableServers = new List<string>();
            IsManuallyConfigured = true;
            SelectedEndDate = DateTime.Now.Date;
            SetResetPoint();
        }

        public void SetResetPoint()
        {
            _IsManuallyConfiguredResetPoint = IsManuallyConfigured;
            _AvailableServersResetPoint = AvailableServers;
            _SelectedEndDateResetPoint = SelectedEndDate;
            _SelectedServerResetPoint = SelectedServer;
            _SelectedProjectResetPoint = SelectedProject;
            foreach (var day in _NonWorkWeekDays)
                day.SetResetPoint();
        }

        public void Reset()
        {
            UIInvoker.Invoke(() =>
            {
                IsManuallyConfigured = _IsManuallyConfiguredResetPoint;
                AvailableServers = _AvailableServersResetPoint;
                SelectedEndDate = _SelectedEndDateResetPoint;
                SelectedServer = _SelectedServerResetPoint;
                SelectedProject = _SelectedProjectResetPoint;
                foreach (var day in _NonWorkWeekDays)
                    day.Reset();
            });
        }
    }

    public class WorkingDay : Client.Framework.ViewModel.AbstractViewModel
    {
        private DayOfWeek day;
        public DayOfWeek Day { get { return day; } }

        private bool isNotWorkingDay;
        private bool isNotWorkingDayResetPoint;
        public bool IsNotWorkingDay
        {
            get { return isNotWorkingDay; }
            set
            {
                if (value != isNotWorkingDay)
                {
                    isNotWorkingDay = value;
                    TriggerPropertyChanged<WorkingDay>(t => t.IsNotWorkingDay);
                }
            }
        }

        public WorkingDay(DayOfWeek day, bool isNotWorkingDay)
        {
            this.day = day;
            this.isNotWorkingDay = isNotWorkingDay;
            SetResetPoint();
        }

        public void SetResetPoint()
        {
            isNotWorkingDayResetPoint = IsNotWorkingDay;
        }

        public void Reset()
        {
            IsNotWorkingDay = isNotWorkingDayResetPoint;
        }
    }
}
