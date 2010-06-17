using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Tests.ViewModel
{
    class TraybarTests
    {
        [TestFixture]
        public class When_spawned : TraybarTestContext
        {
            public override void Context()
            {
                Given_Traybar_is_created();
            }

            [Test]
            public void assure_it_has_widgets()
            {
                viewModel.Widgets.ShouldNotBeNull();
            }

            [Test]
            public void assure_Widgets_are_loaded()
            {
                viewModel.Widgets.Count.ShouldNotBe(0);    
            }

            [Test]
            public void assure_it_has_ErrorInfo()
            {
                viewModel.ErrorInfo.ShouldNotBeNull();
            }

        }

        [TestFixture]
        public class When_loading_widgets_and_fail : TraybarTestContext
        {
            private Mock<IModuleLoader> moduleLoaderFake;
            private string ERROR_MSG = "something happend during initialization of the widgets";

            public override void Context()
            {
                Given_ModuleLoaderFake_is_created_and_configured_to_fail();

                When_Traybar_is_spawned();
            }

            private void Given_ModuleLoaderFake_is_created_and_configured_to_fail()
            {
                moduleLoaderFake = GetFakeFor<IModuleLoader>();
                moduleLoaderFake.Setup(s => s.LoadTraybarWidgets(It.IsAny<Traybar>())).
                    Throws(new Exception(ERROR_MSG));
            }

            [Test]
            public void assure_Failure_is_handled()
            {
                viewModel.ErrorInfo.HasError.ShouldBeTrue();
            }

            [Test]
            public void assure_ErrorMsg_is_set()
            {
                viewModel.ErrorInfo.ErrorMessage.ShouldBe(ERROR_MSG);
            }
        }
    }
}
