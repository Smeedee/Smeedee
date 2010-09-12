using System;
using System.Linq;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Tests.ViewModel;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;

namespace Smeedee.Client.Framework.Tests.ViewModel.Dialogs
{
    public class EditSlideshowDialogTests
    {
        [TestFixture]
        public class When_spawned : Shared
        {
            public override void Context()
            {
                base.Context();

                When_EditSlideshowDialog_is_spawned();
            }

            [Test]
            public void Then_assure_it_doesnt_have_a_Slideshow()
            {
                viewModel.Slideshow.ShouldBeNull();
            }

            [Test]
            public void Then_assure_it_has_a_View()
            {
                viewModel.View.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_it_has_a_Title()
            {
                viewModel.Title.ShouldNotBeNull();
            }

        	[Test]
        	public void Then_assure_it_has_ButtonBarCommands()
        	{
				viewModel.ButtonBarCommands.ShouldNotBeNull();
        	}

        	[Test]
        	public void Then_assure_Delete_Command_is_in_ButtonBarCommands()
        	{
				viewModel.ButtonBarCommands.Any(c => c == viewModel.Delete).ShouldBeTrue();
        	}

        	[Test]
        	public void Then_assure_MoveRight_Command_is_in_ButtonBarCommands()
        	{
				viewModel.ButtonBarCommands.Any(c => c == viewModel.MoveRight).ShouldBeTrue();	
        	}

        	[Test]
        	public void Then_assure_MoveLeft_Command_is_in_ButtonBarCommands()
        	{
				viewModel.ButtonBarCommands.Any(c => c == viewModel.MoveLeft).ShouldBeTrue();
        	}

            [Test]
            public void Then_assure_Delete_Command_Text_is_set()
            {
                viewModel.Delete.Text.ShouldBe("Delete");
            }

            [Test]
            public void Then_assure_MoveRight_Command_Text_is_set()
            {
                viewModel.MoveRight.Text.ShouldBe(" -> ");
            }

            [Test]
            public void Then_assure_MoveLeft_Command_Text_is_set()
            {
                viewModel.MoveLeft.Text.ShouldBe(" <- ");
            }
        }

    	[TestFixture]
    	public class When_no_Slides_are_selected : Shared
    	{
			public override void Context()
			{
                base.Context();

				Given_EditSlideshowDialog_is_created();
			    And_Slideshow_has_Slides();
				And_SelectedSlide_is_set(null);
			}

			[Test]
			public void Then_assure_Delete_Command_is_disabled()
			{
				viewModel.Delete.CanExecute(null).ShouldBeFalse();
			}

			[Test]
			public void Then_assure_MoveRight_Command_is_disabled()
			{
				viewModel.MoveRight.CanExecute(null).ShouldBeFalse();
			}

			[Test]
			public void Then_assure_MoveLeft_Command_is_disabled()
			{
				viewModel.MoveLeft.CanExecute(null).ShouldBeFalse();
			}
    	}

        [TestFixture]
        public class When_Slide_is_Selected : Shared
        {
            private CommandStateChangeRecorder cmdStateChangeRecorder;

            public override void Context()
            {
                base.Context();

                Given_EditSlideshowDialog_is_created();
                cmdStateChangeRecorder = new CommandStateChangeRecorder(viewModel);
                cmdStateChangeRecorder.Start();

                And_Slideshow_has_Slides();

                When("Slide is Selected", () =>
                    viewModel.Slideshow.CurrentSlide = viewModel.Slideshow.Slides.First());
            }

            [Test]
            public void Then_assure_MoveLeft_can_be_executed()
            {
                viewModel.MoveLeft.CanExecute().ShouldBeTrue();
            }

            [Test]
            public void Then_assure_MoveRight_can_be_executed()
            {
                viewModel.MoveRight.CanExecute().ShouldBeTrue();
            }

            [Test]
            public void Then_assure_Delete_can_be_executed()
            {
                viewModel.Delete.CanExecute().ShouldBeTrue();
            }

            [Test]
            public void Then_assure_Observers_are_notified_about_MoveLeft_command_state_change()
            {
                cmdStateChangeRecorder.Data.Any(r => r.Command == viewModel.MoveLeft).ShouldBeTrue();       
            }

            [Test]
            public void Then_assure_Observers_are_notified_about_MoveRight_command_state_change()
            {
                cmdStateChangeRecorder.Data.Any(r => r.Command == viewModel.MoveRight).ShouldBeTrue();
            }

            [Test]
            public void Then_assure_Observers_are_notified_about_Delete_command_state_change()
            {
                cmdStateChangeRecorder.Data.Any(r => r.Command == viewModel.Delete).ShouldBeTrue();
            }
        }

    	[TestFixture]
    	public class When_Delete : Shared
    	{
    		protected Slide removedSlide;

			public override void Context()
			{
                base.Context();

				Given_EditSlideshowDialog_is_created();
				And_Slideshow_has_Slides();

				removedSlide = viewModel.Slideshow.Slides.First();
				And_SelectedSlide_is_set(removedSlide);

				When_execute_Delete_Command();
			}

    		[Test]
    		public void Then_assure_Slide_is_removed()
    		{
    			viewModel.Slideshow.Slides.Any(s => s == removedSlide).ShouldBeFalse();
    		}

            [TestFixture]
            public class When_a_slide_is_removed : Shared
            {
                protected Slide removedSlide;
                
                public override void Context()
                {
                    base.Context();

                    Given_EditSlideshowDialog_is_created();
                    
                }

                [Test]
                public void Then_assure_the_selected_slide_becomes_the_slide_to_the_right_for_the_removed_one()
                {
                    And_Slideshow_has_Slides();
                    removedSlide = viewModel.Slideshow.Slides.First();
                    And_SelectedSlide_is_set(removedSlide);
                    And_index_of_Selected_Slide_is_sampled();
                   
                    When_execute_Delete_Command();

                    var oldIndex = SelectedSlideIndex;
                    And_index_of_Selected_Slide_is_sampled();
                    var newIndex = SelectedSlideIndex;
                    newIndex.ShouldBe(oldIndex);
                }

                [Test]
                public void Then_assure_the_selected_slide_becomes_the_slide_to_the_left_for_the_removed_one()
                {
                    And_Slideshow_has_Slides();
                    removedSlide = viewModel.Slideshow.Slides.Last();
                    And_SelectedSlide_is_set(removedSlide);
                    And_index_of_Selected_Slide_is_sampled();
                  
                    When_execute_Delete_Command();

                    var oldIndex = SelectedSlideIndex;
                    And_index_of_Selected_Slide_is_sampled();
                    var newIndex = SelectedSlideIndex;
                    newIndex.ShouldBe(oldIndex-1);
                }

                [Test]
                public void Then_assure_no_slides_are_selected()
                {
                    And_Slideshow_has_one_Slide();
                    for (int i = viewModel.Slideshow.Slides.Count - 1; i >= 0; i--)
                    {
                        var slide = viewModel.Slideshow.Slides[i];

                        And_SelectedSlide_is_set(slide);
                        When_execute_Delete_Command();
                    }
                    viewModel.Slideshow.CurrentSlide.ShouldBe(null);
                }

            }
    	}

        [TestFixture]
        public class When_move_first_Slide_to_the_right : Shared
        {
            public override void Context()
            {
                base.Context();

                Given_EditSlideshowDialog_is_created();
                And_Slideshow_has_Slides();
                And_SelectedSlide_is_set(viewModel.Slideshow.Slides.First());
                And_index_of_Selected_Slide_is_sampled();
                
                When_execute_MoveRight_Command();
            }

            [Test]
            public void Then_assure_Slide_is_moved_one_step_to_the_right()
            {
                viewModel.Slideshow.Slides[SelectedSlideIndex + 1].ShouldBe(SelectedSlide);             
            }

            [Test]
            public void Then_assure_Slides_are_not_accumulated()
            {
                viewModel.Slideshow.Slides.Count.ShouldBe(numberOfSlidesInSlideshow);
            }
        }

        [TestFixture]
        public class When_move_first_Slide_to_the_left : Shared
        {
            public override void Context()
            {
                base.Context();

                Given_EditSlideshowDialog_is_created();
                And_Slideshow_has_Slides();
                And_SelectedSlide_is_set(viewModel.Slideshow.Slides.First());
                And_index_of_Selected_Slide_is_sampled();

                When_execute_MoveLeft_Command();
            }

            [Test]
            public void Then_assure_Slide_is_last_of_all_Slides()
            {
                viewModel.Slideshow.Slides.Last().ShouldBe(SelectedSlide);
            }

            [Test]
            public void Then_assure_Slides_are_not_accumulated()
            {
                viewModel.Slideshow.Slides.Count.ShouldBe(numberOfSlidesInSlideshow);
            }
        }

        [TestFixture]
        public class When_move_last_Slide_to_the_right : Shared
        {
            public override void Context()
            {
                base.Context();

                Given_EditSlideshowDialog_is_created();
                And_Slideshow_has_Slides();
                And_SelectedSlide_is_set(viewModel.Slideshow.Slides.Last());
                And_index_of_Selected_Slide_is_sampled();

                When_execute_MoveRight_Command();
            }

            [Test]
            public void Then_assure_Slide_is_first_of_all_Slides()
            {
                viewModel.Slideshow.Slides.First().ShouldBe(SelectedSlide);
            }

            [Test]
            public void Then_assure_Slides_are_not_accumulated()
            {
                viewModel.Slideshow.Slides.Count.ShouldBe(numberOfSlidesInSlideshow);
            }
        }

        [TestFixture]
        public class When_move_last_Slide_to_the_left : Shared
        {
            public override void Context()
            {
                base.Context();

                Given_EditSlideshowDialog_is_created();
                And_Slideshow_has_Slides();
                And_SelectedSlide_is_set(viewModel.Slideshow.Slides.Last());
                And_index_of_Selected_Slide_is_sampled();

                When_execute_MoveLeft_Command();
            }

            [Test]
            public void Then_assure_Slide_is_moved_to_the_left()
            {
                viewModel.Slideshow.Slides[SelectedSlideIndex - 1].ShouldBe(SelectedSlide);
            }

            [Test]
            public void Then_assure_Slides_are_not_accumulated()
            {
                viewModel.Slideshow.Slides.Count.ShouldBe(numberOfSlidesInSlideshow);
            }
        }

		public class Shared : EditSlideshowDialogTestContext
		{
		    protected int numberOfSlidesInSlideshow;
		    public int SelectedSlideIndex { get; private set; }
		    public Slide SelectedSlide { get; set; }

			public override void Context()
			{
                ViewModelBootstrapperForTests.Initialize();
			}
            
            protected void And_index_of_Selected_Slide_is_sampled()
            {
                SelectedSlideIndex = viewModel.Slideshow.Slides.IndexOf(viewModel.Slideshow.CurrentSlide);
                SelectedSlide = viewModel.Slideshow.CurrentSlide;
            }

            protected void And_SelectedSlide_is_set(Slide slide)
            {
                viewModel.Slideshow.CurrentSlide = slide;
            }

			protected void And_Slideshow_has_Slides()
			{
				And_Slideshow_is_set(new Slideshow());
				viewModel.Slideshow.Slides.Add(new Slide()
				{
					Title = "Latest commits",
					SecondsOnScreen = 15
				});
				viewModel.Slideshow.Slides.Add(new Slide()
				{
					Title = "Build status",
					SecondsOnScreen = 15
				});

				viewModel.Slideshow.Slides.Add(new Slide()
				{
					Title = "Working days left",
					SecondsOnScreen = 15
				});

			    numberOfSlidesInSlideshow = viewModel.Slideshow.Slides.Count;
			}

            protected void And_Slideshow_has_one_Slide()
            {
                And_Slideshow_is_set(new Slideshow());
                viewModel.Slideshow.Slides.Add(new Slide()
                {
                    Title = "Latest commits",
                    SecondsOnScreen = 15
                });
                Console.WriteLine(viewModel.Slideshow.Slides.Count);
            }
		}
    }
}
