
using System.Windows.Media;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace Smeedee.Widget.TeamPicture.ViewModel
{
	public partial class TeamPictureViewModel : TinyMVVM.Framework.ViewModelBase
	{
		protected IUIInvoker UIInvoker { get; set; }

		//State
		public ObservableCollection<WriteableBitmap> Snapshots
		{
			get { return _Snapshots; }
			set
			{
				if (value != _Snapshots)
				{
					UIInvoker.Invoke(() =>
					{
						_Snapshots = value;
						TriggerPropertyChanged("Snapshots");
					});
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
					UIInvoker.Invoke(() =>
					{
						_SelectedSnapshot = value;
						TriggerPropertyChanged("SelectedSnapshot");
					});
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
					UIInvoker.Invoke(() =>
					{
						_Snapshot = value;
						TriggerPropertyChanged("Snapshot");
					});
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
					UIInvoker.Invoke(() =>
					{
						_HasSelectedSnapshot = value;
						TriggerPropertyChanged("HasSelectedSnapshot");
					});
				}
			}
		}
		private bool _HasSelectedSnapshot;

		public Brush WebcamVideoBrush
		{
			get { return _WebcamVideoBrush; }
			set
			{
				if (value != _WebcamVideoBrush)
				{
					UIInvoker.Invoke(() =>
					{
						_WebcamVideoBrush = value;
						TriggerPropertyChanged("WebcamVideoBrush");
					});
				}
			}
		}
		private Brush _WebcamVideoBrush;

		public string Message
		{
			get { return _Message; }
			set
			{
				if (value != _Message)
				{
					UIInvoker.Invoke(() =>
					{
						_Message = value;
						TriggerPropertyChanged("Message");
					});
				}
			}
		}
		private string _Message;

	
		
		//Commands
		public DelegateCommand Delete { get; set; }
		public DelegateCommand Save { get; set; }
		public DelegateCommand TakePicture { get; set; }
		public DelegateCommand ToggleWebcamOnOff { get; set; }
		
		public TeamPictureViewModel()
		{
			Delete = new DelegateCommand();
			Save = new DelegateCommand();
			TakePicture = new DelegateCommand();
			ToggleWebcamOnOff = new DelegateCommand();
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
}