using System;
using System.ComponentModel.Composition.Hosting;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Widget.TeamPicture.ViewModel;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.TeamPicture.Tests.ViewModel
{
    [TestClass]
    public class TeamPictureViewModelTests : Shared
    {
        [TestMethod]
        public void CanInstatiateViewModel()
        {
            Assert.IsNotNull(_vm);
        }

        [TestMethod]
        public void HasSelectedSnapshot_should_change_when_selectedSnapshot_change()
        {
            
            _vm.SelectedSnapshot = new WriteableBitmap(100,100);
            
            Assert.IsTrue(_vm.HasSelectedSnapshot);

            _vm.SelectedSnapshot = null;

            Assert.IsFalse(_vm.HasSelectedSnapshot);
        }
    }
}