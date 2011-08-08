using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.WebSnapshot.Controllers;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebSnapshot.Controller
{
    public class WebSnapshotConfigSpecs
    {
        public class Shared : ScenarioClass
        {
            protected static WebSnapshotConfig webSnapshotConfig;
            protected static Configuration configuration;

            protected Context config_has_been_created = () => webSnapshotConfig = new WebSnapshotConfig(configuration);

            [SetUp]
            public void SetUp()
            {
                Scenario("");

                configuration = WebSnapshotConfig.NewDefaultConfiguration();
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
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
        public class When_setting_values : Shared
        {
            [Test]
            public void It_should_be_possible_to_set_TaskName()
            {
                var taskName = "default task";

                Given(config_has_been_created);
                When("setting taskName", () => webSnapshotConfig.TaskName = taskName);
                Then("configuration should have that value",
                     () => configuration.GetSetting(WebSnapshotConfig.taskname).Value.ShouldBe(taskName));
            }

            [Test]
            public void It_should_be_possible_to_change_TaskName()
            {
                Given(config_has_been_created).
                    And("TaskName is set", () => webSnapshotConfig.TaskName = "something");
                When("changing TaskName", () => webSnapshotConfig.TaskName = "else");
                Then("configuration should be updated",
                     () =>
                         {
                             configuration.GetSetting(WebSnapshotConfig.taskname).HasMultipleValues.ShouldBeFalse();
                             configuration.GetSetting(WebSnapshotConfig.taskname).Value.ShouldBe("else");
                         });
            }

            [Test]
            public void It_should_be_possible_to_set_CoordinateX()
            {
                var CoordinateX = "default x-coordinate";

                Given(config_has_been_created);
                When("setting CoordinateX", () => webSnapshotConfig.CoordinateX = CoordinateX);
                Then("configuration should have that value",
                     () => configuration.GetSetting(WebSnapshotConfig.coordinateX).Value.ShouldBe(CoordinateX));
            }

            [Test]
            public void It_should_be_possible_to_change_CoordinateX()
            {
                Given(config_has_been_created).
                    And("CoordinateX is set", () => webSnapshotConfig.CoordinateX = "something");
                When("changing CoordinateX", () => webSnapshotConfig.CoordinateX = "else");
                Then("configuration should be updated",
                     () =>
                         {
                             configuration.GetSetting(WebSnapshotConfig.coordinateX).HasMultipleValues.ShouldBeFalse();
                             configuration.GetSetting(WebSnapshotConfig.coordinateX).Value.ShouldBe("else");
                         });
            }

            [Test]
            public void It_should_be_possible_to_set_CoordinateY()
            {
                var CoordinateY = "default y-koordinate";

                Given(config_has_been_created);
                When("setting CoordinateY", () => webSnapshotConfig.CoordinateY = CoordinateY);
                Then("configuration should have that value",
                     () => configuration.GetSetting(WebSnapshotConfig.coordinateY).Value.ShouldBe(CoordinateY));
            }

            [Test]
            public void It_should_be_possible_to_change_CoordinateY()
            {
                Given(config_has_been_created).
                    And("CoordinateY is set", () => webSnapshotConfig.CoordinateY = "something");
                When("changing CoordinateY", () => webSnapshotConfig.CoordinateY = "else");
                Then("configuration should be updated",
                     () =>
                         {
                             configuration.GetSetting(WebSnapshotConfig.coordinateY).HasMultipleValues.ShouldBeFalse();
                             configuration.GetSetting(WebSnapshotConfig.coordinateY).Value.ShouldBe("else");
                         });
            }

            [Test]
            public void It_should_be_possible_to_set_RectangleHeight()
            {
                var RectangleHeight = "default rectangle height";

                Given(config_has_been_created);
                When("setting RectangleHeight", () => webSnapshotConfig.RectangleHeight = RectangleHeight);
                Then("configuration should have that value",
                     () => configuration.GetSetting(WebSnapshotConfig.rectangleHeight).Value.ShouldBe(RectangleHeight));
            }

            [Test]
            public void It_should_be_possible_to_change_RectangleHeight()
            {
                Given(config_has_been_created).
                    And("RectangleHeight is set", () => webSnapshotConfig.RectangleHeight = "something");
                When("changing RectangleHeight", () => webSnapshotConfig.RectangleHeight = "else");
                Then("configuration should be updated",
                     () =>
                         {
                             configuration.GetSetting(WebSnapshotConfig.rectangleHeight).HasMultipleValues.ShouldBeFalse
                                 ();
                             configuration.GetSetting(WebSnapshotConfig.rectangleHeight).Value.ShouldBe("else");
                         });
            }

            [Test]
            public void It_should_be_possible_to_set_RectangleWidth()
            {
                var RectangleWidth = "default rectangle Width";

                Given(config_has_been_created);
                When("setting RectangleWidth", () => webSnapshotConfig.RectangleWidth = RectangleWidth);
                Then("configuration should have that value",
                     () => configuration.GetSetting(WebSnapshotConfig.rectangleWidth).Value.ShouldBe(RectangleWidth));
            }

            [Test]
            public void It_should_be_possible_to_change_RectangleWidth()
            {
                Given(config_has_been_created).
                    And("RectangleWidth is set", () => webSnapshotConfig.RectangleWidth = "something");
                When("changing RectangleWidth", () => webSnapshotConfig.RectangleWidth = "else");
                Then("configuration should be updated",
                     () =>
                         {
                             configuration.GetSetting(WebSnapshotConfig.rectangleWidth).HasMultipleValues.ShouldBeFalse();
                             configuration.GetSetting(WebSnapshotConfig.rectangleWidth).Value.ShouldBe("else");
                         });
            }

            [Test]
            public void It_should_be_possible_to_set_Timestamp()
            {
                var Timestamp = "default timestamp";

                Given(config_has_been_created);
                When("setting Timestamp", () => webSnapshotConfig.Timestamp = Timestamp);
                Then("configuration should have that value",
                     () => configuration.GetSetting(WebSnapshotConfig.timestamp).Value.ShouldBe(Timestamp));
            }

            [Test]
            public void It_should_be_possible_to_change_Timestamp()
            {
                Given(config_has_been_created).
                    And("Timestamp is set", () => webSnapshotConfig.Timestamp = "something");
                When("changing Timestamp", () => webSnapshotConfig.Timestamp = "else");
                Then("configuration should be updated",
                     () =>
                         {
                             configuration.GetSetting(WebSnapshotConfig.timestamp).HasMultipleValues.ShouldBeFalse();
                             configuration.GetSetting(WebSnapshotConfig.timestamp).Value.ShouldBe("else");
                         });
            }

            [Test]
            public void It_should_be_possible_to_set_IsConfigured()
            {
                Given(config_has_been_created);
                When("IsConfigured is set", () => webSnapshotConfig.IsConfigured = true);
                Then("IsConfigured should be set", () => configuration.IsConfigured.ShouldBeTrue());
            }

            [Test]
            public void It_should_be_possible_to_change_IsConfigured()
            {
                Given(config_has_been_created).
                    And("IsConfigured has been set", () => webSnapshotConfig.IsConfigured = true);
                When("IsConfigured is changed", () => webSnapshotConfig.IsConfigured = false);
                Then("configuration should be updated", () => configuration.IsConfigured.ShouldBeFalse());
            }
        }

        [TestFixture]
        public class When_getting_values : Shared
        {
            [Test]
            public void It_should_be_possible_to_get_TaskName()
            {
                Given(config_has_been_created).
                    And("TaskName is set", () => webSnapshotConfig.TaskName = "TaskName");
                When("we want to get TaskName");
                Then("correct TaskName should be returned", () => webSnapshotConfig.TaskName.ShouldBe("TaskName"));
            }

            [Test]
            public void It_should_be_possible_to_get_CoordinateX()
            {
                Given(config_has_been_created).
                    And("CoordinateX is set", () => webSnapshotConfig.CoordinateX = "CoordinateX");
                When("we want to get CoordinateX");
                Then("correct CoordinateX should be returned",
                     () => webSnapshotConfig.CoordinateX.ShouldBe("CoordinateX"));
            }

            [Test]
            public void It_should_be_possible_to_get_CoordinateY()
            {
                Given(config_has_been_created).
                    And("CoordinateY is set", () => webSnapshotConfig.CoordinateY = "CoordinateY");
                When("we want to get CoordinateY");
                Then("correct CoordinateY should be returned",
                     () => webSnapshotConfig.CoordinateY.ShouldBe("CoordinateY"));
            }

            [Test]
            public void It_should_be_possible_to_get_RectangleHeight()
            {
                Given(config_has_been_created).
                    And("RectangleHeight is set", () => webSnapshotConfig.RectangleHeight = "RectangleHeight");
                When("we want to get RectangleHeight");
                Then("correct RectangleHeight should be returned",
                     () => webSnapshotConfig.RectangleHeight.ShouldBe("RectangleHeight"));
            }

            [Test]
            public void It_should_be_possible_to_get_RectangleWidth()
            {
                Given(config_has_been_created).
                    And("RectangleWidth is set", () => webSnapshotConfig.RectangleWidth = "RectangleWidth");
                When("we want to get RectangleWidth");
                Then("correct RectangleWidth should be returned",
                     () => webSnapshotConfig.RectangleWidth.ShouldBe("RectangleWidth"));
            }

            [Test]
            public void It_should_be_possible_to_get_Timestamp()
            {
                Given(config_has_been_created).
                    And("Timestamp is set", () => webSnapshotConfig.Timestamp = "Timestamp");
                When("we want to get Timestamp");
                Then("correct Timestamp should be returned",
                     () => webSnapshotConfig.Timestamp.ShouldBe("Timestamp"));
            }

            [Test]
            public void It_should_be_possible_to_get_IsConfigured()
            {
                Given(config_has_been_created).
                    And("IsConfigured is set", () => webSnapshotConfig.IsConfigured = true);
                When("we want to get IsConfigured");
                Then("correct IsConfigured should be returned", () => webSnapshotConfig.IsConfigured.ShouldBeTrue());
            }
        }
    }
}
