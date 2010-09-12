using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Smeedee.Tests;
using Smeedee.Widget.Twitter.ViewModel;

namespace Smeedee.Widget.Twitter.Tests.ViewModel
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
