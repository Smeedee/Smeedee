using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.GenericCharting.Controllers;
using Smeedee.Widgets.GenericCharting.ViewModels;
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

            [Test]
            public void It_should_be_possible_to_set_XAxisName()
            {
                Given(chart_config_has_been_made);
                When("setting XAxisName", () => chartConfig.XAxisName = "XAxisName");
                Then("configuration should have that value",
                     () => configuration.GetSetting(ChartConfig.x_axis_setting_name).Value.ShouldBe("XAxisName"));
            }

            [Test]
            public void It_should_be_possible_to_change_XAxisName()
            {
                Given(chart_config_has_been_made).
                    And("XAxisName is set", () => chartConfig.XAxisName = "something");
                When("changing XAxisName", () => chartConfig.XAxisName = "else");
                Then("configuration should be updated",
                    () =>
                    {
                        configuration.GetSetting(ChartConfig.x_axis_setting_name).HasMultipleValues.ShouldBeFalse();
                        configuration.GetSetting(ChartConfig.x_axis_setting_name).Value.ShouldBe("else");
                    });
            }

            [Test]
            public void It_should_be_possible_to_set_YAxisName()
            {
                Given(chart_config_has_been_made);
                When("setting YAxisName", () => chartConfig.YAxisName = "YAxisName");
                Then("configuration should have that value",
                     () => configuration.GetSetting(ChartConfig.y_axis_setting_name).Value.ShouldBe("YAxisName"));
            }

            [Test]
            public void It_should_be_possible_to_change_YAxisName()
            {
                Given(chart_config_has_been_made).
                    And("YAxisName is set", () => chartConfig.YAxisName = "something");
                When("changing YAxisName", () => chartConfig.YAxisName = "else");
                Then("configuration should be updated",
                    () =>
                    {
                        configuration.GetSetting(ChartConfig.y_axis_setting_name).HasMultipleValues.ShouldBeFalse();
                        configuration.GetSetting(ChartConfig.y_axis_setting_name).Value.ShouldBe("else");
                    });
            }

            [Test]
            public void It_should_be_possible_to_set_XAxisType()
            {
                Given(chart_config_has_been_made);
                When("setting XAxisType", () => chartConfig.XAxisType = "XAxisType");
                Then("configuration should have that value",
                     () => configuration.GetSetting(ChartConfig.x_axis_setting_type).Value.ShouldBe("XAxisType"));
            }

            [Test]
            public void It_should_be_possible_to_change_XAxisType()
            {
                Given(chart_config_has_been_made).
                    And("XAxisType is set", () => chartConfig.XAxisType = "something");
                When("changing XAxisType", () => chartConfig.XAxisType = "else");
                Then("configuration should be updated",
                    () =>
                    {
                        configuration.GetSetting(ChartConfig.x_axis_setting_type).HasMultipleValues.ShouldBeFalse();
                        configuration.GetSetting(ChartConfig.x_axis_setting_type).Value.ShouldBe("else");
                    });
            }

            [Test]
            public void It_should_be_possible_to_set_IsConfigured()
            {
                Given(chart_config_has_been_made);
                When("IsConfigured is set", () => chartConfig.IsConfigured = true);
                Then("IsConfigured should be set", () => configuration.IsConfigured.ShouldBeTrue());
            }

            [Test]
            public void It_should_be_possible_to_change_IsConfigured()
            {
                Given(chart_config_has_been_made).
                    And("IsConfigured has been set", () => chartConfig.IsConfigured = true);
                When("IsConfigured is changed", () => chartConfig.IsConfigured = false);
                Then("configuration should be updated", () => configuration.IsConfigured.ShouldBeFalse());
            }
        }

        [TestFixture]
        public class When_getting_string_values : Shared
        {
            [Test]
            public void It_should_be_possible_to_get_ChartName()
            {
                Given(chart_config_has_been_made).
                    And("ChartName is set", () => chartConfig.ChartName = "ChartName");
                When("we want to get ChartName");
                Then("correct ChartName should be returned", () => chartConfig.ChartName.ShouldBe("ChartName"));
            }

            [Test]
            public void It_should_be_possible_to_get_XAxisName()
            {
                Given(chart_config_has_been_made).
                    And("XAxisName is set", () => chartConfig.XAxisName = "XAxisName");
                When("we want to get XAxisName");
                Then("correct XAxisName should be returned", () => chartConfig.XAxisName.ShouldBe("XAxisName"));
            }

            [Test]
            public void It_should_be_possible_to_get_YAxisName()
            {
                Given(chart_config_has_been_made).
                    And("YAxisName is set", () => chartConfig.YAxisName = "YAxisName");
                When("we want to get YAxisName");
                Then("correct YAxisName should be returned", () => chartConfig.YAxisName.ShouldBe("YAxisName"));
            }

            [Test]
            public void It_should_be_possible_to_get_XAxisType()
            {
                Given(chart_config_has_been_made).
                    And("XAxisType is set", () => chartConfig.XAxisType = "XAxisType");
                When("we want to get XAxisType");
                Then("correct XAxisType should be returned", () => chartConfig.XAxisType.ShouldBe("XAxisType"));
            }

            [Test]
            public void It_should_be_possible_to_get_IsConfigured()
            {
                Given(chart_config_has_been_made).
                    And("IsConfigured is set", () => chartConfig.IsConfigured = true);
                When("we want to get IsConfigured");
                Then("correct IsConfigured should be returned", () => chartConfig.IsConfigured.ShouldBeTrue());
            }



            [Test]
            public void IsValid_should_return_false_when_no_seriesConfig()
            {
                Given(chart_config_has_been_made);
                When("no series has been made or added");
                Then("IsValid should return false", () => chartConfig.IsValid.ShouldBeFalse());
            }

            [Test]
            public void IsValid_should_return_false_when_no_dataset()
            {
                Given(chart_config_has_been_made).
                    And("seriesConfig is empty", () => seriesConfig.Clear());
                When(setting_series);
                Then("IsValid should return false", () => chartConfig.IsValid.ShouldBeFalse());
            }

            [Test]
            public void IsValid_should_return_true_when_seriesConfig_contains_a_series()
            {
                Given(chart_config_has_been_made).
                    And("seriesConfig has one entry", () => seriesConfig.Add(series1));
                When(setting_series);
                Then("IsValid should return false", () => chartConfig.IsValid.ShouldBeTrue());
            }

            [Test]
            public void IsValid_should_return_false_if_no_dataset_is_show()
            {
                
            }
           
        }

        [TestFixture]
        public class When_setting_series_config : Shared
        {
            [Test]
            public void An_empty_list_of_series_should_result_in_empty_configuration_entry()
            {
                Given(chart_config_has_been_made).
                    And("seriesConfig is empty", () => seriesConfig.Clear());
                When(setting_series);
                Then("configuration should not contain any series", () =>
                        {
                            var setting = configuration.GetSetting(ChartConfig.series_setting_name);
                            setting.Vals.Count().ShouldBe(0);
                        });
            }

            [Test]
            public void One_series_should_be_set_in_configuration()
            {
                Given(chart_config_has_been_made).
                    And("seriesConfig has one entry", () => seriesConfig.Add(series1));
                When(setting_series);
                Then("configuration should contain the given series", () =>
                    CompareConfigurationToSeriesConfigViewModel(0, series1));
            }

            [Test]
            public void Two_series_should_be_set_in_configuration()
            {
                Given(chart_config_has_been_made).
                    And("seriesConfig has two entries", () =>
                    {
                        seriesConfig.Add(series1);
                        seriesConfig.Add(series2);
                    });
                When(setting_series);
                Then("configuration should contain the two series", () =>
                    {
                        CompareConfigurationToSeriesConfigViewModel(0, series1);
                        CompareConfigurationToSeriesConfigViewModel(1, series2);
                    });
            }

            [Test]
            public void Setting_series_twice_in_a_row_it_should_remain_the_same()
            {
                Given(chart_config_has_been_made).
                    And("seriesConfig has one entry", () => seriesConfig.Add(series1)).
                    And(series_has_been_set);
                When(setting_series);
                Then("configuration should contain the given series", () =>
                    {
                        configuration.GetSetting(ChartConfig.series_setting_name).Vals.Count().ShouldBe(1);
                        CompareConfigurationToSeriesConfigViewModel(0, series1);
                    });
            }



            private static void CompareConfigurationToSeriesConfigViewModel(int row, SeriesConfigViewModel series)
            {
                var name = configuration.GetSetting(ChartConfig.series_setting_name);
                var collection = configuration.GetSetting(ChartConfig.series_setting_collection);
                var database = configuration.GetSetting(ChartConfig.series_setting_database);
                var legend = configuration.GetSetting(ChartConfig.series_setting_legend);
                var action = configuration.GetSetting(ChartConfig.series_setting_action);
                var type = configuration.GetSetting(ChartConfig.series_setting_type);

                name.Vals.ToList()[row].ShouldBe(series.Name);
                collection.Vals.ToList()[row].ShouldBe(series.Collection);
                database.Vals.ToList()[row].ShouldBe(series.Database);
                legend.Vals.ToList()[row].ShouldBe(series.Legend);
                action.Vals.ToList()[row].ShouldBe(series.Action);
                type.Vals.ToList()[row].ShouldBe(series.ChartType);
            }

            private Context series_has_been_set = SetSeries;
            

            
        }

        [TestFixture]
        public class When_getting_series_config : Shared
        {
            private static IList<SeriesConfigViewModel> series = null;

            protected override void Before()
            {
                series = null;
            }

            [Test]
            public void Empty_configuration_should_return_empty_list()
            {
                Given(chart_config_has_been_made);
                When(getting_series);
                Then("series should be empty", () => series.Count.ShouldBe(0));
            }

            [Test]
            public void One_configured_series_should_return_the_configured_series()
            {
                Given(chart_config_has_been_made).
                    And(there_is_one_series_configured);
                When(getting_series);
                Then("series should contain the one series", () =>
                    {
                        series.Count.ShouldBe(1);
                        var s = series[0];
                        s.Name.ShouldBe("TestName");
                        s.Collection.ShouldBe("TestCol");
                        s.Database.ShouldBe("TestDb");
                        s.Legend.ShouldBe("TestLegend");
                        s.Action.ShouldBe("Show");
                        s.ChartType.ShouldBe("Columns");
                    });
            }

            [Test]
            public void Two_configured_series_should_be_returned_correctly()
            {
                Given(chart_config_has_been_made).
                    And(two_series_has_been_set);
                When(getting_series);
                Then("the two series should be returned", () =>
                    {
                        Compare(series[0], series1);
                        Compare(series[1], series2);
                    });
            }


            private Context there_is_one_series_configured = () =>
            {
                configuration.NewSetting(ChartConfig.series_setting_name, new [] {"TestName"});
                configuration.NewSetting(ChartConfig.series_setting_collection, new[] { "TestCol" });
                configuration.NewSetting(ChartConfig.series_setting_database, new[] { "TestDb" });
                configuration.NewSetting(ChartConfig.series_setting_legend, new[] { "TestLegend" });
                configuration.NewSetting(ChartConfig.series_setting_action, new[] { "Show" });
                configuration.NewSetting(ChartConfig.series_setting_type, new[] { "Columns" });
            };

            private Context two_series_has_been_set = () =>
                {
                    chartConfig.SetSeries(new ObservableCollection<SeriesConfigViewModel> {series1, series2});
                };

            private When getting_series = () => series = chartConfig.GetSeries();

            private static void Compare(SeriesConfigViewModel given, SeriesConfigViewModel expected)
            {
                given.Name.ShouldBe(expected.Name);
                given.Database.ShouldBe(expected.Database);
                given.Collection.ShouldBe(expected.Collection);
                given.Legend.ShouldBe(expected.Legend);
                given.ChartType.ShouldBe(expected.ChartType);
                given.Action.ShouldBe(expected.Action);
            }
        }

        public class Shared : ScenarioClass
        {
            protected static SeriesConfigViewModel series1 = new SeriesConfigViewModel { Database = "DB", Collection = "Col", Name = "Row1", Legend = "Legend1", Action = "Show", ChartType = "Line" };
            protected static SeriesConfigViewModel series2 = new SeriesConfigViewModel { Database = "D2B", Collection = "Col2", Name = "Row2", Legend = "Legend2", Action = "Hide", ChartType = "Area" };

            protected Context chart_config_has_been_made = CreateChartConfig;

            protected When setting_series = SetSeries;

            protected static void SetSeries()
            {
                chartConfig.SetSeries(seriesConfig);
            }

            protected static Configuration configuration;
            protected static ChartConfig chartConfig;
            protected static ObservableCollection<SeriesConfigViewModel> seriesConfig;

            protected static void CreateChartConfig()
            {
                chartConfig = new ChartConfig(configuration);
            }

            [SetUp]
            public void Setup()
            {
                Scenario("");
                configuration = new Configuration();
                seriesConfig = new ObservableCollection<SeriesConfigViewModel>();
                chartConfig = null;
                Before();
            }

            protected virtual void Before()
            {
                
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
