
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
	public partial class DataPointViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual Object X 
		{ 
			get
			{
				OnGetX(ref _X);
				 
				return _X; 
			}
			set 
			{
				OnSetX(ref value); 
				_X = value; 
			} 
		}

		private Object _X;
		partial void OnGetX(ref Object value);
		partial void OnSetX(ref Object value);
		public virtual Object Y 
		{ 
			get
			{
				OnGetY(ref _Y);
				 
				return _Y; 
			}
			set 
			{
				OnSetY(ref value); 
				_Y = value; 
			} 
		}

		private Object _Y;
		partial void OnGetY(ref Object value);
		partial void OnSetY(ref Object value);
	
		
		//Commands
		
		public DataPointViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
	public partial class DataSetViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Name 
		{ 
			get
			{
				OnGetName(ref _Name);
				 
				return _Name; 
			}
			set 
			{
				OnSetName(ref value); 
				_Name = value; 
			} 
		}

		private string _Name;
		partial void OnGetName(ref string value);
		partial void OnSetName(ref string value);
		public virtual ObservableCollection<DataPointViewModel> Data 
		{ 
			get
			{
				OnGetData(ref _Data);
				 
				return _Data; 
			}
			set 
			{
				OnSetData(ref value); 
				_Data = value; 
			} 
		}

		private ObservableCollection<DataPointViewModel> _Data;
		partial void OnGetData(ref ObservableCollection<DataPointViewModel> value);
		partial void OnSetData(ref ObservableCollection<DataPointViewModel> value);
	
		
		//Commands
		
		public DataSetViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
	public partial class ChartTypeViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string ChartType
		{
			get 
			{
				OnGetChartType(ref _ChartType);
				 
				return _ChartType; 
			}
			set
			{
				if (value != _ChartType)
				{
					OnSetChartType(ref value); 
					_ChartType = value;
					TriggerPropertyChanged("ChartType");
				}
			}
		}
		private string _ChartType;

		partial void OnGetChartType(ref string value);
		partial void OnSetChartType(ref string value);

	
		
		//Commands
		
		public ChartTypeViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
	public partial class CollectionViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Name 
		{ 
			get
			{
				OnGetName(ref _Name);
				 
				return _Name; 
			}
			set 
			{
				OnSetName(ref value); 
				_Name = value; 
			} 
		}

		private string _Name;
		partial void OnGetName(ref string value);
		partial void OnSetName(ref string value);
	
		
		//Commands
		
		public CollectionViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

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

namespace Smeedee.Widgets.GenericCharting.ViewModels
{
	public partial class SeriesConfigViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Action
		{
			get 
			{
				OnGetAction(ref _Action);
				 
				return _Action; 
			}
			set
			{
				if (value != _Action)
				{
					OnSetAction(ref value); 
					_Action = value;
					TriggerPropertyChanged("Action");
				}
			}
		}
		private string _Action;

		partial void OnGetAction(ref string value);
		partial void OnSetAction(ref string value);

		public virtual string Name
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

		public virtual string Legend
		{
			get 
			{
				OnGetLegend(ref _Legend);
				 
				return _Legend; 
			}
			set
			{
				if (value != _Legend)
				{
					OnSetLegend(ref value); 
					_Legend = value;
					TriggerPropertyChanged("Legend");
				}
			}
		}
		private string _Legend;

		partial void OnGetLegend(ref string value);
		partial void OnSetLegend(ref string value);

		public virtual string ChartType
		{
			get 
			{
				OnGetChartType(ref _ChartType);
				 
				return _ChartType; 
			}
			set
			{
				if (value != _ChartType)
				{
					OnSetChartType(ref value); 
					_ChartType = value;
					TriggerPropertyChanged("ChartType");
				}
			}
		}
		private string _ChartType;

		partial void OnGetChartType(ref string value);
		partial void OnSetChartType(ref string value);

		public virtual string Database
		{
			get 
			{
				OnGetDatabase(ref _Database);
				 
				return _Database; 
			}
			set
			{
				if (value != _Database)
				{
					OnSetDatabase(ref value); 
					_Database = value;
					TriggerPropertyChanged("Database");
				}
			}
		}
		private string _Database;

		partial void OnGetDatabase(ref string value);
		partial void OnSetDatabase(ref string value);

		public virtual string Collection
		{
			get 
			{
				OnGetCollection(ref _Collection);
				 
				return _Collection; 
			}
			set
			{
				if (value != _Collection)
				{
					OnSetCollection(ref value); 
					_Collection = value;
					TriggerPropertyChanged("Collection");
				}
			}
		}
		private string _Collection;

		partial void OnGetCollection(ref string value);
		partial void OnSetCollection(ref string value);

		public virtual string DatabaseAndCollection
		{
			get 
			{
				OnGetDatabaseAndCollection(ref _DatabaseAndCollection);
				 
				return _DatabaseAndCollection; 
			}
			set
			{
				if (value != _DatabaseAndCollection)
				{
					OnSetDatabaseAndCollection(ref value); 
					_DatabaseAndCollection = value;
					TriggerPropertyChanged("DatabaseAndCollection");
				}
			}
		}
		private string _DatabaseAndCollection;

		partial void OnGetDatabaseAndCollection(ref string value);
		partial void OnSetDatabaseAndCollection(ref string value);

	
		
		//Commands
		
		public SeriesConfigViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

