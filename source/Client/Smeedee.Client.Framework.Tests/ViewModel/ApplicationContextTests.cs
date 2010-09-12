using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Framework.Tests;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Tests.ViewModel
{
    class ApplicationContextTests
    {
        [TestFixture]
        public class When_spawned : ApplicationContextTestContext
        {
            public override void Context()
            {
            	ViewModelBootstrapperForTests.Initialize();

                Given_ApplicationContext_is_created();
            }


            [Test]
            public void Then_assure_it_has_a_Traybar()
            {
                viewModel.Traybar.ShouldNotBeNull();
            }

            [Test]
            public void The_assure_it_has_a_Slideshow()
            {
                viewModel.Slideshow.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_it_has_a_DockBar()
            {
                viewModel.DockBar.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_CurrentSlide_in_Slideshow_changes : ApplicationContextTestContext
        {
            public override void Context()
            {
                Given_ApplicationContext_is_created();

                viewModel.Slideshow.Next.Execute(null);
            }
        }
    }
}
