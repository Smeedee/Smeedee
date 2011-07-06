
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
	public partial class ChartViewModelTest : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual bool IsCakeALie
		{
			get 
			{
				OnGetIsCakeALie(ref _IsCakeALie);
				 
				return _IsCakeALie; 
			}
			set
			{
				if (value != _IsCakeALie)
				{
					OnSetIsCakeALie(ref value); 
					_IsCakeALie = value;
					TriggerPropertyChanged("IsCakeALie");
				}
			}
		}
		private bool _IsCakeALie;

		partial void OnGetIsCakeALie(ref bool value);
		partial void OnSetIsCakeALie(ref bool value);

	
		
		//Commands
		
		public ChartViewModelTest()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

