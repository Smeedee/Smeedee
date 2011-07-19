
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using TinyMVVM.Framework;
using Smeedee.Client.Framework.ViewModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
	public partial class WebSnapshotSettingsViewModel : SettingsViewModelBase
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

		public virtual string Xpath
		{
			get 
			{
				OnGetXpath(ref _Xpath);
				 
				return _Xpath; 
			}
			set
			{
				if (value != _Xpath)
				{
					OnSetXpath(ref value); 
					_Xpath = value;
					TriggerPropertyChanged("Xpath");
				}
			}
		}
		private string _Xpath;

		partial void OnGetXpath(ref string value);
		partial void OnSetXpath(ref string value);

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
		public DelegateCommand FetchAsImage { get; set; }
		public DelegateCommand FetchAsSnapshot { get; set; }
		public DelegateCommand Save { get; set; }
		public DelegateCommand ReloadSettings { get; set; }
		
		public WebSnapshotSettingsViewModel()
		{
			FetchAsImage = new DelegateCommand();
			FetchAsSnapshot = new DelegateCommand();
			Save = new DelegateCommand();
			ReloadSettings = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
	public partial class WebSnapshotViewModel : AbstractViewModel
	{
		//State
		public virtual WriteableBitmap Snapshot
		{
			get 
			{
				OnGetSnapshot(ref _Snapshot);
				 
				return _Snapshot; 
			}
			set
			{
				if (value != _Snapshot)
				{
					OnSetSnapshot(ref value); 
					_Snapshot = value;
					TriggerPropertyChanged("Snapshot");
				}
			}
		}
		private WriteableBitmap _Snapshot;

		partial void OnGetSnapshot(ref WriteableBitmap value);
		partial void OnSetSnapshot(ref WriteableBitmap value);

		public virtual bool HasStoredImage
		{
			get 
			{
				OnGetHasStoredImage(ref _HasStoredImage);
				 
				return _HasStoredImage; 
			}
			set
			{
				if (value != _HasStoredImage)
				{
					OnSetHasStoredImage(ref value); 
					_HasStoredImage = value;
					TriggerPropertyChanged("HasStoredImage");
				}
			}
		}
		private bool _HasStoredImage;

		partial void OnGetHasStoredImage(ref bool value);
		partial void OnSetHasStoredImage(ref bool value);

	
		
		//Commands
		
		public WebSnapshotViewModel()
		{
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

