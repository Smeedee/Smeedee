using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Widget.TeamPicture.ViewModel;

namespace Smeedee.Widget.TeamPicture.Views
{
    public partial class TeamPictureSettingsView : UserControl
    {
        private VideoCaptureDevice webcam;
        private CaptureSource captureSource;

        private TeamPictureViewModel ViewModel
        {
            get { return DataContext as TeamPictureViewModel; }
        }

        public TeamPictureSettingsView()
        {
            InitializeComponent();
        }

        private void btnTurnOnWebcam_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (captureSource == null)
            {
                captureSource = new CaptureSource();

                captureSource.CaptureImageCompleted += (o, ee) =>
                {
                    ViewModel.Snapshots.Add(ee.Result);
                };

                webcam = CaptureDeviceConfiguration.GetDefaultVideoCaptureDevice();

                if (CaptureDeviceConfiguration.RequestDeviceAccess())
                {
                    if (webcam != null)
                    {
                        captureSource.VideoCaptureDevice = webcam;

                        var videoBrush = new VideoBrush();
                        videoBrush.SetSource(captureSource);
                        videoBrush.Stretch = Stretch.UniformToFill;
                        videoCaptureRectangle.Fill = videoBrush;

                        captureSource.Start();
                    }
                }

            }
        }

        private void btnSnapshot_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (captureSource != null)
            {
                captureSource.CaptureImageAsync();
            }
       }
    }
}
