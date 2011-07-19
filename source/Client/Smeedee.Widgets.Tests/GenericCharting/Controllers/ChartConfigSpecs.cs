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
