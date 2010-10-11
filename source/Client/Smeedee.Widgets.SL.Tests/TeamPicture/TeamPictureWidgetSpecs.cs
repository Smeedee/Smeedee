using System.Threading;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Widgets.SL.TeamPicture;
using Smeedee.Widgets.SL.TeamPicture.ViewModel;

namespace Smeedee.Widgets.SL.Tests.TeamPicture
{
    [TestClass]
    public class TeamPictureWidgetSpecs : Shared
    {
        [TestMethod]
        [Ignore]
        public void Assure_closing_settingsView_stops_webcam()
        {
            //TODO: This test should be working, but we are asserting on GUI-elements that cannot be accessed cia a background thread
            TeamPictureWidget slide = new TeamPictureWidget();
            var teamPictureViewModel = slide.SettingsView.DataContext as TeamPictureViewModel;

            var dispatcher = new TextBlock().Dispatcher;
            slide.Settings.Execute(null);

            Thread.Sleep(2000);

            Assert.IsTrue(teamPictureViewModel.IsWebcamOn);

            slide.Settings.Execute(null);
            Assert.IsTrue(teamPictureViewModel.IsWebcamOn);
        }
    }
}
