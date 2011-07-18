
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
	public partial class DatabaseViewModel : TinyMVVM.Framework.ViewModelBase
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
		public virtual ObservableCollection<CollectionViewModel> Collections 
		{ 
			get
			{
				OnGetCollections(ref _Collections);
				 
				return _Collections; 
			}
			set 
			{
				OnSetCollections(ref value); 
				_Collections = value; 
			} 
		}

		private ObservableCollection<CollectionViewModel> _Collections;
		partial void OnGetCollections(ref ObservableCollection<CollectionViewModel> value);
		partial void OnSetCollections(ref ObservableCollection<CollectionViewModel> value);
	
		
		//Commands
		
		public DatabaseViewModel()
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

		public virtual DatabaseViewModel SelectedDatabase
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
		private DatabaseViewModel _SelectedDatabase;

		partial void OnGetSelectedDatabase(ref DatabaseViewModel value);
		partial void OnSetSelectedDatabase(ref DatabaseViewModel value);

		public virtual CollectionViewModel SelectedCollection
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
		private CollectionViewModel _SelectedCollection;

		partial void OnGetSelectedCollection(ref CollectionViewModel value);
		partial void OnSetSelectedCollection(ref CollectionViewModel value);

	
		
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
	public partial class SetConfigViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string SelectedAction
		{
			get 
			{
				OnGetSelectedAction(ref _SelectedAction);
				 
				return _SelectedAction; 
			}
			set
			{
				if (value != _SelectedAction)
				{
					OnSetSelectedAction(ref value); 
					_SelectedAction = value;
					TriggerPropertyChanged("SelectedAction");
				}
			}
		}
		private string _SelectedAction;

		partial void OnGetSelectedAction(ref string value);
		partial void OnSetSelectedAction(ref string value);

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

		public virtual string DataName
		{
			get 
			{
				OnGetDataName(ref _DataName);
				 
				return _DataName; 
			}
			set
			{
				if (value != _DataName)
				{
					OnSetDataName(ref value); 
					_DataName = value;
					TriggerPropertyChanged("DataName");
				}
			}
		}
		private string _DataName;

		partial void OnGetDataName(ref string value);
		partial void OnSetDataName(ref string value);

		public virtual string SelectedChartType
		{
			get 
			{
				OnGetSelectedChartType(ref _SelectedChartType);
				 
				return _SelectedChartType; 
			}
			set
			{
				if (value != _SelectedChartType)
				{
					OnSetSelectedChartType(ref value); 
					_SelectedChartType = value;
					TriggerPropertyChanged("SelectedChartType");
				}
			}
		}
		private string _SelectedChartType;

		partial void OnGetSelectedChartType(ref string value);
		partial void OnSetSelectedChartType(ref string value);

	
		
		//Commands
		
		public SetConfigViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

