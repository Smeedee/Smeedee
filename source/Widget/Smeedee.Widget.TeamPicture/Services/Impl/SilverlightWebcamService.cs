using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Widget.TeamPicture.Services.Impl
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
