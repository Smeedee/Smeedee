using System;
using System.ComponentModel;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.Widget.Twitter.Controllers;
using Smeedee.Widget.Twitter.View;
using Smeedee.Widget.Twitter.ViewModel;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services.Impl;

namespace Smeedee.Widget.Twitter
{
    [WidgetInfo(Name = "Twitter",
                Description = "A simple widget that let you specify a search on twitter and displays the returned tweets!",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.TeamCommunication, CommonTags.Fun })]
    public class TwitterWidget : Client.Framework.ViewModel.Widget
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

            /// Dette kan generaliseres hvis vi har en settingsviewModel på Widget-klassen:
            Settings.AfterExecute += (o, e) =>
            {
                if( IsInSettingsMode == false && settingsViewModel.HasChanges )
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
