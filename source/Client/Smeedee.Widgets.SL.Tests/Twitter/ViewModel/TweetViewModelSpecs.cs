﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Tests;
using Smeedee.Widgets.SL.Twitter.ViewModel;

namespace Smeedee.Widgets.SL.Twitter.Tests.ViewModel
{
    [TestClass]
    public class TweetViewModelSpecs
    {
        private TweetViewModel tvm;

        [TestInitialize]
        public void Setup()
        {
            tvm = new TweetViewModel();
        }

        [TestMethod]
        public void assert_should_notify_about_username_change()
        {
            PropertyTester.TestChange(tvm, (t) => t.Username);
            Assert.IsTrue(PropertyTester.WasNotified);
        }

        [TestMethod]
        public void assert_should_notify_about_message_change()
        {
            PropertyTester.TestChange(tvm, (t) => t.Message);
            Assert.IsTrue(PropertyTester.WasNotified);
        }

        [TestMethod]
        public void assert_should_notify_about_date_posted_change()
        {
            PropertyTester.TestChange(tvm, (t) => t.Date);
            Assert.IsTrue(PropertyTester.WasNotified);
        }

        [TestMethod]
        public void assert_should_notify_userImageUrl_changed()
        {
            PropertyTester.TestChange(tvm, t => t.UserImageUrl);
            Assert.IsTrue(PropertyTester.WasNotified);
        }
    }
}
