using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Tests;
using Smeedee.Widgets.SL.Twitter.ViewModel;

namespace Smeedee.Widgets.SL.Twitter.Tests.ViewModel
{
    [TestClass]
    public class TwitterViewModelSpecs
    {
        private TwitterViewModel _twitterViewModel;

        [TestInitialize]
        public void Setup()
        {
            _twitterViewModel = new TwitterViewModel();
        }

        [TestMethod]
        public void Assert_shoud_notify_about_Error_change()
        {
            PropertyTester.TestChange(_twitterViewModel, (t) => t.Error);
            Assert.IsTrue(PropertyTester.WasNotified);
        }

        [TestMethod]
        public void Assert_should_notify_about_ErrorMessage_change()
        {
            PropertyTester.TestChange(_twitterViewModel, (t) => t.ErrorMessage);
            Assert.IsTrue(PropertyTester.WasNotified);
        }
    }
}