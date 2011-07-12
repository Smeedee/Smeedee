
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
				OnSetChartType(ref value); 
				_ChartType = value; 
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

		public virtual ObservableCollection<ChartTypeViewModel> ChartType
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
		private ObservableCollection<ChartTypeViewModel> _ChartType;

		partial void OnGetChartType(ref ObservableCollection<ChartTypeViewModel> value);
		partial void OnSetChartType(ref ObservableCollection<ChartTypeViewModel> value);

		public virtual ObservableCollection<DatabaseViewModel> Databases 
		{ 
			get
			{
				OnGetDatabases(ref _Databases);
				 
				return _Databases; 
			}
			set 
			{
				OnSetDatabases(ref value); 
				_Databases = value; 
			} 
		}

		private ObservableCollection<DatabaseViewModel> _Databases;
		partial void OnGetDatabases(ref ObservableCollection<DatabaseViewModel> value);
		partial void OnSetDatabases(ref ObservableCollection<DatabaseViewModel> value);
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

		public virtual ObservableCollection<string> AvailableProperties
		{
			get 
			{
				OnGetAvailableProperties(ref _AvailableProperties);
				 
				return _AvailableProperties; 
			}
			set
			{
				if (value != _AvailableProperties)
				{
					OnSetAvailableProperties(ref value); 
					_AvailableProperties = value;
					TriggerPropertyChanged("AvailableProperties");
				}
			}
		}
		private ObservableCollection<string> _AvailableProperties;

		partial void OnGetAvailableProperties(ref ObservableCollection<string> value);
		partial void OnSetAvailableProperties(ref ObservableCollection<string> value);

		public virtual string SelectedPropertyForXAxis
		{
			get 
			{
				OnGetSelectedPropertyForXAxis(ref _SelectedPropertyForXAxis);
				 
				return _SelectedPropertyForXAxis; 
			}
			set
			{
				if (value != _SelectedPropertyForXAxis)
				{
					OnSetSelectedPropertyForXAxis(ref value); 
					_SelectedPropertyForXAxis = value;
					TriggerPropertyChanged("SelectedPropertyForXAxis");
				}
			}
		}
		private string _SelectedPropertyForXAxis;

		partial void OnGetSelectedPropertyForXAxis(ref string value);
		partial void OnSetSelectedPropertyForXAxis(ref string value);

		public virtual string SelectedPropertyForYAxis
		{
			get 
			{
				OnGetSelectedPropertyForYAxis(ref _SelectedPropertyForYAxis);
				 
				return _SelectedPropertyForYAxis; 
			}
			set
			{
				if (value != _SelectedPropertyForYAxis)
				{
					OnSetSelectedPropertyForYAxis(ref value); 
					_SelectedPropertyForYAxis = value;
					TriggerPropertyChanged("SelectedPropertyForYAxis");
				}
			}
		}
		private string _SelectedPropertyForYAxis;

		partial void OnGetSelectedPropertyForYAxis(ref string value);
		partial void OnSetSelectedPropertyForYAxis(ref string value);

		public virtual ChartViewModel Chart
		{
			get 
			{
				OnGetChart(ref _Chart);
				 
				return _Chart; 
			}
			set
			{
				if (value != _Chart)
				{
					OnSetChart(ref value); 
					_Chart = value;
					TriggerPropertyChanged("Chart");
				}
			}
		}
		private ChartViewModel _Chart;

		partial void OnGetChart(ref ChartViewModel value);
		partial void OnSetChart(ref ChartViewModel value);

	
		
		//Commands
		public DelegateCommand SaveSettings { get; set; }
		public DelegateCommand ReloadSettings { get; set; }
		public DelegateCommand Add { get; set; }
		public DelegateCommand Remove { get; set; }
		
		public ChartSettingsViewModel()
		{
			SaveSettings = new DelegateCommand();
			ReloadSettings = new DelegateCommand();
			Add = new DelegateCommand();
			Remove = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

