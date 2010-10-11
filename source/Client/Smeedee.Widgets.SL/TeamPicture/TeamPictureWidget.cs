using System.ComponentModel;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widgets.SL.TeamPicture.Services;
using Smeedee.Widgets.SL.TeamPicture.ViewModel;
using Smeedee.Widget.TeamPicture.Views;
using Smeedee.Widgets.SL.TeamPicture.Views;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.SL.TeamPicture
{
    [WidgetInfo(Name = "Team Picture",
                Description = "Use the webcam to take a picture of your team and add a message to it!",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.Fun, CommonTags.TeamBuilding })]
    public class TeamPictureWidget : Client.Framework.ViewModel.Widget
    {
        private TeamPictureViewModel viewModel;

        public TeamPictureWidget()
        {
            Title = "Team Picture";
            viewModel = GetInstance<TeamPictureViewModel>();
            viewModel.PropertyChanged += ViewModelPropertyChanged;
            viewModel.Save.ExecuteDelegate += viewModel.OnSave;

            SettingsView = new TeamPictureSettingsView { DataContext = viewModel };
            View = new TeamPictureView { DataContext = viewModel };

            PropertyChanged += TeamPictureSlide_PropertyChanged;
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tempViewModel = sender as TeamPictureViewModel;
            var isDoneSaving = (tempViewModel != null && e.PropertyName.Equals("IsSaving") && !tempViewModel.IsSaving);

            if (isDoneSaving && IsInSettingsMode)
            {
                OnSettings();
            }
        }

        public override void Configure(DependencyConfigSemantics config)
		{
			config.Bind<IWebcamService>().To<SilverlightWebcamService>();
			config.Bind<TeamPictureViewModel>().To<TeamPictureViewModel>().InSingletonScope();
		}

        private void TeamPictureSlide_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsInSettingsMode"))
            {
                ToggleRefreshInSettingsMode();
                AutoToggleWebcamOnOffOnSettingsModeChanged();
            }
        }

        private void ToggleRefreshInSettingsMode()
        {
            if (IsInSettingsMode)
            {
                viewModel.StopRefreshTimer();
            }
            else
            {
                viewModel.StartRefreshTimer();
            }
        }

        public void AutoToggleWebcamOnOffOnSettingsModeChanged()
        {
            if (IsInSettingsMode == false && viewModel.IsWebcamOn)
            {
                viewModel.ToggleWebcamOnOff.Execute(null);
            }

            if (IsInSettingsMode == true && viewModel.IsWebcamOn == false)
            {
                viewModel.ToggleWebcamOnOff.Execute(null);
            }
        }
    }
}
