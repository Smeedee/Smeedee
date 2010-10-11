using System;
using System.Windows.Media;

namespace Smeedee.Widgets.SL.TeamPicture.Services
{
	public interface IWebcamService
	{
		CaptureState CaptureState { get; }
		bool RequestAccess();
		void StartCapture(VideoBrush videoBrush);
		void StopCapture();
		void CaptureImageAsync();
		event EventHandler<CaptureImageCompletedEventArgs> CaptureImageCompleted;
	}
}
