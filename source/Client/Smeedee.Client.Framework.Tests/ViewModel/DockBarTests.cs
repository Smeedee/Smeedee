using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Tests.ViewModel;
using TinyBDD.Specification.NUnit;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.Tests.ViewModel
{
    public class DockBarTests
    {
        [TestFixture]
        public class When_spawned : Shared
        {
            public override void Context()
            {
                base.Context();

                When_DockBar_is_spawned();
            }

            [Test]
            public void Then_assure_it_has_Items()
            {
                viewModel.Items.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_GetDependency : Shared
        {
            public override void Context()
            {
                base.Context();

                Given_DockBar_is_created();

                When("Get Dependency", () => {});
            }

            [Test]
            [Ignore("Will not work because of an bug in TinyMVVM")]
            public void Then_assure_IDockBarService_can_be_resolved()
            {
                viewModel.GetDependency<IDockBarService>().ShouldNotBeNull();
            }
        }

        public class Shared : DockBarTestContext
        {
            public override void Context()
            {
                ConfigureGlobalDependencies.ForAllViewModels(config =>
                {
                    config.Bind<IModuleLoader>().ToInstance(new Mock<IModuleLoader>().Object);
                });
            }
        }
    }
}
