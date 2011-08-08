using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using TinyMVVM.Framework;

namespace Smeedee.Widget.CI.ViewModels
{
    public class CISettingsViewModel : AbstractViewModel
    {
        public CISettingsViewModel()
        {
            SaveSettings = new DelegateCommand();
            Servers = new ObservableCollection<ServerConfigViewModel>();
        }

        public void EachProject(Action<ProjectConfigViewModel> func)
        {
            foreach (var server in Servers)
                foreach (var project in server.Projects)
                    func(project);
        }

        private bool showTriggeredBy;
        public bool ShowTriggeredBy
        {
            get { return showTriggeredBy; }
            set
            {
                if (value != showTriggeredBy)
                {
                    showTriggeredBy = value;
                    TriggerPropertyChanged("ShowTriggeredBy");
                }
            }
        }

        private bool showTriggerCause;
        public bool ShowTriggerCause
        {
            get { return showTriggerCause; }
            set
            {
                if (value != showTriggerCause)
                {
                    showTriggerCause = value;
                    TriggerPropertyChanged("ShowTriggerCause");
                }
            }
        }

        private bool showStartTime;
        public bool ShowStartTime
        {
            get { return showStartTime; }
            set
            {
                if (value != showStartTime)
                {
                    showStartTime = value;
                    TriggerPropertyChanged("ShowStartTime");
                }
            }
        }

        private bool showDuration;
        public bool ShowDuration
        {
            get { return showDuration; }
            set
            {
                if (value != showDuration)
                {
                    showDuration = value;
                    TriggerPropertyChanged("ShowDuration");
                }
            }
        }

        private bool showStatus;
        public bool ShowStatus
        {
            get { return showStatus; }
            set
            {
                if (value != showStatus)
                {
                    showStatus = value;
                    TriggerPropertyChanged("ShowStatus");
                }
            }
        }

        private ObservableCollection<ServerConfigViewModel> servers;
        public ObservableCollection<ServerConfigViewModel> Servers
        {
            get { return servers; }
            set
            {
                if (value != servers)
                {
                    servers = value;
                    TriggerPropertyChanged<CISettingsViewModel>(t => t.Servers);
                }
            }
        }

        private int inactiveProjectThreshold;
        public int InactiveProjectThreshold
        {
            get { return inactiveProjectThreshold; }
            set
            {
                if (value != inactiveProjectThreshold)
                {
                    inactiveProjectThreshold = value;
                    TriggerPropertyChanged<CISettingsViewModel>(t => t.InactiveProjectThreshold);
                }
            }
        }

        private bool filterInactiveProjects;
        public bool FilterInactiveProjects
        {
            get { return filterInactiveProjects; }
            set
            {
                if (value != filterInactiveProjects)
                {
                    filterInactiveProjects = value;
                    TriggerPropertyChanged<CISettingsViewModel>(t => t.FilterInactiveProjects);
                }
            }
        }

        public DelegateCommand SaveSettings { get; set; }
        public DelegateCommand ReloadSettings { get; set; }


        public static string GetProjectSelectedSettingName(CIProject project)
        {
            return project.SystemId + "-is-shown";
        }
    }
    
    /* Wrap projects with their config, so that we can databind the checkboxes 
     * to settings entries, that we can save when we want to.*/
    public class ServerConfigViewModel
    {
        public IEnumerable<ProjectConfigViewModel> Projects { get; private set; }

        public string Name
        {
            get { 
                return String.Format("{0} ({1})", ServerName, ServerUrl);
            }
        }

        public string ServerName { get; private set; }
        public string ServerUrl { get; private set; }
        public ServerConfigViewModel(CIServer server, Configuration config)
        {
            ServerName = server.Name;
            ServerUrl = server.Url;
            Projects = (from project in server.Projects select new ProjectConfigViewModel(project, config)).ToList();
        }
    }


    public class ProjectConfigViewModel : AbstractViewModel
    {
        //public CIProject Project { get; private set; }

        public string ProjectName { get; private set; }

        public DateTime LatestBuildStartTime { get; private set; }

        private string selectedSettingsName;

        public SettingsEntry SelectedSetting;
        public bool IsSelected
        {
            get
            {
                return Boolean.Parse(SelectedSetting.Value);
            }
            set
            {
                SelectedSetting = new SettingsEntry(selectedSettingsName, value.ToString());
                TriggerPropertyChanged<ProjectConfigViewModel>(t => t.IsSelected);
            }
        }

        private bool initialSelectedState;
        public void ResetSelectedState()
        {
            IsSelected = initialSelectedState;
        }

        public void SetResetPoint()
        {
            initialSelectedState = IsSelected;
        }
        
        public ProjectConfigViewModel(CIProject project, Configuration config)
        {
            //this.Project = project;
            this.LatestBuildStartTime = project.LatestBuild.StartTime;
            ProjectName = project.ProjectName;

            selectedSettingsName = CISettingsViewModel.GetProjectSelectedSettingName(project);
            this.SelectedSetting = config.GetSetting(selectedSettingsName);
            this.initialSelectedState = Boolean.Parse(SelectedSetting.Value);
        }
    }
}
