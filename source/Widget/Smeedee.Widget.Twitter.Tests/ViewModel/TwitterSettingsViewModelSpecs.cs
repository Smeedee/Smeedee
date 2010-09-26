using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Tests;
using Smeedee.Widgets.SL.Twitter.ViewModel;

namespace Smeedee.Widgets.SL.Twitter.Tests.ViewModel
{
    [TestClass]
    public class TwitterSettingsViewModelSpecs
    {
        [TestClass]
        public class OnPropertyChange : Shared
        {
            [TestMethod]
            public void Assert_should_notify_about_SearchString_change()
            {
                PropertyTester.TestChange(_twitterSettingsViewModel, (t) => t.SearchString);
                Assert.IsTrue(PropertyTester.WasNotified);
            }

            [TestMethod]
            public void Assert_should_notify_about_NumberOfTweets_change()
            {
                PropertyTester.TestChange(_twitterSettingsViewModel, (t) => t.NumberOfTweetsToDisplay);
                Assert.IsTrue(PropertyTester.WasNotified);
            }

            [TestMethod]
            public void Assert_should_notify_about_RefreshIntervalSeconds_change()
            {
                PropertyTester.TestChange(_twitterSettingsViewModel, (t) => t.RefreshInterval);
                Assert.IsTrue(PropertyTester.WasNotified);
            }
        }

        [TestClass]
        public class OnSpawn : Shared
        {
            [TestMethod]
            public void Assert_searchString_is_not_null_when_spawned()
            {
                Assert.IsNotNull(_twitterSettingsViewModel.SearchString);
            } 
        }

        [TestClass]
        public class OnSearchStringTooShort : Shared
        {
            protected override void Before()
            {
                SetTooShortSearchString();
            }

            [TestMethod]
            public void Asset_validation_error_when_search_string_is_too_short()
            {
                Assert.AreEqual(TwitterSettingsViewModel.VALIDATION_ERROR_STRING, _twitterSettingsViewModel["SearchString"]);
            }

            [TestMethod]
            public void Assert_CanSearchOrSave_returnes_false_when_search_string_is_too_short()
            {
                Assert.IsFalse(_twitterSettingsViewModel.CanSearchOrSave());
            }
        }

        [TestClass]
        public class OnSearchStringLongEnough : Shared
        {
            protected override void Before()
            {
                SetLongEnoughSearchString();
            }


            [TestMethod]
            public void Assert_no_validation_error_when_search_string_is_long_enough()
            {
                Assert.IsNull(_twitterSettingsViewModel["SearchString"]);
            }

            [TestMethod]
            public void Assert_CanSearchOrSave_returnes_true_when_search_string_is_long_enough()
            {
                Assert.IsTrue(_twitterSettingsViewModel.CanSearchOrSave());
            }
        }

        [TestClass]
        public class WhileUpdating : Shared
        {
            protected override void Before()
            {
                _twitterSettingsViewModel.IsLoading = true;
            }

            [TestMethod]
            public void Assert_CanSearchOrSave_returns_false_while_updating()
            {
                Assert.IsFalse(_twitterSettingsViewModel.CanSearchOrSave());
            }
        }

        [TestClass]
        public class Shared
        {
            protected TwitterSettingsViewModel _twitterSettingsViewModel;

            [TestInitialize]
            public void Setup()
            {

                _twitterSettingsViewModel = new TwitterSettingsViewModel(new TwitterViewModel());
                Before();
            }

            protected virtual void Before()
            {
            }

            protected void SetLongEnoughSearchString()
            {
                SetSearchString(TwitterSettingsViewModel.MIN_SEARCH_STRING_LENGTH);
            }

            protected void SetTooShortSearchString()
            {
                SetSearchString(TwitterSettingsViewModel.MIN_SEARCH_STRING_LENGTH - 1);
            }

            private void SetSearchString(int length)
            {
                var s = "";
                for (int i = 0; i < length; i++)
                {
                    s += "A";
                }
                _twitterSettingsViewModel.SearchString = s;
            }
        }
    }
}
