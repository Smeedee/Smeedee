
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using TinyMVVM.Framework;
namespace Smeedee.Widgets.WebPage.ViewModel
{
	public partial class WebPageViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual string InputUrl
		{
			get 
			{
				OnGetInputUrl(ref _InputUrl);
				 
				return _InputUrl; 
			}
			set
			{
				if (value != _InputUrl)
				{
					OnSetInputUrl(ref value); 
					_InputUrl = value;
					TriggerPropertyChanged("InputUrl");
				}
			}
		}
		private string _InputUrl;

		partial void OnGetInputUrl(ref string value);
		partial void OnSetInputUrl(ref string value);

		public virtual string ValidatedUrl
		{
			get 
			{
				OnGetValidatedUrl(ref _ValidatedUrl);
				 
				return _ValidatedUrl; 
			}
			set
			{
				if (value != _ValidatedUrl)
				{
					OnSetValidatedUrl(ref value); 
					_ValidatedUrl = value;
					TriggerPropertyChanged("ValidatedUrl");
				}
			}
		}
		private string _ValidatedUrl;

		partial void OnGetValidatedUrl(ref string value);
		partial void OnSetValidatedUrl(ref string value);

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

		public virtual string ErrorMessage
		{
			get 
			{
				OnGetErrorMessage(ref _ErrorMessage);
				 
				return _ErrorMessage; 
			}
			set
			{
				if (value != _ErrorMessage)
				{
					OnSetErrorMessage(ref value); 
					_ErrorMessage = value;
					TriggerPropertyChanged("ErrorMessage");
				}
			}
		}
		private string _ErrorMessage;

		partial void OnGetErrorMessage(ref string value);
		partial void OnSetErrorMessage(ref string value);

	
		
		//Commands
		public DelegateCommand Save { get; set; }
		public DelegateCommand ReloadSettings { get; set; }
		
		public WebPageViewModel()
		{
			Save = new DelegateCommand();
			ReloadSettings = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

