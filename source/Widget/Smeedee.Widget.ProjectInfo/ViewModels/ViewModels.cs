
using TinyMVVM.Framework.Services;
using System;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Smeedee.DomainModel.Holidays;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Widget.ProjectInfo.ViewModels
{
	public partial class WorkingDaysLeftViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public bool HasInformationToShow
		{
			get { return _HasInformationToShow; }
			set
			{
				if (value != _HasInformationToShow)
				{
					_HasInformationToShow = value;
					TriggerPropertyChanged("HasInformationToShow");
				}
			}
		}
		private bool _HasInformationToShow;

		public DateTime EndDate
		{
			get { return _EndDate; }
			set
			{
				if (value != _EndDate)
				{
					_EndDate = value;
					TriggerPropertyChanged("EndDate");
				}
			}
		}
		private DateTime _EndDate;

		public IEnumerable<Holiday> Holidays { get; set; } 
	
		
		//Commands
		
		public WorkingDaysLeftViewModel()
		{
	
			ApplyDefaultConventions();
		}
	}
		
	public partial class WorkingDaysLeftSettingsViewModel : SettingsViewModelBase
	{
		//State
		public bool IsLoading
		{
			get { return _IsLoading; }
			set
			{
				if (value != _IsLoading)
				{
					_IsLoading = value;
					TriggerPropertyChanged("IsLoading");
				}
			}
		}
		private bool _IsLoading;

		public bool IsManuallyConfigured
		{
			get { return _IsManuallyConfigured; }
			set
			{
				if (value != _IsManuallyConfigured)
				{
					_IsManuallyConfigured = value;
					TriggerPropertyChanged("IsManuallyConfigured");
				}
			}
		}
        private bool isLoadingConfig;
        public bool IsLoadingConfig
        {
            get { return isLoadingConfig; }
            set
            {
                if (value != isLoadingConfig)
                {
                    isLoadingConfig = value;
                    TriggerPropertyChanged<WorkingDaysLeftSettingsViewModel>(t => t.IsLoadingConfig);
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
                    TriggerPropertyChanged<WorkingDaysLeftSettingsViewModel>(t => t.IsSaving);
                }
            }
        }
		private bool _IsManuallyConfigured;

		public DateTime SelectedEndDate
		{
			get { return _SelectedEndDate; }
			set
			{
				if (value != _SelectedEndDate)
				{
					_SelectedEndDate = value;
					TriggerPropertyChanged("SelectedEndDate");
				}
			}
		}
		private DateTime _SelectedEndDate;

		public List<string> AvailableServers
		{
			get { return _AvailableServers; }
			set
			{
				if (value != _AvailableServers)
				{
					_AvailableServers = value;
					TriggerPropertyChanged("AvailableServers");
				}
			}
		}
		private List<string> _AvailableServers;

		public List<string> AvailableProjects
		{
			get { return _AvailableProjects; }
			set
			{
				if (value != _AvailableProjects)
				{
					_AvailableProjects = value;
					TriggerPropertyChanged("AvailableProjects");
				}
			}
		}
		private List<string> _AvailableProjects;

		public string SelectedServer
		{
			get { return _SelectedServer; }
			set
			{
				if (value != _SelectedServer)
				{
					_SelectedServer = value;
					TriggerPropertyChanged("SelectedServer");
				}
			}
		}
		private string _SelectedServer;

		public string SelectedProject
		{
			get { return _SelectedProject; }
			set
			{
				if (value != _SelectedProject)
				{
					_SelectedProject = value;
					TriggerPropertyChanged("SelectedProject");
				}
			}
		}
		private string _SelectedProject;

	
		
		//Commands
		public DelegateCommand ReloadSettings { get; set; }
		public DelegateCommand RefreshAvailableServers { get; set; }
		
		public WorkingDaysLeftSettingsViewModel()
		{
			ReloadSettings = new DelegateCommand();
			RefreshAvailableServers = new DelegateCommand();
	
			ApplyDefaultConventions();
		}
	}
		
}