using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Widget.TeamPicture.ViewModel;

namespace Smeedee.Widget.TeamPicture.Tests
{
    [TestClass]
    public class TeamPictureSlideSpecs : Shared
    {
        [TestMethod]
        [Ignore]
        public void Assure_closing_settingsView_stops_webcam()
        {
            //TODO: This test should be working, but we are asserting on GUI-elements that cannot be accessed cia a background thread
            TeamPictureSlide slide = new TeamPictureSlide();
            var teamPictureViewModel = slide.SettingsView.DataContext as TeamPictureViewModel;

            var dispatcher = new TextBlock().Dispatcher;
            slide.Settings.Execute(null);

            Thread.Sleep(2000);

            Assert.AreEqual(CaptureState.Started, teamPictureViewModel.CaptureState);

            slide.Settings.Execute(null);
            Assert.AreEqual(CaptureState.Stopped, teamPictureViewModel.CaptureState );
        }
    }
}
