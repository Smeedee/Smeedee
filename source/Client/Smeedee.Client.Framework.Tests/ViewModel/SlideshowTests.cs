using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Tests.ViewModel
{
    class SlideshowTests
    {
        private static string SlideshowInfoTemplate = "Slide {0}/{1} - Next slide in 15 seconds";
        private static string SlideshowInfoPausedTemplate = "Slide {0}/{1} - Paused";

        [TestFixture]
        public class When_spawned : SlideshowTestContext
        {
            public override void Context()
            {
                Given_Slideshow_is_created();
            }

            [Test]
            public void assure_it_has_slides()
            {
                viewModel.Slides.ShouldNotBeNull();
            }

            [Test]
            public void assure_it_has_ErrorInfo()
            {
                viewModel.ErrorInfo.ShouldNotBeNull();
            }


            [Test]
            public void assure_slides_are_loaded()
            {
                viewModel.Slides.Count.ShouldNotBe(0);
            }

            [Test]
            public void assure_the_first_Slide_is_set_as_CurrentSlide()
            {
                viewModel.CurrentSlide.ShouldBe(viewModel.Slides.First());
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, 1, viewModel.Slides.Count));
            }
        }

        [TestFixture]
        public class When_Next_slide : SlideshowTestContext
        {
            public override void Context()
            {
                Given_Slideshow_is_created();

                When_execute_Next_Command();
            }

            [Test]
            public void assure_second_slide_is_set_as_CurrentSlide()
            {
                viewModel.CurrentSlide.ShouldBe(viewModel.Slides.Skip(1).First());
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, 2, viewModel.Slides.Count));
            }
        }

        [TestFixture]
        public class When_Next_slide_and_standing_on_last_slide : SlideshowTestContext
        {
            public override void Context()
            {
                Given_Slideshow_is_created();

                for (int i = 0; i < viewModel.Slides.Count; i++)
                    When_execute_Next_Command();
            }

            [Test]
            public void assure_first_slide_is_set_as_CurrentSlide()
            {
                viewModel.CurrentSlide.ShouldBe(viewModel.Slides.First());
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, 1, viewModel.Slides.Count));
            }

        }

        [TestFixture]
        public class When_Previous_Slide_and_standing_on_first_slide : SlideshowTestContext
        {
            public override void Context()
            {
                Given_Slideshow_is_created();

                When_execute_Previous_Command();
            }

            [Test]
            public void assure_last_slide_is_set_as_CurrentSlide()
            {
                viewModel.CurrentSlide.ShouldBe(viewModel.Slides.Last());
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, viewModel.Slides.Count, viewModel.Slides.Count));
            }

        }

        [TestFixture]
        public class When_Previous_Slide : SlideshowTestContext
        {
            public override void Context()
            {
                Given_Slideshow_is_created();
                And_Slideshow_PropertyChangeRecording_is_Started();

                for(int i = 0; i < viewModel.Slides.Count; i++)
                    When_execute_Previous_Command();
            }

            [Test]
            public void assure_previous_slide_is_set_as_CurrentSlide()
            {
                viewModel.CurrentSlide.ShouldBe(viewModel.Slides.First());
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, viewModel.Slides.IndexOf(viewModel.CurrentSlide) + 1, viewModel.Slides.Count));
            }

            [Test]
            public void assure_SlideshowInfo_is_changed_for_everytime_Previous_command_is_executed()
            {
                viewModel.PropertyChangeRecorder.Data.Where(r => r.PropertyName == "SlideshowInfo").Count().ShouldBe(viewModel.Slides.Count);
            }

        }

        [TestFixture]
        public class When_CurrentSlide_changes : SlideshowTestContext
        {
            public override void Context()
            {
                Given_Slideshow_is_created();
                And_Slideshow_PropertyChangeRecording_is_Started();

                When_execute_Next_Command();
            }

            [Test]
            public void assure_observers_are_notified_about_change()
            {
                viewModel.PropertyChangeRecorder.Data.Where(r => r.PropertyName == "CurrentSlide").Count().ShouldBe(1);
            }
        }

        [TestFixture]
        public class When_Start : SlideshowTestContext
        {
            private Mock<ITimer> timerFake;

            public override void Context()
            {
                timerFake = GetFakeFor<ITimer>();
                timerFake.Setup(t => t.Start(It.IsAny<int>())).Callback((int interval) =>
                {
                    timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);
                });

                Given_Slideshow_is_created();
                And_Slideshow_PropertyChangeRecording_is_Started();

                When_execute_Start_Command();
            }

            [Test]
            public void assure_Timer_is_started()
            {
                timerFake.Verify(t => t.Start(It.IsAny<int>()), Times.Once());
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoTemplate, 1, viewModel.Slides.Count));
            }

        }

        [TestFixture]
        public class When_Slideshow_is_started : SlideshowTestContext
        {
            private Mock<ITimer> timerFake;

            public override void Context()
            {
                timerFake = GetFakeFor<ITimer>();

                Given_Slideshow_is_created();
                And_Start_Command_is_executed();

                When_execute_Start_Command();
            }

            [Test]
            public void assure_its_paused_when_slideshow_is_already_started()
            {
                timerFake.Verify(s => s.Stop(), Times.Once());
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, 1, viewModel.Slides.Count));
            }

            [Test]
            public void assure_SlideshowInfo_is_set_When_started_again()
            {
                When_execute_Start_Command();

                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoTemplate, 1, viewModel.Slides.Count));
            }


        }

        [TestFixture]
        public class When_Pause : SlideshowTestContext
        {
            private Mock<ITimer> timerFake;

            public override void Context()
            {
                timerFake = GetFakeFor<ITimer>();

                Given_Slideshow_is_created();
                And_Start_Command_is_executed();

                When_execute_Pause_Command();
            }

            [Test]
            public void assure_Timer_is_also_paused()
            {
                timerFake.Verify(s =>  s.Stop(), Times.Once());
            }
        }

        [TestFixture]
        public class When_loading_slides_and_fail : SlideshowTestContext
        {
            private Mock<IModuleLoader> moduleLoaderFake;
            private string ERROR_MSG = "something happend during initialization of slides";

            public override void Context()
            {
                Given_ModuleLoaderFake_is_created_and_configured_to_fail();

                When_Slideshow_is_spawned();
            }

            private void Given_ModuleLoaderFake_is_created_and_configured_to_fail()
            {
                moduleLoaderFake = GetFakeFor<IModuleLoader>();
                moduleLoaderFake.Setup(s => s.LoadSlides(It.IsAny<Slideshow>())).
                    Throws(new Exception(ERROR_MSG));
            }

            [Test]
            public void assure_failure_is_handeled()
            {
                viewModel.ErrorInfo.HasError.ShouldBeTrue();
            }

            [Test]
            public void assure_ErrorMsg_is_set()
            {
                viewModel.ErrorInfo.ErrorMessage.ShouldBe(ERROR_MSG);
            }

            [Test]
            public void assure_Next_Command_cant_be_executed()
            {
                viewModel.Next.CanExecute(null).ShouldBeFalse();
            }

            [Test]
            public void assure_Previous_Command_cant_be_executed()
            {
                viewModel.Previous.CanExecute(null).ShouldBeFalse();
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe("Slide 0/0 - Paused");
            }

            [Test]
            public void assure_Start_Command_cant_be_executed()
            {
                viewModel.Start.CanExecute(null).ShouldBeFalse();
            }

        }

        [TestFixture]
        public class When_slides_are_added : SlideshowTestContext
        {
            public override void Context()
            {
                Given_no_slides_exists();
                And_Slideshow_is_created();

                When_add_Slide(() =>
                    viewModel.Slides.Add(new Slide()));
            }

            private void Given_no_slides_exists()
            {
                var moduleLoaderFake = GetFakeFor<IModuleLoader>();
                moduleLoaderFake.Setup(s => s.LoadSlides(It.IsAny<Slideshow>()));
            }

            [Test]
            public void assure_Slide_is_added()
            {
                viewModel.Slides.Count.ShouldBe(1);
            }


            [Test]
            public void assure_SlideshowInfo_is_updated()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, 1, viewModel.Slides.Count));
            }

            [Test]
            public void assure_CurrentSlide_is_set()
            {
                viewModel.CurrentSlide.ShouldNotBeNull();
            }
        }

        [TestFixture]
        public class When_change_slide_and_CurrentSlide_is_in_Settings_mode : SlideshowTestContext
        {
            public override void Context()
            {
                Given_Slideshow_is_created();

                And("CurrentSlide is in settings mode", () =>
                    viewModel.CurrentSlide.Settings.Execute(null));
            }

            [Test]
            public void assure_previous_Slide_is_no_longer_in_Settings_mode_after_goto_Next_slide()
            {
                When_execute_Next_Command();

                AssureNoSlidesAreInSettingsMode();
            }

            private void AssureNoSlidesAreInSettingsMode()
            {
                viewModel.Slides.Where(s => s.IsInSettingsMode).Count().ShouldBe(0);
            }

            [Test]
            public void assure_previous_Slide_is_not_in_Settings_mode_after_goto_Previous_slide()
            {
                When_execute_Previous_Command();

                AssureNoSlidesAreInSettingsMode();
            }

        }

    }
}
