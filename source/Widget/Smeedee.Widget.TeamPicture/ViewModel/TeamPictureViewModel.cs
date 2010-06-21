using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Smeedee.Widget.TeamPicture.ViewModel
{
    public partial class TeamPictureViewModel
    {
        private CaptureSource captureSource;
        private VideoCaptureDevice webcam;

        public CaptureState CaptureState 
        { 
            get
            {
                return captureSource.State;
            } 
        }

        public void OnInitialize()
        {
            Snapshots = new ObservableCollection<WriteableBitmap>();
            WebcamVideoBrush = new SolidColorBrush(Colors.Black);
            PropertyChanged += TeamPictureViewModel_PropertyChanged;

            InitializeWebcam();
            
        }

        private void InitializeWebcam()
        {
            captureSource = new CaptureSource();

            captureSource.CaptureImageCompleted += (o, ee) => { Snapshots.Add(ee.Result); };

            webcam = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
        }

        void TeamPictureViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSnapshot")
            {
                Save.TriggerCanExecuteChanged();
                Delete.TriggerCanExecuteChanged();

                HasSelectedSnapshot = SelectedSnapshot != null;
            }
        }

        public void OnDelete()
        {
            Snapshots.Remove(SelectedSnapshot);
        }

        public bool CanDelete()
        {
            return SelectedSnapshot != null;
        }

        public void OnSave()
        {
            Snapshot = SelectedSnapshot;
        }

        public bool CanSave()
        {
            return SelectedSnapshot != null;
        }

        public void OnTakePicture()
        {
            if (captureSource != null)
            {
                captureSource.CaptureImageAsync();
            }
        }


        public void OnToggleWebcamOnOff()
        {
            if (CaptureState == CaptureState.Stopped)
            {
                StartWebcamCapture();
            }
            else if(CaptureState == CaptureState.Started)
            {
                StopWebcamCapture();
            }
            else
            {
                // Log this and notify user. The webcam is in a failed state
            }
        }

        private void StartWebcamCapture()
        {
            if (CaptureDeviceConfiguration.RequestDeviceAccess())
            {
                if (webcam != null)
                {
                    SetVideoBrushAndStartCapture();
                }
            }
        }

        private void SetVideoBrushAndStartCapture()
        {
            captureSource.VideoCaptureDevice = webcam;

            var videoBrush = new VideoBrush();
            videoBrush.SetSource(captureSource);
            videoBrush.Stretch = Stretch.UniformToFill;
            WebcamVideoBrush = videoBrush;

            captureSource.Start();
        }

        private void StopWebcamCapture()
        {
            captureSource.Stop();
            WebcamVideoBrush = new SolidColorBrush(Colors.Black);
        }
    }
}
