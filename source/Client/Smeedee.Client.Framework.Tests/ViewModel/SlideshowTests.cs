using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Tests;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Tests.ViewModel
{
    class SlideshowTests
    {
        private static string SlideshowInfoTemplate = "Slide {0}/{1} - Next slide in {2} seconds";
        private static string SlideshowInfoPausedTemplate = "Slide {0}/{1} - Paused";

        [TestFixture]
        public class When_spawned : Shared
        {
            public override void Context()
            {
				base.Context();

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
        	public void assure_Slides_are_loaded()
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
        public class When_Next_slide : Shared
        {
            public override void Context()
            {
				base.Context();

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
        public class When_Next_slide_and_standing_on_last_slide : Shared
        {
            public override void Context()
            {
				base.Context();

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
        public class When_Previous_Slide_and_standing_on_first_slide : Shared
        {
            public override void Context()
            {
				base.Context();

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
        public class When_Previous_Slide : Shared
        {
            public override void Context()
            {
				base.Context();

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
        public class When_CurrentSlide_changes : Shared
        {
            public override void Context()
            {
				base.Context();

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
        public class When_Start : Shared
        {
        	public override void Context()
            {
				base.Context();

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
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoTemplate, 1, viewModel.Slides.Count, viewModel.CurrentSlide.SecondsOnScreen));
            }

        }

        [TestFixture]
        public class when_changing_slides : Shared
        {
            public override void Context()
            {
                base.Context();
                Given_Slideshow_is_created();

                Slide firstSlide = new Slide() {Title = "one", SecondsOnScreen = 1, Widget = new Widget()};
                var slides = new ObservableCollection<Slide>
                {
                    firstSlide,
                    new Slide() {Title = "next", SecondsOnScreen = 100, Widget = new Widget()}
                };

                And_Slides_is_set(slides);
                And_CurrentSlide_is_set(firstSlide);
                And_Start_Command_is_executed();

            }


            private void When_timer_elapses()
            {
                timerFake = ViewModelBootstrapperForTests.TimerFake;
                timerFake.Raise(r => r.Elapsed += null, new EventArgs());
            }

            [Test]
            public void Assure_SecondsOnScreen_is_used_to_decide_when_to_change_slide()
            {
                Thread.Sleep(2000);
                When_timer_elapses();
                viewModel.CurrentSlide.Title.ShouldBe("next");
            }

            [Test]
            public void Assure_Percent_is_updated_using_seconds_on_screen()
            {
                Thread.Sleep(500);
                When_timer_elapses();
                Debug.WriteLine(viewModel.TimeLeftOfSlideInPercent);

                (viewModel.TimeLeftOfSlideInPercent > 0.4 && viewModel.TimeLeftOfSlideInPercent < 0.6 ).ShouldBeTrue();
                
            }
        }


        [TestFixture]
        public class When_Slideshow_is_started : Shared
        {
            public override void Context()
            {
				base.Context();

                Given_Slideshow_is_created();
                And_Start_Command_is_executed();

                When_execute_Start_Command();
            }

            [Test]
            public void assure_its_ignored_when_slideshow_is_already_started()
            {
                viewModel.IsRunning.ShouldBe(true);
            }

            [Test]
            public void assure_SlideshowInfo_is_set_When_started_again()
            {
                When_execute_Start_Command();

                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoTemplate, 1, viewModel.Slides.Count, viewModel.CurrentSlide.SecondsOnScreen));
            }
        }

        [TestFixture]
        public class When_Pause : Shared
        {
            public override void Context()
            {
                base.Context();

                Given_Slideshow_is_created();
                And_Pause_Command_is_executed();
            }

            [Test]
            public void assure_SlideshowInfo_is_set()
            {
                viewModel.SlideshowInfo.ShouldBe(string.Format(SlideshowInfoPausedTemplate, 1, viewModel.Slides.Count));
            }

            [Test]
            public void assure_Slideshow_is_paused()
            {
                viewModel.IsRunning.ShouldBeFalse();
            }
        }

        [TestFixture]
        public class When_loading_slides_fails : Shared
        {
        	private string ERROR_MSG = "something happend during initialization of slides";

            public override void Context()
            {
				base.Context();

                Given_ModuleLoaderFake_is_created_and_configured_to_fail();

                When_Slideshow_is_spawned();
            }

            private void Given_ModuleLoaderFake_is_created_and_configured_to_fail()
            {
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
        public class When_slides_are_added : Shared
        {
            public override void Context()
            {
				base.Context();

                Given_no_slides_exists();
                And_Slideshow_is_created();

                When_add_Slide(() =>
                    viewModel.Slides.Add(new Slide()));
            }

            private void Given_no_slides_exists()
            {
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
        public class When_slides_are_added_and_welcomeWidget_is_in_slideshow : Shared
        {
            private int numberOfSlidesBeforeAdd;

            public override void Context()
            {
                base.Context();
                Given_Slideshow_is_created();
                And_It_contains_the_welcomeWidget();
                And_one_slide_is_the_dialog();

                numberOfSlidesBeforeAdd = viewModel.Slides.Count;

                When_execute_AddSlide_Command();
            }

            private void And_one_slide_is_the_dialog()
            {
                modalDialogServiceFake.Setup(s => s.Show(
                    It.IsAny<SelectWidgetsDialog>(),
                    It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
                    {
                        viewModel.SelectWidgetsDialog.NewSlides.Add(new Slide());
                        dialogClosedCallback.Invoke(true);
                    });
            }

            private void And_It_contains_the_welcomeWidget()
            {
                viewModel.Slides.Add(new Slide{Widget = new WelcomeWidget()});
            }

            [Test]
            public void assure_the_welcomeWidget_is_not_in_the_slideshow()
            {
                foreach (var slide in viewModel.Slides)
                {
                    if(slide.Widget != null)
                        slide.Widget.GetType().ShouldNotBe(typeof(WelcomeWidget));
                }
            }

            
        }

        [TestFixture]
        public class When_CurrentSlide_is_in_Settings_mode : Shared
        {
            public override void Context()
            {
				base.Context();

                Given_Slideshow_is_created();

                And("CurrentSlide is in settings mode", () =>
                {
                    viewModel.CurrentSlide = new Slide() {Widget = new Widget()};
                    viewModel.CurrentSlide.Settings.Execute(null);
                });
            }

            [Test]
            public void assure_current_slide_is_not_changed_on_next_command()
            {
                var currenSlide = viewModel.CurrentSlide;

                When_execute_Next_Command();

                Assert.AreSame(currenSlide, viewModel.CurrentSlide);
                Assert.IsTrue(viewModel.CurrentSlide.IsInSettingsMode);
            }

            [Test]
            public void assure_current_Slide_is_not_changed_on_previous_command()
            {
                var currentSlide = viewModel.CurrentSlide;

                When_execute_Previous_Command();

                Assert.AreSame(currentSlide, viewModel.CurrentSlide);
                Assert.IsTrue(viewModel.CurrentSlide.IsInSettingsMode);
            }

            [Test]
            public void Assure_pause_command_is_ignored()
            {
                When_execute_Pause_Command();

                timerFake.Verify(t=>t.Stop(), Times.Never());
            }
        }

    	[TestFixture]
    	public class When_AddSlide : Shared
    	{
            private int numberOfSlidesBeforeAdd;

			public override void Context()
			{
				base.Context();

				Given_Slideshow_is_created();
			    And_SelectWidgetsDialog_Contains_one_new_slide();

                numberOfSlidesBeforeAdd = viewModel.Slides.Count;
            }

    		[Test]
    		public void assure_ModalDialog_is_displayed()
    		{
                When_execute_AddSlide_Command();

    			modalDialogServiceFake.Verify(s => s.Show(
                    It.IsAny<SelectWidgetsDialog>(),
                    It.IsAny<Action<bool>>()));
    		}

    	    [Test]
    	    public void assure_Slide_is_added_if_DialogResult_is_OK()
    	    {
    	        modalDialogServiceFake.Setup(s => s.Show(
                    It.IsAny<SelectWidgetsDialog>(),
                    It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
                    {
                        viewModel.SelectWidgetsDialog.NewSlides.Add(new Slide());
                        dialogClosedCallback.Invoke(true);
                    });

                When_execute_AddSlide_Command();
                viewModel.Slides.Count.ShouldBe(numberOfSlidesBeforeAdd + 1);
    	    }

    	    [Test]
    	    public void assure_Slide_is_not_added_if_DialogResult_is_Cancel()
    	    {
    	        modalDialogServiceFake.Setup(s => s.Show(
    	            It.IsAny<Dialog>(),
    	            It.IsAny<Action<bool>>()));

                When_execute_AddSlide_Command();

                viewModel.Slides.Count.ShouldBe(numberOfSlidesBeforeAdd);
    	    }
    	}

        [TestFixture]
        public class When_Edit : Shared
        {
            private int numberOfSlidesBeforeDelete;

            public override void Context()
            {
                base.Context();

                Given_Slideshow_is_created();

                numberOfSlidesBeforeDelete = viewModel.Slides.Count;
            }

            [Test]
            public void assure_ModalDialog_is_displayed()
            {
                When_execute_Edit_Command();

                modalDialogServiceFake.Verify(s => s.Show(
                    It.IsAny<EditSlideshowDialog>(),
                    It.IsAny<Action<bool>>()));
            }

            [Test]
            public void assure_slides_are_persisted_if_dialogresult_is_ok()
            {
                modalDialogServiceFake.Setup(s => s.Show(
                    It.IsAny<EditSlideshowDialog>(),
                    It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
                    {
                        dialogClosedCallback.Invoke(true);
                    });

                When_execute_Edit_Command();
                ViewModelBootstrapperForTests.SlideConfigurationPersisterFake.Verify(p => p.Save(It.IsAny<List<SlideConfiguration>>()), Times.Once());
            }

            [Test]
            public void assure_slides_are_not_persisted_if_dialogresult_is_not_ok()
            {
                modalDialogServiceFake.Setup(s => s.Show(
                    It.IsAny<EditSlideshowDialog>(),
                    It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
                    {
                        dialogClosedCallback.Invoke(false);
                    });

                When_execute_Edit_Command();
                ViewModelBootstrapperForTests.SlideConfigurationPersisterFake.Verify(p => p.Save(It.IsAny<List<SlideConfiguration>>()), Times.Never());
            }

            [Test]
            public void assure_the_amount_of_slides_we_persist_are_the_same_as_in_the_viewmodel()
            {
                Slideshow_has_one_slide();

                modalDialogServiceFake.Setup(s => s.Show(
                    It.IsAny<EditSlideshowDialog>(),
                    It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
                    {
                        dialogClosedCallback.Invoke(true);
                    });

                When_execute_Edit_Command();
                ViewModelBootstrapperForTests.SlideConfigurationPersisterFake.Verify(p => p.Save(It.Is<IEnumerable<SlideConfiguration>>(c => c.Count() == _slides.Count)), Times.Once());
            }



            [Test]
            public void assure_the_slide_information_we_persist_are_the_same_as_in_the_viewmodel()
            {
                Slideshow_has_one_slide();

                modalDialogServiceFake.Setup(s => s.Show(
                    It.IsAny<EditSlideshowDialog>(),
                    It.IsAny<Action<bool>>())).Callback((Dialog dialog, Action<bool> dialogClosedCallback) =>
                    {
                        dialogClosedCallback.Invoke(true);
                    });

                When_execute_Edit_Command();
                ViewModelBootstrapperForTests.SlideConfigurationPersisterFake.Verify(p => p.Save(It.Is<IEnumerable<SlideConfiguration>>(c => c.First().Title == _slides.First().Title)), Times.Once());
            }

        }

        [TestFixture]
        public class When_a_slides_widget_enters_settings_mode_ : Shared
        {
            public override void Context()
            {
                base.Context();

                Given_Slideshow_is_created();
                When_add_Slide(() =>
                   viewModel.Slides.Add(new Slide()
                                            {
                                                Widget = new Widget()
                                            }));
                When_add_Slide(() =>
                   viewModel.Slides.Add(new Slide()
                   {
                       Widget = new Widget()
                   }));
            }

            [Test]
            public void Assure_pauses_when_in_settings_mode()
            {
                viewModel.Start.Execute();
                viewModel.IsRunning.ShouldBeTrue();;
                viewModel.Slides[0].Widget.IsInSettingsMode = true;
                viewModel.IsRunning.ShouldBeFalse();
            }
        }

        public class Shared : SlideshowTestContext
        {
			protected Mock<IModalDialogService> modalDialogServiceFake;
        	protected Mock<ITimer> timerFake;
        	protected Mock<IModuleLoader> moduleLoaderFake;
            protected ObservableCollection<Slide> _slides;

            public override void Context()
            {
				ViewModelBootstrapperForTests.Initialize();
            	modalDialogServiceFake = ViewModelBootstrapperForTests.ModalDialogServiceFake;
				timerFake = ViewModelBootstrapperForTests.TimerFake;
        		moduleLoaderFake = ViewModelBootstrapperForTests.ModuleLoaderFake;
            }

            public void And_SelectWidgetsDialog_Contains_one_new_slide()
            {
                viewModel.SelectWidgetsDialog.NewSlides.Add(new Slide()
                {
                    Title = "My widget",
                    SecondsOnScreen = 100
                });


                
            }

            public void Slideshow_has_one_slide()
            {
                _slides = new ObservableCollection<Slide>
                              {
                                  new Slide()
                                      {
                                          SecondsOnScreen = 10,
                                          Title = "Test",
                                          Widget = new Widget()
                                      }
                              };

                viewModel.Slides = _slides;
            }
        }
    }
}
