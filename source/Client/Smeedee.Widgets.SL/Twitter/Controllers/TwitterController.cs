using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.Framework;
using Smeedee.Widgets.SL.Twitter.ViewModel;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.SL.Twitter.Controllers
{
    public class TwitterController : ControllerBase<TwitterViewModel>
    {
        public const string TWITTER_CONFIG_NAME = "twitter-config";
        public const string SETTING_SEARCH_STRING = "search-string";
        public const string SETTING_MAX_NUMBER_OF_TWEETS = "max-number-of-tweets";
        public const string SETTING_REFRESH_INTERVAL_SECONDS = "refresh-interval-seconds";

        public const string DEFAULT_SEARCH_STRING = "Search";
        public const int DEFAULT_MAX_NUMBER_OF_TWEETS = 7;
        public static TimeSpan DEFAULT_REFRESH_INTERVAL = new TimeSpan(0, 5, 0);

        private readonly IFetchTweets tweetFetcher;
        private readonly TwitterSettingsViewModel settingsViewModel;
        private Configuration currentConfiguration;
        private bool redoGetTweets;

        public TwitterController(TwitterViewModel viewModel,
                                 TwitterSettingsViewModel settingsViewModel,
                                 Configuration configuration,
                                 ITimer timer,
                                 IUIInvoker uiInvoke,
                                 IFetchTweets tweetFetcher,
                                 IProgressbar progressbar)
            : base(viewModel, timer, uiInvoke, progressbar)
        {
            ThrowIfAnyArgumentIsNull(viewModel, settingsViewModel, tweetFetcher, configuration);

            this.tweetFetcher = tweetFetcher;
            this.settingsViewModel = settingsViewModel;
            currentConfiguration = configuration;

            this.tweetFetcher.GetTweetsCompleted += GetTweetsCompleted;

            settingsViewModel.Search.ExecuteDelegate = () => Search();
            settingsViewModel.ReloadSettings.ExecuteDelegate = () => SetViewModelFromConfiguration(currentConfiguration);


            UpdateWidgetFromConfiguration(currentConfiguration);

            BeginGetTweets();    
        }

        private void UpdateWidgetFromConfiguration(Configuration configuration)
        {
            SetViewModelFromConfiguration(configuration);
            refreshNotifier.Stop();
            refreshNotifier.Start((int) settingsViewModel.RefreshInterval.TotalMilliseconds);
        }

        private void SetViewModelFromConfiguration(Configuration configuration)
        {
            settingsViewModel.SearchString = configuration.GetSetting(SETTING_SEARCH_STRING).Value;
            settingsViewModel.NumberOfTweetsToDisplay = int.Parse(configuration.GetSetting(SETTING_MAX_NUMBER_OF_TWEETS).Value);
            settingsViewModel.RefreshInterval = new TimeSpan(0, 0, int.Parse(configuration.GetSetting(SETTING_REFRESH_INTERVAL_SECONDS).Value));
            settingsViewModel.HasChanges = false;
        }

        private void BeginGetTweets()
        {
            if (settingsViewModel.IsLoading)
            {
                redoGetTweets = true; //do a new search when the current one is completed
                return;
            }
            redoGetTweets = false;

            SetIsUpdating(true);
            tweetFetcher.BeginGetTweets(settingsViewModel.SearchString, settingsViewModel.NumberOfTweetsToDisplay);
        }

        private void SetIsUpdating(bool isUpdating)
        {
            settingsViewModel.IsLoading = isUpdating;
            if( isUpdating )
                SetIsLoadingData();
            else
                SetIsNotLoadingData();

        }

        private void GetTweetsCompleted(object sender, AsyncResponseEventArgs<TwitterSearchResult> e)
        {
            ViewModel.Error = e.ReturnValue.Error;
            ViewModel.ErrorMessage = e.ReturnValue.ErrorMessage;

            if (!ViewModel.Error)
            {
                ViewModel.Data = new ObservableCollection<TweetViewModel>(e.ReturnValue.TweetList);
            }
            
            SetIsUpdating(false);

            if (redoGetTweets)
                BeginGetTweets();
        }

        

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            BeginGetTweets();
        }

        private void Search()
        {
            BeginGetTweets();
        }

        public static Configuration GetDefaultConfiguration()
        {
            return MakeConfig(DEFAULT_SEARCH_STRING, DEFAULT_MAX_NUMBER_OF_TWEETS, DEFAULT_REFRESH_INTERVAL);
        }

        public Configuration GetConfigurationFromViewModel()
        {
            return MakeConfig(settingsViewModel.SearchString, settingsViewModel.NumberOfTweetsToDisplay, settingsViewModel.RefreshInterval);
        }

        private static Configuration MakeConfig(string searchString, int maxNumberOfTweets, TimeSpan refreshInterval)
        {
            var config = new Configuration(TWITTER_CONFIG_NAME);
            config.NewSetting(SETTING_SEARCH_STRING, searchString);
            config.NewSetting(SETTING_MAX_NUMBER_OF_TWEETS, maxNumberOfTweets.ToString());
            config.NewSetting(SETTING_REFRESH_INTERVAL_SECONDS, ((int) refreshInterval.TotalSeconds).ToString());
            config.IsConfigured = true;
            return config;
        }

        private void ThrowIfAnyArgumentIsNull(TwitterViewModel viewModel, TwitterSettingsViewModel settingsViewModel, IFetchTweets tweetFetcher, Configuration configuration)
        {
            Guard.ThrowExceptionIfNull(viewModel, "viewModel");
            Guard.ThrowExceptionIfNull(settingsViewModel, "settingsViewModel");
            Guard.ThrowExceptionIfNull(tweetFetcher, "tweetFetcher");
            Guard.ThrowExceptionIfNull(configuration, "configuration");
        }

        public override void ToggleRefreshInSettingsMode(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsInSettingsMode" && sender is Client.Framework.ViewModel.Widget)
            {
                if (((Client.Framework.ViewModel.Widget)sender).IsInSettingsMode)
                    Stop();
                else
                    refreshNotifier.Start((int) settingsViewModel.RefreshInterval.TotalMilliseconds);
            }
        }

        public void ConfigurationChanged(Configuration newConfiguration)
        {
            currentConfiguration = newConfiguration;
            SetViewModelFromConfiguration(newConfiguration);
            BeginGetTweets();
        }

        public void BeforeSaving(object sender, EventArgs e)
        {
            currentConfiguration.ChangeSetting(SETTING_MAX_NUMBER_OF_TWEETS, settingsViewModel.NumberOfTweetsToDisplay.ToString());
            currentConfiguration.ChangeSetting(SETTING_REFRESH_INTERVAL_SECONDS, settingsViewModel.RefreshInterval.TotalSeconds.ToString());
            currentConfiguration.ChangeSetting(SETTING_SEARCH_STRING, settingsViewModel.SearchString);
        }

        public void AfterSave(object sender, EventArgs e)
        {
            settingsViewModel.HasChanges = false;
            UpdateWidgetFromConfiguration(currentConfiguration);
            BeginGetTweets();
        }
    }
}
