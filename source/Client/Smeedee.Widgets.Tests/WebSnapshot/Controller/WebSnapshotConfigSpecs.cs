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
            config.ContainsSetting(WebSnapshotConfig.taskname).ShouldBeTrue();
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
        public void It_should_be_possible_to_set_TaskName()
        {
            var taskName = "default task";
            
            Given(config_is_created);
            When("setting taskName", () => webSnapshotConfig.TaskName = taskName);
            Then("configuration should have that value",
                 () => configuration.GetSetting(WebSnapshotConfig.taskname).Value.ShouldBe(taskName));
        }

        [Test]
        public void It_should_be_possible_to_change_TaskName()
        {
            Given(config_is_created).
                And("TaskName is set", () => webSnapshotConfig.TaskName = "something");
            When("changing TaskName", () => webSnapshotConfig.TaskName = "else");
            Then("configuration should be updated",
                () =>
                {
                    configuration.GetSetting(webSnapshotConfig.TaskName).HasMultipleValues.ShouldBeFalse();
                    configuration.GetSetting(webSnapshotConfig.TaskName).Value.ShouldBe("else");
                });
        }
    }

}
