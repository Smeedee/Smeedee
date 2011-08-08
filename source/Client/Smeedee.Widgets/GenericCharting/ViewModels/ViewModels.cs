
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TinyMVVM.Framework;
namespace Smeedee.Widgets.GenericCharting.ViewModels
{
	public partial class ChartSettingsViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string ChartName
		{
			get 
			{
				OnGetChartName(ref _ChartName);
				 
				return _ChartName; 
			}
			set
			{
				if (value != _ChartName)
				{
					OnSetChartName(ref value); 
					_ChartName = value;
					TriggerPropertyChanged("ChartName");
				}
			}
		}
		private string _ChartName;

		partial void OnGetChartName(ref string value);
		partial void OnSetChartName(ref string value);

		public virtual string XAxisName
		{
			get 
			{
				OnGetXAxisName(ref _XAxisName);
				 
				return _XAxisName; 
			}
			set
			{
				if (value != _XAxisName)
				{
					OnSetXAxisName(ref value); 
					_XAxisName = value;
					TriggerPropertyChanged("XAxisName");
				}
			}
		}
		private string _XAxisName;

		partial void OnGetXAxisName(ref string value);
		partial void OnSetXAxisName(ref string value);

		public virtual string YAxisName
		{
			get 
			{
				OnGetYAxisName(ref _YAxisName);
				 
				return _YAxisName; 
			}
			set
			{
				if (value != _YAxisName)
				{
					OnSetYAxisName(ref value); 
					_YAxisName = value;
					TriggerPropertyChanged("YAxisName");
				}
			}
		}
		private string _YAxisName;

		partial void OnGetYAxisName(ref string value);
		partial void OnSetYAxisName(ref string value);

		public virtual string XAxisType
		{
			get 
			{
				OnGetXAxisType(ref _XAxisType);
				 
				return _XAxisType; 
			}
			set
			{
				if (value != _XAxisType)
				{
					OnSetXAxisType(ref value); 
					_XAxisType = value;
					TriggerPropertyChanged("XAxisType");
				}
			}
		}
		private string _XAxisType;

		partial void OnGetXAxisType(ref string value);
		partial void OnSetXAxisType(ref string value);

		public virtual ObservableCollection<string> XAxisTypes 
		{ 
			get
			{
				OnGetXAxisTypes(ref _XAxisTypes);
				 
				return _XAxisTypes; 
			}
			set 
			{
				OnSetXAxisTypes(ref value); 
				_XAxisTypes = value; 
			} 
		}

		private ObservableCollection<string> _XAxisTypes;
		partial void OnGetXAxisTypes(ref ObservableCollection<string> value);
		partial void OnSetXAxisTypes(ref ObservableCollection<string> value);
		public virtual ObservableCollection<string> Databases
		{
			get 
			{
				OnGetDatabases(ref _Databases);
				 
				return _Databases; 
			}
			set
			{
				if (value != _Databases)
				{
					OnSetDatabases(ref value); 
					_Databases = value;
					TriggerPropertyChanged("Databases");
				}
			}
		}
		private ObservableCollection<string> _Databases;

		partial void OnGetDatabases(ref ObservableCollection<string> value);
		partial void OnSetDatabases(ref ObservableCollection<string> value);

		public virtual ObservableCollection<string> Collections
		{
			get 
			{
				OnGetCollections(ref _Collections);
				 
				return _Collections; 
			}
			set
			{
				if (value != _Collections)
				{
					OnSetCollections(ref value); 
					_Collections = value;
					TriggerPropertyChanged("Collections");
				}
			}
		}
		private ObservableCollection<string> _Collections;

		partial void OnGetCollections(ref ObservableCollection<string> value);
		partial void OnSetCollections(ref ObservableCollection<string> value);

		public virtual ObservableCollection<SeriesConfigViewModel> SeriesConfig
		{
			get 
			{
				OnGetSeriesConfig(ref _SeriesConfig);
				 
				return _SeriesConfig; 
			}
			set
			{
				if (value != _SeriesConfig)
				{
					OnSetSeriesConfig(ref value); 
					_SeriesConfig = value;
					TriggerPropertyChanged("SeriesConfig");
				}
			}
		}
		private ObservableCollection<SeriesConfigViewModel> _SeriesConfig;

		partial void OnGetSeriesConfig(ref ObservableCollection<SeriesConfigViewModel> value);
		partial void OnSetSeriesConfig(ref ObservableCollection<SeriesConfigViewModel> value);

		public virtual string SelectedDatabase
		{
			get 
			{
				OnGetSelectedDatabase(ref _SelectedDatabase);
				 
				return _SelectedDatabase; 
			}
			set
			{
				if (value != _SelectedDatabase)
				{
					OnSetSelectedDatabase(ref value); 
					_SelectedDatabase = value;
					TriggerPropertyChanged("SelectedDatabase");
				}
			}
		}
		private string _SelectedDatabase;

		partial void OnGetSelectedDatabase(ref string value);
		partial void OnSetSelectedDatabase(ref string value);

		public virtual string SelectedCollection
		{
			get 
			{
				OnGetSelectedCollection(ref _SelectedCollection);
				 
				return _SelectedCollection; 
			}
			set
			{
				if (value != _SelectedCollection)
				{
					OnSetSelectedCollection(ref value); 
					_SelectedCollection = value;
					TriggerPropertyChanged("SelectedCollection");
				}
			}
		}
		private string _SelectedCollection;

		partial void OnGetSelectedCollection(ref string value);
		partial void OnSetSelectedCollection(ref string value);

	
		
		//Commands
		public DelegateCommand SaveSettings { get; set; }
		public DelegateCommand ReloadSettings { get; set; }
		public DelegateCommand AddDataSettings { get; set; }
		
		public ChartSettingsViewModel()
		{
			SaveSettings = new DelegateCommand();
			ReloadSettings = new DelegateCommand();
			AddDataSettings = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

