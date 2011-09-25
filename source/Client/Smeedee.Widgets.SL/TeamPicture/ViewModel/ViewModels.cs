
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Conventions;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;
namespace Smeedee.Widgets.SL.TeamPicture.ViewModel
{
	public partial class TeamPictureViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public virtual ObservableCollection<WriteableBitmap> Snapshots
		{
			get 
			{
				OnGetSnapshots(ref _Snapshots);
				 
				return _Snapshots; 
			}
			set
			{
				if (value != _Snapshots)
				{
					OnSetSnapshots(ref value); 
					_Snapshots = value;
					TriggerPropertyChanged("Snapshots");
				}
			}
		}
		private ObservableCollection<WriteableBitmap> _Snapshots;

		partial void OnGetSnapshots(ref ObservableCollection<WriteableBitmap> value);
		partial void OnSetSnapshots(ref ObservableCollection<WriteableBitmap> value);

		public virtual WriteableBitmap SelectedSnapshot
		{
			get 
			{
				OnGetSelectedSnapshot(ref _SelectedSnapshot);
				 
				return _SelectedSnapshot; 
			}
			set
			{
				if (value != _SelectedSnapshot)
				{
					OnSetSelectedSnapshot(ref value); 
					_SelectedSnapshot = value;
					TriggerPropertyChanged("SelectedSnapshot");
				}
			}
		}
		private WriteableBitmap _SelectedSnapshot;

		partial void OnGetSelectedSnapshot(ref WriteableBitmap value);
		partial void OnSetSelectedSnapshot(ref WriteableBitmap value);

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

		public virtual bool HasSelectedSnapshot
		{
			get 
			{
				OnGetHasSelectedSnapshot(ref _HasSelectedSnapshot);
				 
				return _HasSelectedSnapshot; 
			}
			set
			{
				if (value != _HasSelectedSnapshot)
				{
					OnSetHasSelectedSnapshot(ref value); 
					_HasSelectedSnapshot = value;
					TriggerPropertyChanged("HasSelectedSnapshot");
				}
			}
		}
		private bool _HasSelectedSnapshot;

		partial void OnGetHasSelectedSnapshot(ref bool value);
		partial void OnSetHasSelectedSnapshot(ref bool value);

		public virtual string Message
		{
			get 
			{
				OnGetMessage(ref _Message);
				 
				return _Message; 
			}
			set
			{
				if (value != _Message)
				{
					OnSetMessage(ref value); 
					_Message = value;
					TriggerPropertyChanged("Message");
				}
			}
		}
		private string _Message;

		partial void OnGetMessage(ref string value);
		partial void OnSetMessage(ref string value);

		public virtual string ErrorMsg
		{
			get 
			{
				OnGetErrorMsg(ref _ErrorMsg);
				 
				return _ErrorMsg; 
			}
			set
			{
				if (value != _ErrorMsg)
				{
					OnSetErrorMsg(ref value); 
					_ErrorMsg = value;
					TriggerPropertyChanged("ErrorMsg");
				}
			}
		}
		private string _ErrorMsg;

		partial void OnGetErrorMsg(ref string value);
		partial void OnSetErrorMsg(ref string value);

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

		public virtual Brush CaptureBrush
		{
			get 
			{
				OnGetCaptureBrush(ref _CaptureBrush);
				 
				return _CaptureBrush; 
			}
			set
			{
				if (value != _CaptureBrush)
				{
					OnSetCaptureBrush(ref value); 
					_CaptureBrush = value;
					TriggerPropertyChanged("CaptureBrush");
				}
			}
		}
		private Brush _CaptureBrush;

		partial void OnGetCaptureBrush(ref Brush value);
		partial void OnSetCaptureBrush(ref Brush value);

	
		
		//Commands
		public DelegateCommand Delete { get; set; }
		public DelegateCommand Save { get; set; }
		public DelegateCommand TakePicture { get; set; }
		public DelegateCommand ToggleWebcamOnOff { get; set; }
		public DelegateCommand Refresh { get; set; }
		
		public TeamPictureViewModel()
		{
			Delete = new DelegateCommand();
			Save = new DelegateCommand();
			TakePicture = new DelegateCommand();
			ToggleWebcamOnOff = new DelegateCommand();
			Refresh = new DelegateCommand();
	
			OnInitialize();
			ApplyConvention(new BindCommandsDelegatesToMethods());
		}

		partial void OnInitialize();
	}
}

