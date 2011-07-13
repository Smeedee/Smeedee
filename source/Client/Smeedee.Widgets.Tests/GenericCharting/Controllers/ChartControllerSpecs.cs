using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Framework;
using Smeedee.Widgets.GenericCharting.Controllers;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Widgets.Tests.GenericCharting.Controllers
{
    class ChartControllerSpecs
    {

        [TestFixture]
        public class When_controller_is_spawning : Shared
        {

            [Test]
            public void Then_null_viewModelArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as viewModel");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(null, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, downloadStringServiceFake.Object)));
            }

            [Test]
            public void Then_null_timerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as timer");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, null, uIInvokerFake.Object, loadingNotifierFake.Object, downloadStringServiceFake.Object)));
            }

            [Test]
            public void Then_null_uIInvokerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as uIInvoker");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, null, loadingNotifierFake.Object, downloadStringServiceFake.Object)));
            }

            [Test]
            public void Then_null_loadingNotifierArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as loadingNotifier");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, null, downloadStringServiceFake.Object)));
            }

            [Test]
            public void Then_null_downloadStringServiceArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as downloadStringService");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object ,null)));
            }


            //Test for if/when we want to add configuration to the constructor of ChartController

            //[Test]
            //public void Then_null_configurationArgs_should_return_exception()
            //{
            //    Given("");
            //    When("creating new controller with null as configuration");
            //    Then("an ArgumentException should be thrown",
            //         () =>
            //         this.ShouldThrowException<ArgumentException>(
            //            () => new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, downloadStringServiceFake.Object, null)));
            //}

        }
        public class Shared : ScenarioClass
        {
            protected ChartController chartController;
            protected ChartViewModel chartViewModel;

            //protected ChartSettingsViewModel chartSettingsViewModel;
            
            protected Mock<ITimer> timerFake;
            protected Mock<IUIInvoker> uIInvokerFake;
            protected Mock<IProgressbar> loadingNotifierFake;
            protected Mock<IDownloadStringService> downloadStringServiceFake;
           
            
            


            //protected Mock<IRepository<ChartViewModel>> ChartRepositoryFake = new Mock<IRepository<ChartViewModel>>();
            //protected Mock<IRepository<DataPointViewModel>> DataPointRepositoryFake = new Mock<IRepository<DataPointViewModel>>();
            //protected Mock<IRepository<ChartSettingsViewModel>> ChartSettingsRepositoryFake = new Mock<IRepository<ChartSettingsViewModel>>();
            //protected Mock<IRepository<DatabaseViewModel>> DataBaseRepositoryFake = new Mock<IRepository<DatabaseViewModel>>();
            //protected Mock<IRepository<CollectionViewModel>> CollectionRepositoryFake = new Mock<IRepository<CollectionViewModel>>();

            [SetUp]
            public void Setup()
            {
                Scenario("");
                chartViewModel = new ChartViewModel();

                timerFake = new Mock<ITimer>();
                uIInvokerFake = new Mock<IUIInvoker>();
                loadingNotifierFake = new Mock<IProgressbar>();
                downloadStringServiceFake = new Mock<IDownloadStringService>();
           


                //RemoveAllGlobalDependencies.ForAllViewModels();
                //ConfigureGlobalDependencies.ForAllViewModels(config =>
                //{
                //    config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

                //    config.Bind<IRepository<ChartViewModel>>().ToInstance(
                //        ChartRepositoryFake.Object);
                //    config.Bind<IRepository<DataPointViewModel>>().
                //        ToInstance(DataPointRepositoryFake.Object);
                //    config.Bind<IRepository<ChartSettingsViewModel>>().
                //        ToInstance(ChartSettingsRepositoryFake.Object);
                //    config.Bind<IRepository<DatabaseViewModel>>().
                //        ToInstance(DataBaseRepositoryFake.Object);
                //    config.Bind<IRepository<CollectionViewModel>>().
                //        ToInstance(CollectionRepositoryFake.Object);
                //});
            }

            [TearDown]
            public void TearDown()
            {
                //RemoveAllGlobalDependencies.ForAllViewModels();
                StartScenario();
            } 
        }
    }
}
