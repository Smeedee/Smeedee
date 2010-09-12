

using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using System;
using System.Collections.ObjectModel;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.ViewModel
{
	public partial class CollectionViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public string Name { get; set; } 
	
		
		//Commands
		
		public CollectionViewModel()
		{
		
		
			ApplyDefaultConventions();
		}
	}
}
