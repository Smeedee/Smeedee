

using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using System;
using System.Collections.ObjectModel;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.ViewModel
{
	public partial class Graph : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public ObservableCollection<DataPoint> Data { get; set; } 
	
		
		//Commands
		public DelegateCommand Refresh { get; set; }
		
		public Graph()
		{
			Data = new ObservableCollection<DataPoint>();
	
			Refresh = new DelegateCommand();
		
			ApplyDefaultConventions();
		}
	}
}
