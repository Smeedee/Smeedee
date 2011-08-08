
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using Smeedee.Client.Framework.ViewModel;
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

		public virtual object LoadedImage
		{
			get 
			{
				OnGetLoadedImage(ref _LoadedImage);
				 
				return _LoadedImage; 
			}
			set
			{
                if (value != _LoadedImage)
                {
					OnSetLoadedImage(ref value); 
					_LoadedImage = value;
					TriggerPropertyChanged("LoadedImage");
                }
			}
		}
		private object _LoadedImage;

		partial void OnGetLoadedImage(ref object value);
		partial void OnSetLoadedImage(ref object value);

		public virtual object Image
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
		private object _Image;

		partial void OnGetImage(ref object value);
		partial void OnSetImage(ref object value);

	
		
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
		public virtual object Snapshot
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
		private object _Snapshot;

		partial void OnGetSnapshot(ref object value);
		partial void OnSetSnapshot(ref object value);

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

