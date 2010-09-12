using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Client.Framework.Tests.Controller
{
    public class WidgetControllerSpecs
    {
        public class Shared
        {
            protected Mock<IPersistDomainModelsAsync<Configuration>> configPersisterFake;
            protected Widget viewModel;
            protected WidgetController controller;
        }


        [TestFixture]
        public class when_SaveSettings : Shared
        {
            [SetUp]
            public void Setup()
            {
                ViewModelBootstrapperForTests.Initialize();
                viewModel = new Widget();
                configPersisterFake = new Mock<IPersistDomainModelsAsync<Configuration>>();
                controller = new WidgetController(viewModel, configPersisterFake.Object);
                viewModel.SaveSettings.Execute();
            }

            [Test]
            public void Assure_config_is_saved()
            {
                configPersisterFake.Verify(p => p.Save(It.IsAny<Configuration>()), Times.Once());
            }

            [Test]
            public void Assure_progressBar_is_shown()
            {
                viewModel.SettingsProgressbar.IsVisible.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class when_save_is_completed : Shared
        {
            private bool settingsCommandExecuted = false;

            [SetUp]
            public  void Setup()
            {               
                ViewModelBootstrapperForTests.Initialize();
                viewModel = new Widget();
                configPersisterFake = new Mock<IPersistDomainModelsAsync<Configuration>>();
                controller = new WidgetController(viewModel, configPersisterFake.Object);
                viewModel.SaveSettings.Execute();

                viewModel.Settings.Execute();
                viewModel.SaveSettings.Execute();
                viewModel.Settings.ExecuteDelegate = () => settingsCommandExecuted = true;

                configPersisterFake.Raise(p => p.SaveCompleted += null, new SaveCompletedEventArgs());
            }

            [Test]
            public void Assure_progress_bar_is_hidden()
            {
                viewModel.SettingsProgressbar.IsVisible.ShouldBeFalse();
            }

            [Test]
            public void Assure_settingsView_is_exited()
            {
                settingsCommandExecuted.ShouldBeTrue();
            }
        }

        [TestFixture]
        public class when_save_fails : Shared
        {
            private Mock<IPersistDomainModelsAsync<Configuration>> configPersisterFake;

            [SetUp]
            public void Setup()
            {
                ViewModelBootstrapperForTests.Initialize();
                viewModel = new Widget();
                configPersisterFake = new Mock<IPersistDomainModelsAsync<Configuration>>();
                controller = new WidgetController(viewModel, configPersisterFake.Object);
                viewModel.SaveSettings.Execute();
                configPersisterFake.Raise(p => p.SaveCompleted += null, new SaveCompletedEventArgs(new Exception()));

            }

            [Test]
            public void Assure_failure_is_shown_in_errorInfo()
            {
                viewModel.ErrorInfo.HasError.ShouldBe(true);
                viewModel.ErrorInfo.ErrorMessage.ShouldBe("Failed to save settings :(");
            }
        }
    }
}
