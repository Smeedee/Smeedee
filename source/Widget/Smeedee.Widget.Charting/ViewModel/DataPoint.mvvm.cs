

using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using System;
using System.Collections.ObjectModel;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.ViewModel
{
	public partial class DataPoint : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public Object X { get; set; } 
		public Object Y { get; set; } 
	
		
		//Commands
		
		public DataPoint()
		{
			X = new Object();
			Y = new Object();
	
		
			ApplyDefaultConventions();
		}
	}
}
