using System;
using System.Collections.ObjectModel;
using Smeedee.Widget.Admin.Tasks.ViewModels;

namespace Smeedee.Widget.Admin.Tasks
{
    public static class FakeData
    {
        public static ObservableCollection<TaskViewModel> GetAllTasks()
        {
            var allTasks = new ObservableCollection<TaskViewModel>();
            for (int i = 1; i <= 5; i++)
            {
                allTasks.Add(new TaskViewModel { Name = "Task " + i });
            }
            return allTasks;
        }

        public static ObservableCollection<TaskInstanceConfigurationViewModel> GetActiveTasks()
        {
            var instances = new ObservableCollection<TaskInstanceConfigurationViewModel>();
            instances.Add(
                new TaskInstanceConfigurationViewModel
                    {
                        TaskName = "Task 1",
                        InstanceName = "Instance of Task 1",
                        DispatchInterval = 200,
                        ConfigurationEntries = new ObservableCollection<ConfigurationEntryViewModel>
                                                   {
                                                       new ConfigurationEntryViewModel
                                                           {
                                                               Name = "Username",
                                                               Type = typeof(string),
                                                               Value = "Alex"
                                                           },
                                                       new ConfigurationEntryViewModel
                                                           {
                                                               Name = "Password",
                                                               Type = typeof(string),
                                                               Value = "pa55word"
                                                           },
                                                       new ConfigurationEntryViewModel
                                                           {
                                                               Name = "UseHttps",
                                                               Type = typeof(bool),
                                                               Value = false
                                                           },
                                                   }
                    });
            return instances;
        }
    }
}