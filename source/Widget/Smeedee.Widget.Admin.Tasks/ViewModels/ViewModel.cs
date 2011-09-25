
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using System.Collections.ObjectModel;
using TinyMVVM.Framework;
namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
	public partial class TasksViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public ObservableCollection<TaskViewModel> AvailableTasks
		{
			get 
			{
				OnGetAvailableTasks(ref _AvailableTasks);
				 
				return _AvailableTasks; 
			}
			set
			{
				if (value != _AvailableTasks)
				{
					OnSetAvailableTasks(ref value); 
					_AvailableTasks = value;
					TriggerPropertyChanged("AvailableTasks");
				}
			}
		}
		private ObservableCollection<TaskViewModel> _AvailableTasks;

		partial void OnGetAvailableTasks(ref ObservableCollection<TaskViewModel> value);
		partial void OnSetAvailableTasks(ref ObservableCollection<TaskViewModel> value);

		public ObservableCollection<TaskInstanceConfigurationViewModel> RunningTasks
		{
			get 
			{
				OnGetRunningTasks(ref _RunningTasks);
				 
				return _RunningTasks; 
			}
			set
			{
				if (value != _RunningTasks)
				{
					OnSetRunningTasks(ref value); 
					_RunningTasks = value;
					TriggerPropertyChanged("RunningTasks");
				}
			}
		}
		private ObservableCollection<TaskInstanceConfigurationViewModel> _RunningTasks;

		partial void OnGetRunningTasks(ref ObservableCollection<TaskInstanceConfigurationViewModel> value);
		partial void OnSetRunningTasks(ref ObservableCollection<TaskInstanceConfigurationViewModel> value);

		public TaskViewModel SelectedAvailableTask
		{
			get 
			{
				OnGetSelectedAvailableTask(ref _SelectedAvailableTask);
				 
				return _SelectedAvailableTask; 
			}
			set
			{
				if (value != _SelectedAvailableTask)
				{
					OnSetSelectedAvailableTask(ref value); 
					_SelectedAvailableTask = value;
					TriggerPropertyChanged("SelectedAvailableTask");
				}
			}
		}
		private TaskViewModel _SelectedAvailableTask;

		partial void OnGetSelectedAvailableTask(ref TaskViewModel value);
		partial void OnSetSelectedAvailableTask(ref TaskViewModel value);

		public bool RunningTaskIsSelected
		{
			get 
			{
				OnGetRunningTaskIsSelected(ref _RunningTaskIsSelected);
				 
				return _RunningTaskIsSelected; 
			}
			set
			{
				if (value != _RunningTaskIsSelected)
				{
					OnSetRunningTaskIsSelected(ref value); 
					_RunningTaskIsSelected = value;
					TriggerPropertyChanged("RunningTaskIsSelected");
				}
			}
		}
		private bool _RunningTaskIsSelected;

		partial void OnGetRunningTaskIsSelected(ref bool value);
		partial void OnSetRunningTaskIsSelected(ref bool value);

		public bool AvailableTaskIsSelected
		{
			get 
			{
				OnGetAvailableTaskIsSelected(ref _AvailableTaskIsSelected);
				 
				return _AvailableTaskIsSelected; 
			}
			set
			{
				if (value != _AvailableTaskIsSelected)
				{
					OnSetAvailableTaskIsSelected(ref value); 
					_AvailableTaskIsSelected = value;
					TriggerPropertyChanged("AvailableTaskIsSelected");
				}
			}
		}
		private bool _AvailableTaskIsSelected;

		partial void OnGetAvailableTaskIsSelected(ref bool value);
		partial void OnSetAvailableTaskIsSelected(ref bool value);

		public TaskInstanceConfigurationViewModel SelectedRunningTask
		{
			get 
			{
				OnGetSelectedRunningTask(ref _SelectedRunningTask);
				 
				return _SelectedRunningTask; 
			}
			set
			{
				if (value != _SelectedRunningTask)
				{
					OnSetSelectedRunningTask(ref value); 
					_SelectedRunningTask = value;
					TriggerPropertyChanged("SelectedRunningTask");
				}
			}
		}
		private TaskInstanceConfigurationViewModel _SelectedRunningTask;

		partial void OnGetSelectedRunningTask(ref TaskInstanceConfigurationViewModel value);
		partial void OnSetSelectedRunningTask(ref TaskInstanceConfigurationViewModel value);

		public bool HasChanges
		{
			get 
			{
				OnGetHasChanges(ref _HasChanges);
				 
				return _HasChanges; 
			}
			set
			{
				if (value != _HasChanges)
				{
					OnSetHasChanges(ref value); 
					_HasChanges = value;
					TriggerPropertyChanged("HasChanges");
				}
			}
		}
		private bool _HasChanges;

		partial void OnGetHasChanges(ref bool value);
		partial void OnSetHasChanges(ref bool value);

	
		
		//Commands
		public DelegateCommand ActivateSelectedTask { get; set; }
		public DelegateCommand DeactivateSelectedTask { get; set; }
		public DelegateCommand SaveChanges { get; set; }
		public DelegateCommand ReloadSettings { get; set; }
		
		public TasksViewModel()
		{
			ActivateSelectedTask = new DelegateCommand();
			DeactivateSelectedTask = new DelegateCommand();
			SaveChanges = new DelegateCommand();
			ReloadSettings = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
	public partial class TaskViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public string Author 
		{ 
			get
			{
				OnGetAuthor(ref _Author);
				 
				return _Author; 
			}
			set 
			{
				OnSetAuthor(ref value); 
				_Author = value; 
			} 
		}

		private string _Author;
		partial void OnGetAuthor(ref string value);
		partial void OnSetAuthor(ref string value);
		public string Description 
		{ 
			get
			{
				OnGetDescription(ref _Description);
				 
				return _Description; 
			}
			set 
			{
				OnSetDescription(ref value); 
				_Description = value; 
			} 
		}

		private string _Description;
		partial void OnGetDescription(ref string value);
		partial void OnSetDescription(ref string value);
		public long Version 
		{ 
			get
			{
				OnGetVersion(ref _Version);
				 
				return _Version; 
			}
			set 
			{
				OnSetVersion(ref value); 
				_Version = value; 
			} 
		}

		private long _Version;
		partial void OnGetVersion(ref long value);
		partial void OnSetVersion(ref long value);
		public string Webpage 
		{ 
			get
			{
				OnGetWebpage(ref _Webpage);
				 
				return _Webpage; 
			}
			set 
			{
				OnSetWebpage(ref value); 
				_Webpage = value; 
			} 
		}

		private string _Webpage;
		partial void OnGetWebpage(ref string value);
		partial void OnSetWebpage(ref string value);
		public string Name
		{
			get 
			{
				OnGetName(ref _Name);
				 
				return _Name; 
			}
			set
			{
				if (value != _Name)
				{
					OnSetName(ref value); 
					_Name = value;
					TriggerPropertyChanged("Name");
				}
			}
		}
		private string _Name;

		partial void OnGetName(ref string value);
		partial void OnSetName(ref string value);

		public ObservableCollection<ConfigurationEntryViewModel> ConfigurationEntries
		{
			get 
			{
				OnGetConfigurationEntries(ref _ConfigurationEntries);
				 
				return _ConfigurationEntries; 
			}
			set
			{
				if (value != _ConfigurationEntries)
				{
					OnSetConfigurationEntries(ref value); 
					_ConfigurationEntries = value;
					TriggerPropertyChanged("ConfigurationEntries");
				}
			}
		}
		private ObservableCollection<ConfigurationEntryViewModel> _ConfigurationEntries;

		partial void OnGetConfigurationEntries(ref ObservableCollection<ConfigurationEntryViewModel> value);
		partial void OnSetConfigurationEntries(ref ObservableCollection<ConfigurationEntryViewModel> value);

	
		
		//Commands
		
		public TaskViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
	public partial class TaskInstanceConfigurationViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public string AvailableTaskName
		{
			get 
			{
				OnGetAvailableTaskName(ref _AvailableTaskName);
				 
				return _AvailableTaskName; 
			}
			set
			{
				if (value != _AvailableTaskName)
				{
					OnSetAvailableTaskName(ref value); 
					_AvailableTaskName = value;
					TriggerPropertyChanged("AvailableTaskName");
				}
			}
		}
		private string _AvailableTaskName;

		partial void OnGetAvailableTaskName(ref string value);
		partial void OnSetAvailableTaskName(ref string value);

		public string RunningTaskName
		{
			get 
			{
				OnGetRunningTaskName(ref _RunningTaskName);
				 
				return _RunningTaskName; 
			}
			set
			{
				if (value != _RunningTaskName)
				{
					OnSetRunningTaskName(ref value); 
					_RunningTaskName = value;
					TriggerPropertyChanged("RunningTaskName");
				}
			}
		}
		private string _RunningTaskName;

		partial void OnGetRunningTaskName(ref string value);
		partial void OnSetRunningTaskName(ref string value);

		public int DispatchIntervalHours
		{
			get 
			{
				OnGetDispatchIntervalHours(ref _DispatchIntervalHours);
				 
				return _DispatchIntervalHours; 
			}
			set
			{
				if (value != _DispatchIntervalHours)
				{
					OnSetDispatchIntervalHours(ref value); 
					_DispatchIntervalHours = value;
					TriggerPropertyChanged("DispatchIntervalHours");
				}
			}
		}
		private int _DispatchIntervalHours;

		partial void OnGetDispatchIntervalHours(ref int value);
		partial void OnSetDispatchIntervalHours(ref int value);

		public int DispatchIntervalMinutes
		{
			get 
			{
				OnGetDispatchIntervalMinutes(ref _DispatchIntervalMinutes);
				 
				return _DispatchIntervalMinutes; 
			}
			set
			{
				if (value != _DispatchIntervalMinutes)
				{
					OnSetDispatchIntervalMinutes(ref value); 
					_DispatchIntervalMinutes = value;
					TriggerPropertyChanged("DispatchIntervalMinutes");
				}
			}
		}
		private int _DispatchIntervalMinutes;

		partial void OnGetDispatchIntervalMinutes(ref int value);
		partial void OnSetDispatchIntervalMinutes(ref int value);

		public int DispatchIntervalSeconds
		{
			get 
			{
				OnGetDispatchIntervalSeconds(ref _DispatchIntervalSeconds);
				 
				return _DispatchIntervalSeconds; 
			}
			set
			{
				if (value != _DispatchIntervalSeconds)
				{
					OnSetDispatchIntervalSeconds(ref value); 
					_DispatchIntervalSeconds = value;
					TriggerPropertyChanged("DispatchIntervalSeconds");
				}
			}
		}
		private int _DispatchIntervalSeconds;

		partial void OnGetDispatchIntervalSeconds(ref int value);
		partial void OnSetDispatchIntervalSeconds(ref int value);

		public ObservableCollection<ConfigurationEntryViewModel> ConfigurationEntries
		{
			get 
			{
				OnGetConfigurationEntries(ref _ConfigurationEntries);
				 
				return _ConfigurationEntries; 
			}
			set
			{
				if (value != _ConfigurationEntries)
				{
					OnSetConfigurationEntries(ref value); 
					_ConfigurationEntries = value;
					TriggerPropertyChanged("ConfigurationEntries");
				}
			}
		}
		private ObservableCollection<ConfigurationEntryViewModel> _ConfigurationEntries;

		partial void OnGetConfigurationEntries(ref ObservableCollection<ConfigurationEntryViewModel> value);
		partial void OnSetConfigurationEntries(ref ObservableCollection<ConfigurationEntryViewModel> value);

	
		
		//Commands
		
		public TaskInstanceConfigurationViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widget.Admin.Tasks.ViewModels
{
	public partial class ConfigurationEntryViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public int OrderIndex 
		{ 
			get
			{
				OnGetOrderIndex(ref _OrderIndex);
				 
				return _OrderIndex; 
			}
			set 
			{
				OnSetOrderIndex(ref value); 
				_OrderIndex = value; 
			} 
		}

		private int _OrderIndex;
		partial void OnGetOrderIndex(ref int value);
		partial void OnSetOrderIndex(ref int value);
		public string Name
		{
			get 
			{
				OnGetName(ref _Name);
				 
				return _Name; 
			}
			set
			{
				if (value != _Name)
				{
					OnSetName(ref value); 
					_Name = value;
					TriggerPropertyChanged("Name");
				}
			}
		}
		private string _Name;

		partial void OnGetName(ref string value);
		partial void OnSetName(ref string value);

		public Type Type
		{
			get 
			{
				OnGetType(ref _Type);
				 
				return _Type; 
			}
			set
			{
				if (value != _Type)
				{
					OnSetType(ref value); 
					_Type = value;
					TriggerPropertyChanged("Type");
				}
			}
		}
		private Type _Type;

		partial void OnGetType(ref Type value);
		partial void OnSetType(ref Type value);

		public object Value
		{
			get 
			{
				OnGetValue(ref _Value);
				 
				return _Value; 
			}
			set
			{
				if (value != _Value)
				{
					OnSetValue(ref value); 
					_Value = value;
					TriggerPropertyChanged("Value");
				}
			}
		}
		private object _Value;

		partial void OnGetValue(ref object value);
		partial void OnSetValue(ref object value);


        public string HelpText
        {
            get
            {
                OnGetHelpText(ref _HelpText);

                return _HelpText;
            }
            set
            {
                if (value != _HelpText)
                {
                    OnSetHelpText(ref value);
                    _HelpText = value;
                    TriggerPropertyChanged("HelpText");
                }
            }
        }
        private string _HelpText;

        partial void OnGetHelpText(ref string value);
        partial void OnSetHelpText(ref string value);

	
		
		//Commands
		
		public ConfigurationEntryViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

