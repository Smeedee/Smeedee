
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using TinyMVVM.Framework;
namespace Smeedee.Widgets.WebPage.ViewModel
{
	public partial class WebPageViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string Url
		{
			get 
			{   
				OnGetUrl(ref _Url);
				 
				return _Url; 
			}
			set
			{
				if (value != _Url)
				{
					OnSetUrl(ref value); 
					_Url = value;
					TriggerPropertyChanged("Url");
				}
			}
		}
		private string _Url;

		partial void OnGetUrl(ref string value);
		partial void OnSetUrl(ref string value);

		public virtual int RefreshInterval
		{
			get 
			{
				OnGetRefreshInterval(ref _RefreshInterval);
				 
				return _RefreshInterval; 
			}
			set
			{
				if (value != _RefreshInterval)
				{
					OnSetRefreshInterval(ref value); 
					_RefreshInterval = value;
					TriggerPropertyChanged("RefreshInterval");
				}
			}
		}
		private int _RefreshInterval;

		partial void OnGetRefreshInterval(ref int value);
		partial void OnSetRefreshInterval(ref int value);

	
		
		//Commands
		public DelegateCommand GoTo { get; set; }
		
		public WebPageViewModel()
		{
			GoTo = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

