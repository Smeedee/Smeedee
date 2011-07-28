
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using System;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using Smeedee.Client.Framework.ViewModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
namespace Smeedee.Widgets.WebSnapshot.ViewModel
{
	public partial class WebSnapshotSettingsViewModel : SettingsViewModelBase
	{
		//State
		public virtual ObservableCollection<string> AvailableImages
		{
			get 
			{
				OnGetAvailableImages(ref _AvailableImages);
				 
				return _AvailableImages; 
			}
			set
			{
				if (value != _AvailableImages)
				{
					OnSetAvailableImages(ref value); 
					_AvailableImages = value;
					TriggerPropertyChanged("AvailableImages");
				}
			}
		}
		private ObservableCollection<string> _AvailableImages;

		partial void OnGetAvailableImages(ref ObservableCollection<string> value);
		partial void OnSetAvailableImages(ref ObservableCollection<string> value);

		public virtual string SelectedImage
		{
			get 
			{
				OnGetSelectedImage(ref _SelectedImage);
				 
				return _SelectedImage; 
			}
			set
			{
				if (value != _SelectedImage)
				{
					OnSetSelectedImage(ref value); 
					_SelectedImage = value;
					TriggerPropertyChanged("SelectedImage");
				}
			}
		}
		private string _SelectedImage;

		partial void OnGetSelectedImage(ref string value);
		partial void OnSetSelectedImage(ref string value);

		public virtual WriteableBitmap Image
		{
			get 
			{
				OnGetImage(ref _Image);
				 
				return _Image; 
			}
			set
			{
				if (value != _Image)
				{
					OnSetImage(ref value); 
					_Image = value;
					TriggerPropertyChanged("Image");
				}
			}
		}
		private WriteableBitmap _Image;

		partial void OnGetImage(ref WriteableBitmap value);
		partial void OnSetImage(ref WriteableBitmap value);

	
		
		//Commands
		public DelegateCommand Save { get; set; }
		public DelegateCommand ReloadSettings { get; set; }
		public DelegateCommand Reset { get; set; }
		public DelegateCommand Crop { get; set; }
		
		public WebSnapshotSettingsViewModel()
		{
			Save = new DelegateCommand();
			ReloadSettings = new DelegateCommand();
			Reset = new DelegateCommand();
			Crop = new DelegateCommand();
	
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

