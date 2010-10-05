using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widgets.SL.Twitter.Controllers;
using Smeedee.Widgets.SL.Twitter.View;
using Smeedee.Widgets.SL.Twitter.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.SL.Twitter
{
    [WidgetInfo(Name = "Twitter",
                Description = "Allows you to display a twitter search",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.TeamCommunication, CommonTags.Fun })]
    public class TwitterWidget : Widget
    {
        public TwitterWidget()
        {
            var viewModel = GetInstance<TwitterViewModel>();
			var settingsViewModel = GetInstance<TwitterSettingsViewModel>();

			var controller = NewController<TwitterController>();
			PropertyChanged += controller.ToggleRefreshInSettingsMode;

            View = new TwitterView { DataContext = viewModel };
            SettingsView = new TwitterSettingsView {DataContext = settingsViewModel};

            Title = "Twitter";

            ConfigurationChanged += (o, e) => controller.ConfigurationChanged(Configuration);

            // Dette kan generaliseres hvis vi har en settingsviewModel på Widget-klassen:
            Settings.AfterExecute += (o, e) =>
            {
                if (IsInSettingsMode == false && settingsViewModel.HasChanges)
                    settingsViewModel.ReloadSettings.Execute();
            };

            SaveSettings.BeforeExecute += controller.BeforeSaving;
            SaveSettings.AfterExecute += controller.AfterSave;

            // Temporary patch
            settingsViewModel.Save.ExecuteDelegate = () =>  SaveSettings.Execute();
        }

        public override void Configure(DependencyConfigSemantics config)
		{
			config.Bind<IFetchTweets>().ToInstance(new TwitterTweetFetcher(new XmlFetcher()));
			config.Bind<TwitterViewModel>().To<TwitterViewModel>().InSingletonScope();
			config.Bind<TwitterSettingsViewModel>().To<TwitterSettingsViewModel>().InSingletonScope();
		}

        protected override Configuration NewConfiguration()
        {
            return TwitterController.GetDefaultConfiguration();
        }
    }
}