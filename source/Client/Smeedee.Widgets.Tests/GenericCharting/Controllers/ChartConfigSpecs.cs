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
            public void It_should_be_possible_to_change_Action()
            {
                
            }
            [Test]
            public void It_should_be_possible_to_set_Name()
            {
                Given(chart_config_has_been_made);
                When("setting Name", () => chartConfig.Name = "Name");
                Then("configuration should have that value",
                     () => configuration.GetSetting(ChartConfig.series_setting_name).Value.ShouldBe("Name"));
            }

            [Test]
            public void It_should_be_possible_to_change_Name()
            {
                Given(chart_config_has_been_made).
                    And("Name is set", () => chartConfig.Name = "something");
                When("changing Name", () => chartConfig.Name = "else");
                Then("configuration should be updated",
                    () =>
                    {
                        configuration.GetSetting(ChartConfig.series_setting_name).HasMultipleValues.ShouldBeFalse();
                        configuration.GetSetting(ChartConfig.series_setting_name).Value.ShouldBe("else");
                    });
            }

            [Test]
            public void It_should_be_possible_to_set_Database()
            {
                Given(chart_config_has_been_made);
                When("setting Database", () => chartConfig.Database = "Database");
                Then("configuration should have that value",
                     () => configuration.GetSetting(ChartConfig.series_setting_database).Value.ShouldBe("Database"));
            }

            [Test]
            public void It_should_be_possible_to_change_Database()
            {
                Given(chart_config_has_been_made).
                    And("Database is set", () => chartConfig.Database = "something");
                When("changing Database", () => chartConfig.Database = "else");
                Then("configuration should be updated",
                    () =>
                    {
                        configuration.GetSetting(ChartConfig.series_setting_database).HasMultipleValues.ShouldBeFalse();
                        configuration.GetSetting(ChartConfig.series_setting_database).Value.ShouldBe("else");
                    });
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


            private static SeriesConfigViewModel series1 = new SeriesConfigViewModel { Database = "DB", Collection = "Col", Name = "Row1", Legend = "Legend1", SelectedAction = "Show", SelectedChartType = "Line" };
            private static SeriesConfigViewModel series2 = new SeriesConfigViewModel { Database = "D2B", Collection = "Col2", Name = "Row2", Legend = "Legend2", SelectedAction = "Hide", SelectedChartType = "Area" };

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
                action.Vals.ToList()[row].ShouldBe(series.SelectedAction);
                type.Vals.ToList()[row].ShouldBe(series.SelectedChartType);
            }

            private Context series_has_been_set = () => SetSeries();
            private When setting_series = () => SetSeries();

            private static void SetSeries()
            {
                chartConfig.SetSeries(seriesConfig);
            }
        }

        public class Shared : ScenarioClass
        {

            protected Context chart_config_has_been_made = () => CreateChartConfig();

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
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
