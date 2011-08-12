using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.ViewModel
{
    public class WebSnapshotSettingsViewModelSpec
    {

        [TestFixture]
        public class When_created : Shared
        {

            [Test]
            public void Then_assure_it_has_a_save_command()
            {
                webSnapshotSettingsViewModel.Save.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_it_has_a_reload_command()
            {
                webSnapshotSettingsViewModel.ReloadSettings.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_available_images_is_not_null()
            {
                webSnapshotSettingsViewModel.AvailableImages.ShouldNotBeNull();
            }

            [Test]
            public void Then_assure_available_images_uri_is_not_null()
            {
                webSnapshotSettingsViewModel.AvailableImagesUri.ShouldNotBeNull();
            }
        }


        public class Shared : ScenarioClass
        {
            protected WebSnapshotSettingsViewModel webSnapshotSettingsViewModel;
            protected const string ERROR_MESSAGE = "Invalid URL!";
            protected const string INPUT_MESSAGE = "Enter URL here";

            [SetUp]
            public void Setup()
            {
                webSnapshotSettingsViewModel = new WebSnapshotSettingsViewModel();
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }

        }
    }
}
