using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.DSL.Specifications;
using Smeedee.DomainModel.Framework.Logging;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.Services.Impl
{
    public class ModuleLoaderSpecs
    {
    	[TestFixture]
    	public class When_creating : Shared
    	{
    		[Test]
    		public void Then_assure_SlideConfigRepository_arg_is_validated()
    		{
    			this.ShouldThrowException<ArgumentNullException>(() =>
					new ModuleLoader(null, widgetMetadataRepoMock.Object, loggerMock.Object));
    		}

    		[Test]
    		public void Then_assure_Logger_arg_is_validated()
    		{
    			this.ShouldThrowException<ArgumentNullException>(() =>
					new ModuleLoader(slideConfigRepoMock.Object, widgetMetadataRepoMock.Object, null));
    		}

    		[Test]
    		public void Then_assure_WidgetMetadataRepository_arg_is_validated()
    		{
    			this.ShouldThrowException<ArgumentNullException>(() =>
					new ModuleLoader(slideConfigRepoMock.Object, null, loggerMock.Object));
    		}
    	}

    	[TestFixture]
    	public class When_loading_slides : Shared
    	{
    		[Test]
    		public void Then_assure_Slideshow_arg_is_validated()
    		{
    			this.ShouldThrowException<ArgumentNullException>(() =>
					moduleLoader.LoadSlides(null));
    		}

    		[Test]
    		public void Then_assure_Slideshow_is_created_based_on_config()
    		{
    			var slidesInConfig = GetSlideshowConfiguration();

    			moduleLoader.LoadSlides(slideshowViewModel);

				AssertThatSlidesAreLoadedCorrectly(slidesInConfig);
    		}
    	}

		[TestFixture]
        public class When_loading_slides_and_one_fails_to_load : Shared
        {
            protected override void Before()
            {
            	WidgetsExists();
                One_wronge_SlideConfig_is_added();
                moduleLoader.LoadSlides(slideshowViewModel);
            }

            private void One_wronge_SlideConfig_is_added()
            {
                slideConfigs.Add(new SlideConfiguration(){WidgetType = typeof(FailingWidget).FullName});
            }


            [Test]
            public void Then_assure_rest_of_the_slides_are_loaded()
            {
            	var slidesInConfig = GetSlideshowConfiguration();

            	slideConfigs.RemoveAt(slideConfigs.Count - 1); //Remove the failing one
				AssertThatSlidesAreLoadedCorrectly(slidesInConfig);
            }
        }

        [TestFixture]
        public class When_loading_admin_widgets : Shared
        {
            protected override void Before()
            {
                moduleLoader.LoadAdminWidgets(dockBarViewModel);
            }

            [Test]
            public void Then_assure_DockBarItems_are_created()
            {
				(dockBarViewModel.Items.Count > 1).ShouldBeTrue();
            }

        	[Test]
        	public void Then_assure_DockBarViewModel_args_is_validated()
        	{
        		this.ShouldThrowException<ArgumentNullException>(() =>
					moduleLoader.LoadAdminWidgets(null));
        	}
        }

		public class Shared
		{
			protected Mock<IAsyncRepository<SlideConfiguration>> slideConfigRepoMock;
			protected Mock<IAsyncRepository<WidgetMetadata>> widgetMetadataRepoMock;
			protected ModuleLoader moduleLoader;
			protected Slideshow slideshowViewModel;
			protected SlideConfiguration slideConfig;
			protected List<SlideConfiguration> slideConfigs;
			protected Mock<ILog> loggerMock;
			protected DockBar dockBarViewModel;

			[SetUp]
			public void Setup()
			{
				ViewModelBootstrapperForTests.Initialize();

				slideConfigRepoMock = new Mock<IAsyncRepository<SlideConfiguration>>();
				loggerMock = new Mock<ILog>();

				widgetMetadataRepoMock = new Mock<IAsyncRepository<WidgetMetadata>>();
				WidgetsExists();

				slideConfig = new SlideConfiguration
				{
					Duration = 100,
					Title = "Holidays provider widget",
					WidgetType = typeof(HolidayProviderTestWidget).FullName,
					WidgetConfigurationId = Guid.NewGuid()
				};
				slideConfigs = new List<SlideConfiguration> { slideConfig };
				slideConfigRepoMock.Setup(t => t.BeginGet(It.IsAny<Specification<SlideConfiguration>>())).Raises(
					t => t.GetCompleted += null, new GetCompletedEventArgs<SlideConfiguration>(slideConfigs, new AllSpecification<SlideConfiguration>()));

				moduleLoader = new ModuleLoader(slideConfigRepoMock.Object, widgetMetadataRepoMock.Object, loggerMock.Object);

				slideshowViewModel = new Slideshow();
				dockBarViewModel = new DockBar();

				Before();
			}

			protected virtual void Before()
			{

			}

			protected void WidgetsExists()
			{
				var availalbleWidgets = new List<WidgetMetadata> 
                { 
                    new WidgetMetadata() { Type = typeof(HolidayProviderTestWidget), Tags = new[] { "Admin" } },
					new WidgetMetadata() { Type = typeof(LatestCommitsWidget), Tags = new[] { "VCS" } },
					new WidgetMetadata() { Type = typeof(TaskAdminTestWidget), Name = "Task Administration" }
				};
				
				widgetMetadataRepoMock.Setup(s => s.BeginGet(It.IsAny<Specification<WidgetMetadata>>())).Raises(
					c => c.GetCompleted += null, 
						new GetCompletedEventArgs<WidgetMetadata>(availalbleWidgets, new AllSpecification<WidgetMetadata>()));
			}

			protected void AssertThatSlidesAreLoadedCorrectly(IEnumerable<SlideConfiguration> slidesInConfig)
			{
				foreach (var slideConfig in slidesInConfig)
				{
					slideshowViewModel.Slides.Any(s => s.Title == slideConfig.Title).ShouldBeTrue();
				}
			}

			protected IEnumerable<SlideConfiguration> GetSlideshowConfiguration()
			{
				IEnumerable<SlideConfiguration> slidesInConfig = null;
				slideConfigRepoMock.Object.GetCompleted += (o, e) =>
				{
					slidesInConfig = e.Result;
				};
				slideConfigRepoMock.Object.BeginGet(new AllSpecification<SlideConfiguration>());
				return slidesInConfig;
			}
		}

		public class HolidayProviderTestWidget : Widget
		{

		}

		public class LatestCommitsWidget : Widget
		{
			
		}

		public class TaskAdminTestWidget : Widget
		{
			
		}

		public class FailingWidget : Widget
		{
			public FailingWidget()
			{
				throw new Exception("Widget fails to load because it doesn't longer exist, and then it don't get downloaded");
			}
		}
    }
}
