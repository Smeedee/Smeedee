using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.WebSnapshot.Controllers;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.Controllers
{
    public class WebSnapshotControllerSpec
    {

        [TestFixture]
        public class When_get_default_configuration : Shared
        {
            private Configuration config;

            
            public override void Before()
            {
                config = WebSnapshotController.GetDefaultConfiguration();
            }

            [Test]
            public void Then_assure_config_has_a_name()
            {
                config.Name.ShouldBe("websnapshot");
            }

            [Test]
            public void Then_assure_it_can_store_a_URL()
            {
                config.ContainsSetting("url").ShouldBeTrue();
            }

            [Test]
            public void Then_assure_it_has_a_default_refresh_interval()
            {
                config.ContainsSetting("refresh-interval").ShouldBeTrue();
                config.GetSetting("refresh-interval").Value.ShouldBe("15");
            }

     
        }

        [TestFixture]
        public class When_update_configuration : Shared
        {
            private Configuration updatedConfig;

            public override void Before()
            {
                updatedConfig = WebSnapshotController.GetDefaultConfiguration();
            }

            [Test]
            public void Then_assure_Settingsviewmodel_URL_is_changed()
            {
                updatedConfig.ChangeSetting("url", "http://smeedee.org");
                webSnapshotController.UpdateConfiguration(updatedConfig);
                webSnapshotSettingsViewModel.InputUrl.ShouldBe("http://smeedee.org");
            }

            [Test]
            public void Then_assure_configuration_is_validated()
            {
                var invalidConfig = new Configuration("websnapshot");
                
                this.ShouldThrowException<ArgumentNullException>(() =>
                    webSnapshotController.UpdateConfiguration(null));
                this.ShouldThrowException<ArgumentException>(() =>
                    webSnapshotController.UpdateConfiguration(invalidConfig));

                invalidConfig.NewSetting("url", "");
                this.ShouldThrowException<ArgumentException>(() =>
                    webSnapshotController.UpdateConfiguration(invalidConfig));

                invalidConfig.ChangeSetting("refresh-interval", "warp factor 9");
                this.ShouldThrowException<ArgumentException>(() =>
                    webSnapshotController.UpdateConfiguration(invalidConfig));
            }
        }

        [TestFixture]
        public class When_save_configuration : Shared
        {
            [SetUp]
            public void Setup()
            {
                webSnapshotViewModel = new WebSnapshotViewModel();
                webSnapshotSettingsViewModel = new WebSnapshotSettingsViewModel();
                webSnapshotController = new WebSnapshotController(webSnapshotViewModel, webSnapshotSettingsViewModel, WebSnapshotController.GetDefaultConfiguration(), new StandardTimer());
            }

            [Test]
            public void Then_assure_URL_is_extracted_from_Settingsviewmodel()
            {
                webSnapshotSettingsViewModel.InputUrl = "http://smeedee.org";
                var config = webSnapshotController.SaveConfiguration();
                config.GetSetting("url").Value.ShouldBe("http://smeedee.org");
            }

            [Test]
            public void Then_assure_refresh_interval_is_extracted_from_Settingsviewmodel()
            {
                webSnapshotSettingsViewModel.RefreshInterval = 30;
                var config = webSnapshotController.SaveConfiguration();
                config.GetSetting("refresh-interval").Value.ShouldBe("30");
            }
        }


        public class Shared
        {
            protected Mock<ITimer> timerFake;
            protected WebSnapshotViewModel webSnapshotViewModel;
            protected WebSnapshotSettingsViewModel webSnapshotSettingsViewModel;
            protected WebSnapshotController webSnapshotController;
            
            [SetUp]
            public void Setup()
            {
                timerFake = new Mock<ITimer>();
                webSnapshotViewModel = new WebSnapshotViewModel();
                webSnapshotSettingsViewModel = new WebSnapshotSettingsViewModel();
                webSnapshotController = new WebSnapshotController(webSnapshotViewModel, webSnapshotSettingsViewModel, WebSnapshotController.GetDefaultConfiguration(), timerFake.Object);
                Before();
            }

            public virtual void Before()
            {
                
            }


        }

    }
}
