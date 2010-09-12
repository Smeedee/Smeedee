
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Smeedee.Widget.TeamPicture.ViewModel
{
	public partial class TeamPictureViewModel : TinyMVVM.Framework.ViewModelBase
	{
		//State
		public ObservableCollection<WriteableBitmap> Snapshots
		{
			get { return _Snapshots; }
			set
			{
				if (value != _Snapshots)
				{
					_Snapshots = value;
					TriggerPropertyChanged("Snapshots");
				}
			}
		}
		private ObservableCollection<WriteableBitmap> _Snapshots;

		public WriteableBitmap SelectedSnapshot
		{
			get { return _SelectedSnapshot; }
			set
			{
				if (value != _SelectedSnapshot)
				{
					_SelectedSnapshot = value;
					TriggerPropertyChanged("SelectedSnapshot");
				}
			}
		}
		private WriteableBitmap _SelectedSnapshot;

		public WriteableBitmap Snapshot
		{
			get { return _Snapshot; }
			set
			{
				if (value != _Snapshot)
				{
					_Snapshot = value;
					TriggerPropertyChanged("Snapshot");
				}
			}
		}
		private WriteableBitmap _Snapshot;

		public bool HasSelectedSnapshot
		{
			get { return _HasSelectedSnapshot; }
			set
			{
				if (value != _HasSelectedSnapshot)
				{
					_HasSelectedSnapshot = value;
					TriggerPropertyChanged("HasSelectedSnapshot");
				}
			}
		}
		private bool _HasSelectedSnapshot;

		public string Message
		{
			get { return _Message; }
			set
			{
				if (value != _Message)
				{
					_Message = value;
					TriggerPropertyChanged("Message");
				}
			}
		}
		private string _Message;

		public string ErrorMsg
		{
			get { return _ErrorMsg; }
			set
			{
				if (value != _ErrorMsg)
				{
					_ErrorMsg = value;
					TriggerPropertyChanged("ErrorMsg");
				}
			}
		}
		private string _ErrorMsg;

		public bool HasStoredImage
		{
			get { return _HasStoredImage; }
			set
			{
				if (value != _HasStoredImage)
				{
					_HasStoredImage = value;
					TriggerPropertyChanged("HasStoredImage");
				}
			}
		}
		private bool _HasStoredImage;

		public Brush CaptureBrush
		{
			get { return _CaptureBrush; }
			set
			{
				if (value != _CaptureBrush)
				{
					_CaptureBrush = value;
					TriggerPropertyChanged("CaptureBrush");
				}
			}
		}
		private Brush _CaptureBrush;

	
		
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
	
			ApplyDefaultConventions();
		}
	}
		
}