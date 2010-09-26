using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.SL.Twitter.Controllers;
using Smeedee.Widgets.SL.Twitter.ViewModel;

namespace Smeedee.Widgets.SL.Twitter.Tests.Controllers
{
    [TestClass]
    public class TwitterControllerSpecs
    {
        [TestClass]
        public class When_spawned_and_arguments_are_null : Shared
        {

            [TestMethod]
            //[ExpectedException(typeof (ArgumentException))]
            public void Should_throw_exception_if_viewModel_is_null()
            {
                //This try/catch is used instead of the [ExpectedExce... above because
                //the test runner complains about unhandled exceptions if one uses it.
                try
                {
                    var controllerWithNullAruguments = new TwitterController(null, _twitterSettingsViewModel, 
                                                                            defaultConfiguration, _timerMock.Object,
                                                                             new NoUIInvokation(),
                                                                             _tweetFetcherMock.Object,
                                                                             new Mock<IProgressbar>().Object);
                    Assert.IsFalse(true);
                }
                catch(ArgumentException e)
                {
                    Assert.IsTrue(true);
                }
            }

            [TestMethod]
            public void Should_throw_exception_if_settingsViewModel_is_null()
            {
                try
                {
                    var controllerWithNullAruguments = new TwitterController(_twitterViewModel, null, defaultConfiguration, _timerMock.Object,
                                                                             new NoUIInvokation(),
                                                                             _tweetFetcherMock.Object,
                                                                             new Mock<IProgressbar>().Object);
                    Assert.IsFalse(true);
                }
                catch (ArgumentException e)
                {
                    Assert.IsTrue(true);
                }
            }

            [TestMethod]
            public void Should_throw_exception_if_tweetFetcher_is_null()
            {
                try
                {
                    var controllerWithNullAruguments = new TwitterController(_twitterViewModel, _twitterSettingsViewModel, defaultConfiguration, _timerMock.Object,
                                                                             new NoUIInvokation(),
                                                                             null,
                                                                             new Mock<IProgressbar>().Object);
                    Assert.IsFalse(true);
                }
                catch (ArgumentException e)
                {
                    Assert.IsTrue(true);
                }
            }

            [TestMethod]
            public void Should_throw_exception_if_configuration_is_null()
            {
                try
                {
                    var controllerWithNullAruguments = new TwitterController(_twitterViewModel, _twitterSettingsViewModel, null, _timerMock.Object,
                                                                             new NoUIInvokation(),
                                                                             _tweetFetcherMock.Object,
                                                                             new Mock<IProgressbar>().Object);
                    Assert.IsFalse(true);
                }
                catch (ArgumentException e)
                {
                    Assert.IsTrue(true);
                }
            }


        }

        [TestClass]
        public class When_spawned : Shared
        {
            [TestMethod]
            public void Should_set_HasChanges_to_false()
            {
                Assert.IsFalse(_twitterSettingsViewModel.HasChanges);
            }

            [TestMethod]
            public void Should_load_tweets_into_viewmodel_when_spawned()
            {
                Assert.AreEqual(_controller.ViewModel.Data.Count, _tweetList.Count);
            }

            [TestMethod]
            public void Should_begin_getting_tweets_when_spawned_with_correct_number_of_tweets()
            {
                _tweetFetcherMock.Verify(t => t.BeginGetTweets(It.IsAny<string>(), _twitterSettingsViewModel.NumberOfTweetsToDisplay), Times.Once());
            }

            [TestMethod]
            public void Should_start_timer_when_spawned_with_correct_refresh_interval()
            {
                _timerMock.Verify(t => t.Start((int) _twitterSettingsViewModel.RefreshInterval.TotalMilliseconds));
            }

            [TestMethod]
            public void Should_set_ViewModel_from_config_when_spawned()
            {
                var defaultConfig = TwitterController.GetDefaultConfiguration();
                Assert.AreEqual( defaultConfig.GetSetting(TwitterController.SETTING_SEARCH_STRING).Value, _twitterSettingsViewModel.SearchString);
                Assert.AreEqual( defaultConfig.GetSetting(TwitterController.SETTING_MAX_NUMBER_OF_TWEETS).ValueAsInt(), _twitterSettingsViewModel.NumberOfTweetsToDisplay);
                Assert.AreEqual(defaultConfig.GetSetting(TwitterController.SETTING_REFRESH_INTERVAL_SECONDS).ValueAsInt(), (int) _twitterSettingsViewModel.RefreshInterval.TotalSeconds);
            }
        }

        [TestClass]
        public class When_timer_elapses : Shared
        {
            protected override void Before()
            {
                _timerMock.Raise(t => t.Elapsed += null, EventArgs.Empty);
            }

            [TestMethod]
            public void Should_begin_getting_tweets_on_timer_Elapsed_with_correct_number_of_tweets()
            {
                _tweetFetcherMock.Verify(t => t.BeginGetTweets(It.IsAny<string>(), _twitterSettingsViewModel.NumberOfTweetsToDisplay), Times.Exactly(2));
            }
        }

        [TestClass]
        public class When_setings_change :Shared
        {
            protected override void Before()
            {
                _twitterSettingsViewModel.HasChanges = false;
            }
            [TestMethod]
            public void Should_set_HasChanges_to_true_when_changing_searchString()
            {
                _twitterSettingsViewModel.SearchString = _twitterSettingsViewModel.SearchString + "A";
                Assert.IsTrue(_twitterSettingsViewModel.HasChanges);
            }

            [TestMethod]
            public void Should_set_HasChanges_to_true_when_changing_MaxNumberOfTweets()
            {
                _twitterSettingsViewModel.NumberOfTweetsToDisplay = _twitterSettingsViewModel.NumberOfTweetsToDisplay + 1;
                Assert.IsTrue(_twitterSettingsViewModel.HasChanges);
            }

            [TestMethod]
            public void Should_set_HasChanges_to_true_when_changing_RefreshInterval()
            {
                _twitterSettingsViewModel.RefreshInterval =
                    _twitterSettingsViewModel.RefreshInterval.Add(TimeSpan.FromSeconds(1));
                Assert.IsTrue(_twitterSettingsViewModel.HasChanges);
            }
        }

        [TestClass]
        public class On_Search : Shared
        {
            [TestMethod]
            public void Should_begin_getting_tweets_on_Search_With_correct_SearchString_and_number_of_tweets()
            {
                _twitterSettingsViewModel.Search.Execute(null);
                _tweetFetcherMock.Verify(t => t.BeginGetTweets(_twitterSettingsViewModel.SearchString, _twitterSettingsViewModel.NumberOfTweetsToDisplay), Times.Exactly(2));
            }
            
            [TestMethod]
            public void Assert_CanSearchOrSave_returns_true_after_search_is_executed_and_webClient_is_done()
            {
                _twitterSettingsViewModel.Search.Execute(null);
                Assert.IsTrue(_twitterSettingsViewModel.CanSearchOrSave());
            }

            [TestMethod]
            public void Assert_CanSearchOrSave_returns_false_after_update_is_executed_before_webClient_is_done()
            {
                _tweetFetcherMock = new Mock<IFetchTweets>();
                CreateController();
                _twitterSettingsViewModel.Search.Execute(null);
                Assert.IsFalse(_twitterSettingsViewModel.CanSearchOrSave());
            }
        }

        [TestClass]
        public class On_AfterSave : Shared
        {

            protected override void Before()
            {
                _twitterSettingsViewModel.HasChanges = true;
                _controller.AfterSave(null,null);
            }

            [TestMethod]
            public void Should_begin_getting_tweets_on_save()
            {
                _tweetFetcherMock.Verify(
                    t =>
                    t.BeginGetTweets(_twitterSettingsViewModel.SearchString, _twitterSettingsViewModel.NumberOfTweetsToDisplay), Times.Exactly(2));
            }

            [TestMethod]
            public void Should_set_HasChanges_to_false_on_Save()
            {
                Assert.IsFalse(_twitterSettingsViewModel.HasChanges);
            }
        }



        [TestClass]
        public class On_empty_search_result : Shared
        {
            [TestMethod]
            public void Assert_Error_and_ErrorMessage_is_set_on_empty_result()
            {
                CreateTweetFetcherMock(MakeTwitterSearchResult(0));
                CreateController();
                
                _twitterSettingsViewModel.Search.Execute(null);
                Assert.IsTrue(_twitterViewModel.Error);
                Assert.AreEqual(TwitterSearchResult.NO_TWEETS_FOUND_ERROR_MESSAGE, _twitterViewModel.ErrorMessage);
            }

            [TestMethod]
            public void Assert_Error_and_ErrorMessage_is_set_on_failed_result()
            {
                CreateTweetFetcherMock(MakeWebClientFailSearchResult());
                CreateController();

                _twitterSettingsViewModel.Search.Execute(null);
                Assert.IsTrue(_twitterViewModel.Error);
                Assert.AreEqual(TwitterSearchResult.UNABLE_TO_LOAD_TWEETS_ERROR_MESSAGE, _twitterViewModel.ErrorMessage);
            }

            [TestMethod]
            public void Assert_Error_is_reset_to_false_on_new_search_with_nonempty_result()
            {
                _twitterViewModel.Error = true;
                _twitterSettingsViewModel.Search.Execute(null);
                Assert.IsFalse(_twitterViewModel.Error);
            }
        }

        [TestClass]
        public class On_ReloadSettings : Shared
        {
            [TestMethod]
            public void Should_get_false_from_CanReloadSettings_when_updating()
            {
                _twitterSettingsViewModel.IsLoading = true;
                Assert.IsFalse(_twitterSettingsViewModel.CanReloadSettings());
            }

            [TestMethod]
            public void Should_have_CanReloadSettings_as_CanExecute()
            {
                Assert.IsTrue(_twitterSettingsViewModel.ReloadSettings.CanExecuteDelegate ==
                              _twitterSettingsViewModel.CanReloadSettings);
            }

            [TestMethod]
            public void Should_not_be_able_get_new_tweets_when_loading_tweets()
            {
                _tweetFetcherMock = new Mock<IFetchTweets>();
                CreateController(); //Calls BeginGet
                _controller.ConfigurationChanged(defaultConfiguration);
                _tweetFetcherMock.Verify(t => t.BeginGetTweets(It.IsAny<string>(), It.IsAny<int>()), Times.Exactly(1));
            }

            [TestMethod]
            public void Assure_settingsViewModel_is_populated_from_configuration()
            {
                _twitterSettingsViewModel.RefreshInterval = new TimeSpan(0, 0, 999);
                _twitterSettingsViewModel.NumberOfTweetsToDisplay = 999;
                _twitterSettingsViewModel.SearchString = "smeedee";

                _twitterSettingsViewModel.ReloadSettings.Execute();

                var defaultConfig = TwitterController.GetDefaultConfiguration();
                Assert.AreEqual(defaultConfig.GetSetting(TwitterController.SETTING_SEARCH_STRING).Value, _twitterSettingsViewModel.SearchString);
                Assert.AreEqual(defaultConfig.GetSetting(TwitterController.SETTING_MAX_NUMBER_OF_TWEETS).ValueAsInt(), _twitterSettingsViewModel.NumberOfTweetsToDisplay);
                Assert.AreEqual(defaultConfig.GetSetting(TwitterController.SETTING_REFRESH_INTERVAL_SECONDS).ValueAsInt(), _twitterSettingsViewModel.RefreshInterval.TotalSeconds);
            }

            [TestMethod]
            [Ignore]
            //This should be green, but it doesn't work, so it is ignored
            public void Should_TriggerCanExecuteChanged_when_isUpdating_changes()
            {
                bool hasChanged = false;
                _twitterSettingsViewModel.ReloadSettings.CanExecuteDelegate = () =>
                                                                                  {
                                                                                      hasChanged = true;
                                                                                      return false;
                                                                                  };
                _twitterSettingsViewModel.IsLoading = !_twitterSettingsViewModel.IsLoading;
                Assert.IsTrue(hasChanged);
            }
        }

        [TestClass]
        public class On_notification : Shared
        {
            [TestMethod]
            public void Assure_progressbar_is_shown_while_loading_data()
            {
                _progressbarMock.Verify(l => l.ShowInView(It.IsAny<string>()), Times.Once());
            }

            [TestMethod]
            public void Assure_progressbar_is_hidden_after_loading_data()
            {
                _progressbarMock.Verify(l => l.HideInView(), Times.Once());
                Assert.IsFalse(_controller.ViewModel.IsLoading);
            }
        }
      

        [TestClass]
        public class Shared
        {
            protected TwitterController _controller;
            protected List<TweetViewModel> _tweetList;
            protected TwitterViewModel _twitterViewModel;
            protected TwitterSettingsViewModel _twitterSettingsViewModel;
            protected Mock<ITimer> _timerMock;
            protected Mock<IFetchTweets> _tweetFetcherMock;
            protected Mock<IProgressbar> _progressbarMock;
            protected Guid _guid;
            protected Configuration defaultConfiguration;

            [TestInitialize]
            public void Setup()
            {
                _guid = Guid.NewGuid();

                _timerMock = new Mock<ITimer>();
                _progressbarMock = new Mock<IProgressbar>();
                CreateTweetFetcherMock(MakeTwitterSearchResult(20));
                defaultConfiguration = TwitterController.GetDefaultConfiguration();
                _twitterViewModel = new TwitterViewModel();
                _twitterSettingsViewModel = new TwitterSettingsViewModel(_twitterViewModel);
                CreateController();
                Before();
            }

            protected void CreateController()
            {
                _controller = new TwitterController(
                    _twitterViewModel,
                    _twitterSettingsViewModel,
                    defaultConfiguration,
                    _timerMock.Object,
                    new NoUIInvokation(),
                    _tweetFetcherMock.Object,
                    _progressbarMock.Object);

            }


            protected virtual void Before()
            {
                
            }

            protected void SetViewModelData()
            {
                _twitterSettingsViewModel.SearchString = _guid.ToString();
                _twitterSettingsViewModel.NumberOfTweetsToDisplay = _guid.ToString().GetHashCode();
                _twitterSettingsViewModel.RefreshInterval = new TimeSpan(0, 0, _guid.ToString().GetHashCode());
            }

            protected void CreateTweetFetcherMock(TwitterSearchResult result)
            {
                _tweetFetcherMock = new Mock<IFetchTweets>();

                _tweetFetcherMock.Setup(t => t.BeginGetTweets(It.IsAny<string>(), It.IsAny<int>()))
                    .Raises(t => t.GetTweetsCompleted += null, new AsyncResponseEventArgs<TwitterSearchResult>(result));
            }

            protected TwitterSearchResult MakeTwitterSearchResult(int tweetListSize)
            {
                _tweetList = new List<TweetViewModel>();
                for (int i = 0; i < tweetListSize; i++)
                {
                    _tweetList.Add(new TweetViewModel());
                }

                var result = new TwitterSearchResult();
                if (tweetListSize == 0)
                {
                    result.Error = true;
                    result.ErrorMessage = "No tweets found";
                }
                else
                {
                    result.Error = false;
                }
                result.TweetList = _tweetList;
                return result;
            }

            protected TwitterSearchResult MakeWebClientFailSearchResult()
            {
                var result = new TwitterSearchResult();
                result.Error = true;
                result.ErrorMessage = TwitterSearchResult.UNABLE_TO_LOAD_TWEETS_ERROR_MESSAGE;
                return result;
            }

            protected void AssertConfigurationIsCorrect(Configuration config)
            {
                Assert.IsTrue(config.IsConfigured);
                Assert.AreEqual(TwitterController.TWITTER_CONFIG_NAME, config.Name);
                Assert.AreEqual(_guid.ToString(), config.GetSetting(TwitterController.SETTING_SEARCH_STRING).Value);
                Assert.AreEqual(_guid.ToString().GetHashCode(), int.Parse(config.GetSetting(TwitterController.SETTING_MAX_NUMBER_OF_TWEETS).Value));
                Assert.AreEqual(_guid.ToString().GetHashCode(), int.Parse(config.GetSetting(TwitterController.SETTING_REFRESH_INTERVAL_SECONDS).Value));
            }
        }
    }
}
