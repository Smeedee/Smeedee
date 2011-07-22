using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Widgets.WebSnapshot.Controllers;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Dsl.GivenWhenThen;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.Tests.WebSnapshot.Controller
{
    public class Shared : ScenarioClass
    {
        protected static WebSnapshotController controller;
        protected static WebSnapshotViewModel viewModel;
        protected static WebSnapshotSettingsViewModel settingsViewModel;
        //protected static Configuration config;
        protected static Mock<ITimer> timerFake;
        protected static Mock<ILog> logFake;
        protected static IUIInvoker uiInvoker;
        protected static Mock<IProgressbar> loadingNotifierFake;
        protected static Mock<IRepository<DomainModel.WebSnapshot.WebSnapshot>> repositoryFake;

        [SetUp]
        public void SetUp()
        {
            Scenario("");
            viewModel = new WebSnapshotViewModel();
            settingsViewModel = new WebSnapshotSettingsViewModel();
            //config = new Configuration();
            timerFake = new Mock<ITimer>();
            logFake = new Mock<ILog>();
            uiInvoker = new NoUIInvokation();
            loadingNotifierFake = new Mock<IProgressbar>();
            repositoryFake = new Mock<IRepository<DomainModel.WebSnapshot.WebSnapshot>>();


            //controller = new WebSnapshotController(viewModel, settingsViewModel, timerFake.Object, logFake.Object, uiInvoker, loadingNotifierFake.Object, repositoryFake.Object);
        
        }
        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
        
    }
    [TestFixture]
    public class When_Spawned : Shared
    {

        [Test]
        public void assure_we_get_data_from_repo()
        {
            //controller.
        }
    }
}
