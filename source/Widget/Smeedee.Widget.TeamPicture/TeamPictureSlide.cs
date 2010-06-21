using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.TeamPicture.ViewModel;
using Smeedee.Widget.TeamPicture.Views;

namespace Smeedee.Widget.TeamPicture
{
    [Export(typeof(Slide))]
    public class TeamPictureSlide : Slide
    {
        private TeamPictureViewModel _dataContext;

        public TeamPictureSlide()
        {
            Title = "Team Picture";
            _dataContext = new TeamPictureViewModel();
            SettingsView = new TeamPictureSettingsView()
            {
                DataContext = _dataContext
            };

            View = new TeamPictureView()
            {
                DataContext = _dataContext
            };

            this.PropertyChanged += TeamPictureSlide_PropertyChanged;

        }

        void TeamPictureSlide_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if( e.PropertyName.Equals("IsInSettingsMode"))
            {
                if (IsInSettingsMode == false && _dataContext.CaptureState == CaptureState.Started)
                    _dataContext.ToggleWebcamOnOff.Execute(null);
                if (IsInSettingsMode == true && _dataContext.CaptureState == CaptureState.Stopped)
                    _dataContext.ToggleWebcamOnOff.Execute(null);
                
            }
        }
    }
}
