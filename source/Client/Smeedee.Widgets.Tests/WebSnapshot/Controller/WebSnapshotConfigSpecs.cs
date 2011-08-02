using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.WebSnapshot.Controllers;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.Controller
{
    public class SharedConfig : ScenarioClass
    {
        protected static WebSnapshotConfig webSnapshotConfig;
        protected static Configuration configuration;

        protected Context config_is_created = () => webSnapshotConfig = new WebSnapshotConfig(configuration);
    }

    [TestFixture]
    public class When_creating_default_configuration
    {
        [Test]
        public void WebSnapshotConfig_should_have_default_configuration()
        {
            var config = WebSnapshotConfig.NewDefaultConfiguration();
            config.ContainsSetting(WebSnapshotConfig.url).ShouldBeTrue();
            config.ContainsSetting(WebSnapshotConfig.coordinateX).ShouldBeTrue();
            config.ContainsSetting(WebSnapshotConfig.coordinateY).ShouldBeTrue();
            config.ContainsSetting(WebSnapshotConfig.rectangleHeight).ShouldBeTrue();
            config.ContainsSetting(WebSnapshotConfig.rectangleWidth).ShouldBeTrue();
            config.ContainsSetting(WebSnapshotConfig.timestamp).ShouldBeTrue();
            
            config.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class When_setting_string_values : SharedConfig
    {
        [Test]
        public void It_should_be_possible_to_set_URL()
        {
            var url = "http://my.url";
            
            Given(config_is_created);
            When("setting ChartName", () => webSnapshotConfig.URL = url);
            Then("configuration should have that value",
                 () => configuration.GetSetting(WebSnapshotConfig.url).Value.ShouldBe(url));
        }

        [Test]
        public void It_should_be_possible_to_change_URL()
        {
            Given(config_is_created).
                And("URL is set", () => webSnapshotConfig.URL = "something");
            When("changing URL", () => webSnapshotConfig.URL = "else");
            Then("configuration should be updated",
                () =>
                {
                    configuration.GetSetting(webSnapshotConfig.URL).HasMultipleValues.ShouldBeFalse();
                    configuration.GetSetting(webSnapshotConfig.URL).Value.ShouldBe("else");
                });
        }
    }

}
