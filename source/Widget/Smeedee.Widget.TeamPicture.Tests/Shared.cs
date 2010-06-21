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
using Ninject;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Widget.TeamPicture.Tests.ViewModel;
using Smeedee.Widget.TeamPicture.ViewModel;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.TeamPicture.Tests
{
    public class ServiceLocatorForTesting : DefaultServiceLocator
    {
        private IKernel kernel;

        public ServiceLocatorForTesting()
        {
            kernel = new StandardKernel();
            kernel.Bind<IUIInvoker>().ToConstant(new NoUIInvokation());
        }


        public override T GetInstance<T>()
        {
            return kernel.Get<T>();
        }
    }


    [TestClass]
    public class Shared
    {
        protected TeamPictureViewModel _vm;

        [TestInitialize]
        public void Setup()
        {
            ServiceLocator.SetLocator(new ServiceLocatorForTesting());
            _vm = new TeamPictureViewModel();
        }

    }
}
