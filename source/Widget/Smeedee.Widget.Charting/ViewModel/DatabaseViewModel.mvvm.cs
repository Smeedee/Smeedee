

using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using System;
using System.Collections.ObjectModel;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.ViewModel
{
	public partial class DatabaseViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public string Name { get; set; } 
		public ObservableCollection<CollectionViewModel> Collections { get; set; } 
	
		
		//Commands
		
		public DatabaseViewModel()
		{
				Collections = new ObservableCollection<CollectionViewModel>();
	
		
			ApplyDefaultConventions();
		}
	}
}
