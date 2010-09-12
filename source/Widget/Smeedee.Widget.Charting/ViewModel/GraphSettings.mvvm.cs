

using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using System;
using System.Collections.ObjectModel;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.ViewModel
{
	public partial class GraphSettings : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public ObservableCollection<DatabaseViewModel> Databases { get; set; } 
		public DatabaseViewModel SelectedDatabase
		{
			get { return _SelectedDatabase; }
			set
			{
				if (value != _SelectedDatabase)
				{
					_SelectedDatabase = value;
					TriggerPropertyChanged("SelectedDatabase");
				}
			}
		}
		private DatabaseViewModel _SelectedDatabase;

		public CollectionViewModel SelectedCollection
		{
			get { return _SelectedCollection; }
			set
			{
				if (value != _SelectedCollection)
				{
					_SelectedCollection = value;
					TriggerPropertyChanged("SelectedCollection");
				}
			}
		}
		private CollectionViewModel _SelectedCollection;

		public ObservableCollection<string> AvailableProperties
		{
			get { return _AvailableProperties; }
			set
			{
				if (value != _AvailableProperties)
				{
					_AvailableProperties = value;
					TriggerPropertyChanged("AvailableProperties");
				}
			}
		}
		private ObservableCollection<string> _AvailableProperties;

		public string SelectedPropertyForXAxis
		{
			get { return _SelectedPropertyForXAxis; }
			set
			{
				if (value != _SelectedPropertyForXAxis)
				{
					_SelectedPropertyForXAxis = value;
					TriggerPropertyChanged("SelectedPropertyForXAxis");
				}
			}
		}
		private string _SelectedPropertyForXAxis;

		public string SelectedPropertyForYAxis
		{
			get { return _SelectedPropertyForYAxis; }
			set
			{
				if (value != _SelectedPropertyForYAxis)
				{
					_SelectedPropertyForYAxis = value;
					TriggerPropertyChanged("SelectedPropertyForYAxis");
				}
			}
		}
		private string _SelectedPropertyForYAxis;

		public int NumberOfDataPoints
		{
			get { return _NumberOfDataPoints; }
			set
			{
				if (value != _NumberOfDataPoints)
				{
					_NumberOfDataPoints = value;
					TriggerPropertyChanged("NumberOfDataPoints");
				}
			}
		}
		private int _NumberOfDataPoints;

		public Graph Graph
		{
			get { return _Graph; }
			set
			{
				if (value != _Graph)
				{
					_Graph = value;
					TriggerPropertyChanged("Graph");
				}
			}
		}
		private Graph _Graph;

	
		
		//Commands
		public DelegateCommand Save { get; set; }
		public DelegateCommand Refresh { get; set; }
		public DelegateCommand Test { get; set; }
		public DelegateCommand Cancel { get; set; }
		
		public GraphSettings()
		{
			Databases = new ObservableCollection<DatabaseViewModel>();
			SelectedDatabase = new DatabaseViewModel();
			SelectedCollection = new CollectionViewModel();
			AvailableProperties = new ObservableCollection<string>();
						Graph = new Graph();
	
			Save = new DelegateCommand();
			Refresh = new DelegateCommand();
			Test = new DelegateCommand();
			Cancel = new DelegateCommand();
		
			ApplyDefaultConventions();
		}
	}
}
