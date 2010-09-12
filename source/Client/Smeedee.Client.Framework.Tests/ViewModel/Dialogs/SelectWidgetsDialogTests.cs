using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Tests.ViewModel;
using Smeedee.DomainModel.Framework;
using TinyBDD.Specification.NUnit;
using TinyMVVM.IoC;

namespace Smeedee.Client.Framework.Tests.ViewModel.Dialogs
{
	public class SelectWidgetsDialogTests
	{
		[TestFixture]
		public class When_spawned : Shared
		{
			public override void Context()
			{
				Given_SelectWidgetsDialog_is_created();
			}

			[Test]
			public void assure_it_has_AvailableWidgets()
			{
				viewModel.AvailableWidgets.ShouldNotBeNull();
			}

			[Test]
			public void assure_is_has_SelectedWidgets()
			{
				viewModel.SelectedWidgets.ShouldNotBeNull();
			}

		    [Test]
		    public void assure_it_has_a_Progressbar()
		    {
		        viewModel.Progressbar.ShouldNotBeNull();
		    }
		}

		[TestFixture]
		public class When_SelectAll : Shared
		{
			public override void Context()
			{
				Given_SelectWidgetsDialog_is_created();
				And_it_has_AvailableWidgets();
			    And_SearchTerm_is_set("top");

				When_execute_SelectAll_Command();
			}

			[Test]
			public void assure_all_FilteredWidgets_are_selected()
			{
				viewModel.SelectedWidgets.Count().ShouldBe(viewModel.FilteredWidgets.Count());
			}

		    [Test]
		    public void Assure_non_filtered_widgets_are_not_selected()
		    {
                viewModel.AvailableWidgets[1].IsSelected.ShouldBeFalse();
		    }
		}

		[TestFixture]
		public class When_DeselectAll : Shared
		{
			public override void Context()
			{
				Given_SelectWidgetsDialog_is_created();
				And_it_has_AvailableWidgets();
				And_SelectAll_Command_is_executed();

				When_execute_DeselectAll_Command();
			}

			[Test]
			public void assure_no_Widgets_are_selected()
			{
				viewModel.SelectedWidgets.Count().ShouldBe(0);
			}
		}

	    [TestFixture]
	    public class when_setting_SearchTerm : Shared
	    {
	        [Test]
	        public override void Context()
	        {
	            Given_SelectWidgetsDialog_is_created();
                And_it_has_AvailableWidgets();
                And_SelectWidgetsDialog_PropertyChangeRecording_is_Started();
                And_SearchTerm_is_set("top");
	        }

	        [Test]
	        public void Assure_hits_are_shown_in_FilteredWidgets()
	        {
                viewModel.FilteredWidgets.Count().ShouldBe(1);
	        }

	        [Test]
	        public void Assure_tags_are_included_in_the_search()
	        {
                viewModel.AvailableWidgets.Add(new WidgetMetadata() { Tags = new string[] { "top notch"}});
                viewModel.FilteredWidgets.Count().ShouldBe(2);
	        }

	        [Test]
            public void Assure_PropertyChanged_for_FilteredWidgets_is_fired()
	        {
	            viewModel.PropertyChangeRecorder.Data.Any(r => r.PropertyName == "FilteredWidgets").ShouldBeTrue();
	        }

	        [Test]
	        public void Assure_blank_SearchTerm_gives_all_availalble_widgets_in_FilteredWidgets()
	        {
                And_SearchTerm_is_set("");
                viewModel.FilteredWidgets.Count().ShouldBe(viewModel.AvailableWidgets.Count);
	        }
	    }

        public class Shared : SelectWidgetsDialogTestContext
        {
            protected WidgetMetadata widgetMetadata;

            public override void Context()
            {
                ViewModelBootstrapperForTests.Initialize();
            }

            protected void And_it_has_AvailableWidgets()
            {
                widgetMetadata = new WidgetMetadata() { Name = "Top commiters", UserSelectedTitle = "Noe annet", Type = typeof(TestWidget), SecondsOnScreen = 100, XAPName = "TestWidget.xap"};
                viewModel.AvailableWidgets.Add(widgetMetadata);
                viewModel.AvailableWidgets.Add(new WidgetMetadata() { Name = "Latest commits", Type = typeof(TestWidget), SecondsOnScreen = 200, XAPName = "TestWidget.xap"});
            }
        }

        public class TestWidget : Widget
        {
            
        }
	}
}
