using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.TeamPicture;
using Smeedee.Widget.TeamPicture.Services;

namespace Smeedee.Widget.TeamPicture.ViewModel
{
    public partial class TeamPictureViewModel
    {
        private const int REFRESH_INTERVAL = 1000 * 10;
        private const string LOADING_DATA_MESSAGE = "Loading picture from server...";
        private const string SAVING_CONFIG_MESSAGE = "Saving...";

        private readonly IRepository<DomainModel.TeamPicture.TeamPicture> _teamPictureRepository;
        private readonly IPersistDomainModelsAsync<DomainModel.TeamPicture.TeamPicture> _teamPicturePersister;
        private readonly ITimer _refreshTimer;
        private readonly IInvokeBackgroundWorker<IEnumerable<DomainModel.TeamPicture.TeamPicture>> _backgroundWorker;
        private readonly ILog _log;
    	private readonly IWebcamService _webcamService;
        private readonly IProgressbar _progressbarService;
        
        public TeamPictureViewModel(
            IRepository<DomainModel.TeamPicture.TeamPicture> teamPictureRepository, 
            IPersistDomainModelsAsync<DomainModel.TeamPicture.TeamPicture> teamPicturePersister, 
            ITimer refreshTimer, 
            IInvokeBackgroundWorker<IEnumerable<DomainModel.TeamPicture.TeamPicture>> backgroundWorker, 
            ILog log, 
            IWebcamService webcam, 
            IProgressbar progressbarService)
            : this()
        {
            _teamPictureRepository = teamPictureRepository;
            _teamPicturePersister = teamPicturePersister;
            _refreshTimer = refreshTimer;
            _backgroundWorker = backgroundWorker;
            _log = log;
        	_webcamService = webcam;
        	_webcamService.CaptureImageCompleted += ImageCapturedFromWebCam;
            _refreshTimer.Start(REFRESH_INTERVAL);
            _progressbarService = progressbarService;

            _refreshTimer.Elapsed += _refreshTimer_Elapsed;
            _teamPicturePersister.SaveCompleted += ConfigPersisterSaveCompleted;

            LoadDataFromDatabaseIntoViewModel();
        }

        private void ConfigPersisterSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSaving();
        }

        private void SetIsNotSaving()
        {
            IsSaving = false;
            _progressbarService.HideInSettingsView();
        }

        private bool isWebCamOn;

    	public bool IsWebcamOn
    	{
            get
            {
                var state = _webcamService.CaptureState == CaptureState.Started;
                if(state != isWebCamOn)
                {
                    isWebCamOn = state;
                    TriggerPropertyChanged("IsWebcamOn");
                }
                return state;
            }
    	}

        void _refreshTimer_Elapsed(object sender, EventArgs e)
        {
            LoadDataFromDatabaseIntoViewModel();
        }

        private void LoadDataFromDatabaseIntoViewModel()
        {
			_backgroundWorker.RunAsyncVoid(() =>
			{
                SetIsLoadingData();
				try
				{
					var teamPictures = _teamPictureRepository.Get(new CurrentTeamPictureSpecification());
					UpdateViewModel(teamPictures);
				}
				catch (Exception e)
				{
					_log.WriteEntry(new ErrorLogEntry("TeamPictureWidget", e.ToString()));
				}
                SetIsNotLoadingData();
			});
        }

        private void SetIsLoadingData()
        {
            IsLoading = true;
            _progressbarService.ShowInBothViews(LOADING_DATA_MESSAGE);
        }

        private void SetIsNotLoadingData()
        {
            IsLoading = false;
            _progressbarService.HideInBothViews();
        }

        private void UpdateViewModel(IEnumerable<DomainModel.TeamPicture.TeamPicture> teamPictures)
        {
            if (teamPictures.Count() > 0)
            {
                var teamPicture = teamPictures.First();
                UIInvoker.Invoke(() =>
                {
                    var imageFromDb = WriteableBitmapHelper.FromByteArray(teamPicture.Picture, teamPicture.PictureWidth, teamPicture.PictureHeight);
                    SetSnapshot(imageFromDb);
                    Message = teamPicture.Message;
                    Snapshots.Clear();
                    Snapshots.Add(imageFromDb);
                    SelectedSnapshot = imageFromDb;
                    DisplaySelectedSnapshotInCaptureBrush();
                });
            }
            else
            {
                UIInvoker.Invoke(() =>
                                     {
                                         Message = "";
                                         HasStoredImage = false;
                                     });
            }
        }

        private void SetSnapshot(WriteableBitmap imageFromDb)
        {
            Snapshot = imageFromDb;
            HasStoredImage = imageFromDb != null;
        }

        public void OnInitialize()
        {
            Snapshots = new ObservableCollection<WriteableBitmap>();
            CaptureBrush = new SolidColorBrush(Colors.Black);
            PropertyChanged += TeamPictureViewModel_PropertyChanged;
        }

        void TeamPictureViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSnapshot")
            {
                Save.TriggerCanExecuteChanged();
                Delete.TriggerCanExecuteChanged();
                HasSelectedSnapshot = SelectedSnapshot != null;
            	DisplaySelectedSnapshotInCaptureBrush();
				StopWebcamCapture();
				TakePicture.TriggerCanExecuteChanged();
            }
        }

        public void OnDelete()
        {
        	var index = Snapshots.IndexOf(SelectedSnapshot);
            Snapshots.Remove(SelectedSnapshot);
			if (Snapshots.Count > 0)
				SelectedSnapshot = Snapshots[index - 1];
        }

        public bool CanDelete()
        {
            return SelectedSnapshot != null;
        }

        public bool CanTakePicture()
        {
            return IsWebcamOn;
        }

        public void OnSave()
        {
            SetIsSaving();
            UIInvoker.Invoke(() => SetSnapshot(SelectedSnapshot));
            PersistTeamPictureToDatabase();
        }

        public void OnRefresh()
        {
            LoadDataFromDatabaseIntoViewModel();
        }

        private void PersistTeamPictureToDatabase()
        {
            try
            {
                var teamPicture = new DomainModel.TeamPicture.TeamPicture();
                teamPicture.Message = Message;
                teamPicture.Picture = WriteableBitmapHelper.ToByteArray(SelectedSnapshot.Pixels);
                teamPicture.PictureHeight = SelectedSnapshot.PixelHeight;
                teamPicture.PictureWidth = SelectedSnapshot.PixelWidth;
                _teamPicturePersister.Save(teamPicture);
            }
            catch(Exception e)
            {
                _log.WriteEntry(new ErrorLogEntry("TeamPictureWidget", e.ToString()));
            }

        }

        private void SetIsSaving()
        {
            IsSaving = true;
            _progressbarService.ShowInSettingsView(SAVING_CONFIG_MESSAGE);
        }

        public bool CanSave()
        {
            return SelectedSnapshot != null;
        }

        public void OnTakePicture()
        {
            if (IsWebcamOn)
            {
				_webcamService.CaptureImageAsync();
            }
        }

		private void ImageCapturedFromWebCam(object sender, CaptureImageCompletedEventArgs e)
		{
			Snapshots.Add(e.Result);
			SelectedSnapshot = e.Result;
			TakePicture.TriggerCanExecuteChanged();
		}

        public void OnToggleWebcamOnOff()
        {
            if (IsWebcamOn)
            {
				StopWebcamCapture();

				if (Snapshots.Count > 0)
				{
					SelectedSnapshot = Snapshots.Last();
					DisplaySelectedSnapshotInCaptureBrush();
				}

				TakePicture.TriggerCanExecuteChanged();
            }
            else if(!IsWebcamOn)
            {
				StartWebcamCapture();
            }
            else
            {
                _log.WriteEntry(new ErrorLogEntry("TeamPictureWidget", "The webcam is in a failed state. Cannot turn on/off"));
            }
        }

        private void StartWebcamCapture()
        {
            if (_webcamService.RequestAccess())
            {
				TriggerPropertyChanged("IsWebcamOn");
                SetVideoBrushAndStartCapture();
            }
        }

        private void SetVideoBrushAndStartCapture()
        {
            var videoBrush = new VideoBrush();
            videoBrush.Stretch = Stretch.UniformToFill;
            CaptureBrush = videoBrush;
            try
            {
                _webcamService.StartCapture(videoBrush);
                TakePicture.TriggerCanExecuteChanged();
                ErrorMsg = "";
            }
            catch (Exception e)
            {
                _log.WriteEntry(new ErrorLogEntry("TeamPictureViewModel", e.ToString()));
                ErrorMsg = "Camera is not working properly!";
            }

        }

        private void StopWebcamCapture()
        {
        	_webcamService.StopCapture();
			
			if (Snapshots.Count == 0)
				CaptureBrush = new SolidColorBrush(Colors.Black);

			TriggerPropertyChanged("IsWebcamOn");
		}

    	private void DisplaySelectedSnapshotInCaptureBrush()
    	{
    		CaptureBrush = new ImageBrush() { ImageSource = SelectedSnapshot, Stretch = Stretch.UniformToFill };
    	}

        public void StopRefreshTimer()
        {
            _refreshTimer.Stop();
        }

        public void StartRefreshTimer()
        {
            _refreshTimer.Start(REFRESH_INTERVAL);
        }
        
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (value != isLoading)
                {
                    isLoading = value;
                    TriggerPropertyChanged("IsLoading");
                }
            }
        }
        private bool isLoading;

        public bool IsSaving
        {
            get { return isSaving; }
            set
            {
                if (value != isSaving)
                {
                    isSaving = value;
                    TriggerPropertyChanged("IsSaving");
                }
            }
        }
        private bool isSaving;
    }
}
