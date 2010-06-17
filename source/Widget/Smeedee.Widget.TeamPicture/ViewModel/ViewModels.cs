
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

	
		
		//Commands
		public DelegateCommand Delete { get; set; }
		public DelegateCommand Select { get; set; }
		
		public TeamPictureViewModel()
		{
			Delete = new DelegateCommand();
			Select = new DelegateCommand();
		
			ServiceLocator.SetLocatorIfNotSet(() => ServiceLocator.GetServiceLocator());
			UIInvoker = ServiceLocator.Instance.GetInstance<IUIInvoker>();
		
			ApplyDefaultConventions();
		}
	}
		
}