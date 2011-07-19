using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.GenericCharting.Controllers;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.GenericCharting.Controllers
{
    class ChartConfigSpecs
    {

        [TestFixture]
        public class When_setting_string_values : Shared
        {
            [Test]
            public void It_should_be_possible_to_set_ChartName()
            {
                Given(chart_config_has_been_made);
                When("setting ChartName", () => chartConfig.ChartName = "ChartName");
                Then("configuration should have that value",
                     () => configuration.GetSetting(ChartConfig.chart_setting_name).Value.ShouldBe("ChartName"));
            }

            [Test]
            public void It_should_be_possible_to_change_ChartName()
            {
                Given(chart_config_has_been_made).
                    And("ChartName is set", () => chartConfig.ChartName = "something");
                When("changing ChartName", () => chartConfig.ChartName = "else");
                Then("configuration should be updated", 
                    () =>
                        {
                            configuration.GetSetting(ChartConfig.chart_setting_name).HasMultipleValues.ShouldBeFalse();
                            configuration.GetSetting(ChartConfig.chart_setting_name).Value.ShouldBe("else");
                        });
            }
        }

   

        public class Shared : ScenarioClass
        {

            protected Context chart_config_has_been_made = () => CreateChartConfig();

            protected static Configuration configuration;
            protected static ChartConfig chartConfig;
            
            protected static void CreateChartConfig()
            {
                chartConfig = new ChartConfig(configuration);
            }

            [SetUp]
            public void Setup()
            {
                Scenario("");
                configuration = new Configuration();
                chartConfig = null;
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
