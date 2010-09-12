using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Smeedee.Client.Framework.Services;
using TinyMVVM.IoC;

namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
    public partial class TasksViewModel
    {
        partial void OnInitialize()
        {
            AvailableTasks = new ObservableCollection<TaskViewModel>();
            RunningTasks = new ObservableCollection<TaskInstanceConfigurationViewModel>();
            RunningTasks.CollectionChanged += SetHasChanges;

            PropertyChanged += TasksViewModel_PropertyChanged;
        }

        private void SetHasChanges(object sender, EventArgs e)
        {
            HasChanges = true;
        }

        private void TasksViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedAvailableTask":
                    ActivateSelectedTask.TriggerCanExecuteChanged();
                    if(AvailableTaskIsSelected)
                        SelectedRunningTask = null;
                    break;
                case "SelectedRunningTask":
                    DeactivateSelectedTask.TriggerCanExecuteChanged();
                    if(RunningTaskIsSelected)
                        SelectedAvailableTask = null;
                    break;
                case "HasChanges":
                    SaveChanges.TriggerCanExecuteChanged();
                    break;
            }
        }

        public void AddTask(TaskViewModel taskViewModel)
        {
            if(!TaskAlreadyExist(taskViewModel))
                AvailableTasks.Add(taskViewModel);
        }

        private bool TaskAlreadyExist(TaskViewModel taskViewModel)
        {
            return AvailableTasks.Any(task => task.Name == taskViewModel.Name && task.Version == taskViewModel.Version && task.Description == taskViewModel.Description);
        }

        public void AddTaskInstance(TaskInstanceConfigurationViewModel taskInstanceViewModel)
        {
            if (!TaskInstanceNameAlreadyExists(taskInstanceViewModel.RunningTaskName))
                SubscribeRunningTaskToEvents(taskInstanceViewModel);
                RunningTasks.Add(taskInstanceViewModel);
        }

        private bool TaskInstanceNameAlreadyExists(String taskConfigName)
        {
            return RunningTasks.Any(taskInstance => taskInstance.RunningTaskName == taskConfigName);
        }
        
        private void SubscribeRunningTaskToEvents(TaskInstanceConfigurationViewModel taskInstanceViewModel)
        {
            taskInstanceViewModel.PropertyChanged += TaskConfigNameHasChanged;

            if (taskInstanceViewModel.ConfigurationEntries != null)
            {
                foreach (var configEntries in taskInstanceViewModel.ConfigurationEntries)
                {
                        configEntries.ConfigChanged += SetHasChanges;
                }
            }
        }

        private void TaskConfigNameHasChanged(object sender, EventArgs args)
        {
            var editedTaskInstanceName = ((TaskInstanceConfigurationViewModel) sender).RunningTaskName;
            if (TaskInstanceNameAlreadyExists(editedTaskInstanceName))
                SelectedRunningTask.RunningTaskName = GetNameWithPostfix(editedTaskInstanceName);
                
            HasChanges = true;
        }

        private string GetNameWithPostfix(string taskInstanceName)
        {
            var similiarTasks = _RunningTasks
                .Where(t => t.RunningTaskName.StartsWith(taskInstanceName) && t != SelectedRunningTask).ToList();

            var nameWithPostfix = taskInstanceName;

            int i = 0;
            while (similiarTasks.Select(t => t.RunningTaskName).Contains(nameWithPostfix))
            {
                i++;
                nameWithPostfix = taskInstanceName + "(" + i + ")";
            }
            return nameWithPostfix;
        }


        public void OnActivateSelectedTask()
        {
            var newTaskInstance = new TaskInstanceConfigurationViewModel
            {
                RunningTaskName = "New " + SelectedAvailableTask.Name,
                AvailableTaskName = SelectedAvailableTask.Name,
                ConfigurationEntries = SelectedAvailableTask.ConfigurationEntries
            };

            if (TaskInstanceNameAlreadyExists(newTaskInstance.RunningTaskName))
                newTaskInstance.RunningTaskName = GetNameWithPostfix(newTaskInstance.RunningTaskName);

            AddTaskInstance(newTaskInstance);
            SelectedAvailableTask = null;
            SelectedRunningTask = newTaskInstance;

        }

        public bool CanActivateSelectedTask()
        {
            AvailableTaskIsSelected = SelectedAvailableTask != null;

            if (AvailableTaskIsSelected)
            {
                RunningTaskIsSelected = false;
            }

            return AvailableTaskIsSelected;
        }

        public void OnDeactivateSelectedTask()
        {
            var messageBoxService = this.GetDependency<IMessageBoxService>();

            var result = messageBoxService.Show("Are you sure you want to remove "+SelectedRunningTask.RunningTaskName+" as a running task?", "", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                var nextSelectedTask = GetNextSelectedTaskInstance(RunningTasks.IndexOf(SelectedRunningTask));
                UnsubscribeRemovedTaskInstanceFromEvents(SelectedRunningTask);
                RunningTasks.Remove(SelectedRunningTask);
                SelectedRunningTask = nextSelectedTask;
            }      
        }

        private void UnsubscribeRemovedTaskInstanceFromEvents(TaskInstanceConfigurationViewModel taskInstanceViewModel)
        {
            taskInstanceViewModel.PropertyChanged -= TaskConfigNameHasChanged;

            if (taskInstanceViewModel.ConfigurationEntries != null)
            {
                foreach (var configEntries in taskInstanceViewModel.ConfigurationEntries)
                {
                    configEntries.ConfigChanged -= SetHasChanges;
                }
            }
        }

        private TaskInstanceConfigurationViewModel GetNextSelectedTaskInstance(int selectedTaskInstanceIndex)
        {
            TaskInstanceConfigurationViewModel nextSelectedTask = null;

            if (RunningTasks.Count > 1)
            {
                if (TaskInstanceIsAtEndOfList(selectedTaskInstanceIndex))
                    nextSelectedTask = GetTaskInstanceAbove(selectedTaskInstanceIndex);
                else
                    nextSelectedTask = GetTaskInstanceBelow(selectedTaskInstanceIndex);
            }

            return nextSelectedTask;
        }

        private bool TaskInstanceIsAtEndOfList(int taskInstanceIndex)
        {
            return taskInstanceIndex == RunningTasks.Count - 1;
        }

        private TaskInstanceConfigurationViewModel GetTaskInstanceAbove(int taskInstanceIndex)
        {
            return RunningTasks.ElementAt(taskInstanceIndex - 1);
        }

        private TaskInstanceConfigurationViewModel GetTaskInstanceBelow(int taskInstanceIndex)
        {
            return RunningTasks.ElementAt(taskInstanceIndex + 1);
        }

        public bool CanDeactivateSelectedTask()
        {
            RunningTaskIsSelected = SelectedRunningTask != null;

            if (RunningTaskIsSelected)
            {
                AvailableTaskIsSelected = false;
            }

            return RunningTaskIsSelected;
        }

        public bool CanSaveChanges()
        {
            return HasChanges;
        }

        public void OnSaveChanges()
        {
            HasChanges = false;
        }
    }
}