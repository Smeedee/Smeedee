using System;
using System.Windows.Media;

namespace Smeedee.Widgets.SL.TeamPicture.Services
{
	public class SilverlightWebcamService : IWebcamService
	{
		private CaptureSource captureSource;
		private VideoCaptureDevice webcam;

		public SilverlightWebcamService()
		{
			captureSource = new CaptureSource();
			captureSource.CaptureImageCompleted += (o, e) =>
			{
				if (CaptureImageCompleted != null)
					CaptureImageCompleted(this, e);
			};
			webcam = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();
		}

		public CaptureState CaptureState
		{
			get { return captureSource.State; }
		}

		public bool RequestAccess()
		{
			return CaptureDeviceConfiguration.RequestDeviceAccess();
		}

		public void StartCapture(VideoBrush videoBrush)
		{
			videoBrush.SetSource(captureSource);
			captureSource.Start();
		}

		public void StopCapture()
		{
			captureSource.Stop();
		}

		public void CaptureImageAsync()
		{
			captureSource.CaptureImageAsync();
		}

		public event EventHandler<CaptureImageCompletedEventArgs> CaptureImageCompleted;
	}
}
