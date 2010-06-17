using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Tests.ViewModel
{
    class ApplicationTests
    {
        [TestFixture]
        public class When_spawned : ApplicationContextTestContext
        {
            public override void Context()
            {
                Given_ApplicationContext_is_created();
            }

            [Test]
            public void assure_it_has_a_Title()
            {
                viewModel.Title.ShouldNotBeNull();
            }

            [Test]
            public void assure_it_has_a_Traybar()
            {
                viewModel.Traybar.ShouldNotBeNull();
            }

            [Test]
            public void assure_it_has_a_Slideshow()
            {
                viewModel.Slideshow.ShouldNotBeNull();
            }

            [Test]
            public void assure_Slideshows_CurrentSlide_Title_is_set_as_Application_Title()
            {
                viewModel.Title.ShouldBe(viewModel.Slideshow.CurrentSlide.Title);
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

            [Test]
            public void assure_Slideshows_CurrentSlide_Title_is_reflected_to_the_Application_Title()
            {
                viewModel.Title.ShouldBe(viewModel.Slideshow.Slides.Skip(1).First().Title);
            }

        }


        [TestFixture]
        public class When_Title_changes : ApplicationContextTestContext
        {
            public override void Context()
            {
                Given_ApplicationContext_is_created();
                And_ApplicationContext_PropertyChangeRecording_is_Started();

                When_Title_is_set("Hello world");
            }

            [Test]
            public void assure_observers_are_notified_about_change()
            {
                viewModel.PropertyChangeRecorder.Data.Where(r => r.PropertyName == "Title").Count().ShouldBe(1);
            }
        }
    }
}
