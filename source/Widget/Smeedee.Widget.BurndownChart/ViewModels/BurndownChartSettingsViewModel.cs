using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.BurndownChart.ViewModels
{
    public class BurndownChartSettingsViewModel : SettingsViewModelBase
    {

        public BurndownChartSettingsViewModel()
        {
            AvailableProjects = new ObservableCollection<string>();
        }

        private bool includeWorkItemsBeforeIterationStartdate;

        public bool IncludeWorkItemsBeforeIterationStartdate
        {
            get { return includeWorkItemsBeforeIterationStartdate; }
            set
            {
                if (value != includeWorkItemsBeforeIterationStartdate)
                {
                    includeWorkItemsBeforeIterationStartdate = value;
                    HasChanges = true;
                    TriggerPropertyChanged("IncludeWorkItemsBeforeIterationStartdate");
                }
            }
        }

        private ObservableCollection<string> availableProjects;

        public ObservableCollection<string> AvailableProjects
        {
            get { return availableProjects; }
            set
            {
                if (value != availableProjects)
                {
                    availableProjects = value;
                    TriggerPropertyChanged("AvailableProjects");
                }
            }
        }

        private string selectedProjectName;

        public string SelectedProjectName
        {
            get { return selectedProjectName; }
            set
            {
                if (value != selectedProjectName && value != null)
                {
                    selectedProjectName = value;
                    HasChanges = true;
					TriggerPropertyChanged("SelectedProjectName");
                }
            }
        }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (value != isLoading)
                {
                    isLoading = value;
                    TriggerPropertyChanged<AbstractViewModel>(vm => vm.IsLoading);
                }
            }
        }

        private bool isLoadingConfig = true;
        public bool IsLoadingConfig
        {
            get { return isLoadingConfig; }
            set
            {
                if (value != isLoadingConfig)
                {
                    isLoadingConfig = value;
                    TriggerPropertyChanged<AbstractViewModel>(vm => vm.IsLoadingConfig);
                }
            }
        }

        private bool isSaving;
        public bool IsSaving
        {
            get { return isSaving; }
            set
            {
                if (value != isSaving)
                {
                    isSaving = value;
                    TriggerPropertyChanged<AbstractViewModel>(vm => vm.IsSaving);
                }
            }
        }

    }
}
