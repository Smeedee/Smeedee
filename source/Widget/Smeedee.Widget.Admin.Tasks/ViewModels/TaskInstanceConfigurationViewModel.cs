using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
    partial class TaskInstanceConfigurationViewModel
    {
        public int DispatchInterval
        {
            get
            {
                var hours = TimeSpan.FromHours(DispatchIntervalHours);
                var minutes = TimeSpan.FromMinutes(DispatchIntervalMinutes);
                var seconds = TimeSpan.FromSeconds(DispatchIntervalSeconds);

                var dispatchIntervalInMs = hours.Add(minutes).Add(seconds).TotalMilliseconds;
                return Convert.ToInt32(dispatchIntervalInMs);
                
            }
            set
            {
                var time = TimeSpan.FromMilliseconds(value);
                DispatchIntervalHours = time.Hours;
                DispatchIntervalMinutes = time.Minutes;
                DispatchIntervalSeconds = time.Seconds;
            }
        }

        public void SetInstanceNameWithoutFiringProperty(string instanceName)
        {
            _RunningTaskName = instanceName;
        }

        partial void OnSetConfigurationEntries(ref ObservableCollection<ConfigurationEntryViewModel> value)
        {
            var orderedSettings = new ObservableCollection<ConfigurationEntryViewModel>();
            foreach (var entry in value.OrderBy(t => t.OrderIndex))
            {
                orderedSettings.Add(entry);
            }
            value = orderedSettings;

            
        }
    }
}
