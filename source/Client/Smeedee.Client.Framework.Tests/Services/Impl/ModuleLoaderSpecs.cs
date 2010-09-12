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
        public class context
        {
            protected Mock<IAsyncRepository<SlideConfiguration>> slideRepoMock;
            protected ModuleLoader Loader;
            protected Slideshow slideshowViewModel;
            protected SlideConfiguration slideConfig;
            protected List<SlideConfiguration> slideConfigs;
            protected Mock<ILog> loggerMock;

            [SetUp]
            public void Setup()
            {
                ViewModelBootstrapperForTests.Initialize();
                slideRepoMock = new Mock<IAsyncRepository<SlideConfiguration>>();
                loggerMock = new Mock<ILog>();

                slideConfig = new SlideConfiguration
                {
                    Duration = 100,
                    Title = "Tittel",
                    WidgetType = typeof (TestWidget).FullName,
                    WidgetConfigurationId = Guid.NewGuid()
                };
                slideConfigs = new List<SlideConfiguration> {slideConfig};
                slideRepoMock.Setup(t => t.BeginGet(It.IsAny<Specification<SlideConfiguration>>())).Raises(

                    t => t.GetCompleted += null, new GetCompletedEventArgs<SlideConfiguration>(slideConfigs, new AllSpecification<SlideConfiguration>()));
                Loader = new ModuleLoader(slideRepoMock.Object, loggerMock.Object);
                before();
            }

            protected virtual void before()
            {
                
            }

            protected void MEF_Is_Finished_loading_available_widgets()
            {
                Loader.availalbleWidgets = new List<Lazy<Widget, IWidgetMetadata>>
                {new Lazy<Widget, IWidgetMetadata>(() => new TestWidget(), null)};
                Loader.OnImportsSatisfied();
            }protected void MEF_Is_Finished_loading_available_widgets_with_admin_tag()
            {
                Loader.availalbleWidgets = new List<Lazy<Widget, IWidgetMetadata>> 
                { new Lazy<Widget, IWidgetMetadata>(() => new TestWidget() { Title = "Holidays" }, 
                    new WidgetMetadata() { Tags = new[] { "Admin" } }) };
                Loader.OnImportsSatisfied();
            }
        }


        [TestFixture]
        public class when_loading_slides_and_mef_types_are_ready : context
        {
            protected override void before()
            {
                slideshowViewModel = new Slideshow();
                MEF_Is_Finished_loading_available_widgets();
                Loader.LoadSlides(slideshowViewModel);
            }

            [Test]
            public void Assure_slideConfigurations_are_fetched()
            {
                slideRepoMock.Verify(r => r.BeginGet(All.ItemsOf<SlideConfiguration>()));
            }

            [Test]
            public void Assure_slide_is_instantiated()
            {
                slideshowViewModel.Slides.Count.ShouldBe(1);
                slideshowViewModel.Slides.ElementAt(0).Title.ShouldBe(slideConfig.Title);
                slideshowViewModel.Slides.ElementAt(0).SecondsOnScreen.ShouldBe(slideConfig.Duration);
                slideshowViewModel.Slides.ElementAt(0).Widget.GetType().FullName.ShouldBe(slideConfig.WidgetType);
                slideshowViewModel.Slides.ElementAt(0).Widget.Configuration.Id.ShouldBe(slideConfig.WidgetConfigurationId);
            }
        }

        [TestFixture]
        public class when_loading_slides_and_one_fails_to_load : context
        {
            protected override void before()
            {
                slideshowViewModel = new Slideshow();
                MEF_Is_Finished_loading_available_widgets();
                one_slideConfig_is_wrong();
                Loader.LoadSlides(slideshowViewModel);
            }

            

            private void one_slideConfig_is_wrong()
            {
                slideConfigs.Insert(0, new SlideConfiguration(){WidgetType = "error up in here!"});
            }


            [Test]
            public void Assure_rest_of_the_slides_are_loaded()
            {
                slideshowViewModel.Slides.Count.ShouldBe(1);
            }

            [Test]
            public void Assure_failiure_is_logged()
            {
                loggerMock.Verify(t => t.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Once());
            }   
        }

        [TestFixture]
        public class when_loading_admin_widgets : context
        {
            protected override void before()
            {
                slideshowViewModel = new Slideshow();
                MEF_Is_Finished_loading_available_widgets_with_admin_tag();
                Loader.LoadSlides(slideshowViewModel);
            }

            [Test]
            public void Assure_one_DockBarItem_is_created()
            {
                //TODO: Find a way to check that the admin widget is added
                loggerMock.Verify(t => t.WriteEntry(It.IsAny<ErrorLogEntry>()), Times.Never());
            }

            [Test]
            public void Assure_admin_widgets_are_not_loaded_to_slideshow()
            {
                slideshowViewModel.Slides.First().Title.ShouldNotBe("Holidays");
            }
        }

    }

    public class TestWidget : Widget
    {

    }
}
