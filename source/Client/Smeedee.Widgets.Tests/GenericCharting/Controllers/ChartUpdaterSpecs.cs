﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Repositories.NoSql;
using Smeedee.Client.Framework.Resources;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Widgets.GenericCharting.Controllers;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.Tests.GenericCharting.Controllers
{
    public class ChartUpdaterSpecs
    {

        [TestFixture]
        public class When_updating_view : Shared
        {
            [Test]
            public void Then_invalid_configuration_should_result_in_an_error_message()
            {
                Given("default configuration");
                When(calling_update);
                Then("there should be an error message in viewmodel", () =>
                                                                          {
                                                                              viewModel.ShowErrorMessageInsteadOfChart.ShouldBe(true);
                                                                              viewModel.ErrorMessage.ShouldNotBeNull();
                                                                              viewModel.ErrorMessage.Count().ShouldNotBe(0);
                                                                          });
            }

            [Test]
            public void Then_a_valid_configuration_should_hide_error_message()
            {
                Given(a_valid_configuration);
                When(calling_update);
                Then("error message should be hidden", () =>
                    {
                        viewModel.ShowErrorMessageInsteadOfChart = false;
                        viewModel.ErrorMessage.ShouldBeNull();
                    });
            }

            [Test]
            public void Then_updater_should_not_look_for_data_in_storage_if_it_is_invalid_configuration()
            {
                Given("default configuration");
                When(calling_update);
                Then("storageReader should not be called", () => storageReaderFake.Verify(s => s.LoadChart(It.IsAny<string>(), It.IsAny<string>()), Times.Never()));
            }

            [Test]
            public void Then_updater_should_look_for_data_when_there_is_a_valid_configuration()
            {
                Given(a_valid_configuration);
                When(calling_update);
                Then("storageReader should be called with correct database and collection", () =>
                    VerifyThatStorageReaderLoadChartWasCalledWithParameters("Charting", "Collection"));
            }

            [Test]
            public void Then_updater_should_look_for_more_data_when_there_is_a_more_complex()
            {
                Given(a_more_complex_valid_configuration);
                When(calling_update);
                Then("storageReader should be called once for each database/collection", () =>
                    {
                        VerifyThatStorageReaderLoadChartWasCalledWithParameters("Charting", "SomeCollection");
                        VerifyThatStorageReaderLoadChartWasCalledWithParameters("Charting", "OtherCollection");
                        VerifyThatStorageReaderLoadChartWasCalledWithParameters("Other", "Col");
                    });
            }

            [Test]
            public void Then_updater_should_load_data_into_viewmodel_old()
            {
                Given(a_valid_configuration).
                    And(storage_returns_a_chart);
                When(calling_update);
                Then("viewModel should contain a line", () =>
                    {
                        viewModel.Name.ShouldBeNull();
                        viewModel.XAxisName.ShouldBeNull();
                        viewModel.YAxisName.ShouldBeNull();
                        viewModel.XAxisType.ShouldBe(ChartConfig.CATEGORY);
                        viewModel.Lines.ShouldNotBeNull();
                        viewModel.Lines.Count.ShouldBe(1);
                        viewModel.Lines[0].Name.ShouldBe("Row");
                        viewModel.Lines[0].Data.Count.ShouldBe(5);
                        viewModel.Lines[0].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[0]));
                        for (int i=0; i<5; i++)
                        {
                            viewModel.Lines[0].Data[i].X.ShouldBe(i);
                            viewModel.Lines[0].Data[i].Y.ShouldBe(i + 1);
                        }

                    });
            }

            [Test]
            public void Then_updater_should_load_data_into_viewmodel()
            {
                Given(a_valid_configuration).
                    And(storage_returns_a_chart);
                When(calling_update);
                Then("viewModel should contain a line", () =>
                {
                    viewModel.Name.ShouldBeNull();
                    viewModel.XAxisName.ShouldBeNull();
                    viewModel.YAxisName.ShouldBeNull();
                    viewModel.XAxisType.ShouldBe(ChartConfig.CATEGORY);

                    viewModel.Series[0].Name.ShouldBe("Row");
                    viewModel.Series[0].Data.Count.ShouldBe(5);
                    viewModel.Series[0].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[0]));
                    viewModel.Series[0].Type.ShouldBe(ChartConfig.LINE);
                    for (int i = 0; i < 5; i++)
                    {
                        viewModel.Series[0].Data[i].X.ShouldBe(i);
                        viewModel.Series[0].Data[i].Y.ShouldBe(i + 1);
                    }
                });
            }

            [Test]
            public void Then_updater_should_load_complex_data_into_viewmodel_old()
            {
                Given(a_more_complex_valid_configuration).
                    And(storage_returns_many_charts);
                When(calling_update);
                Then(viewModel_should_contain_complex_data_old);
            }

            [Test]
            public void Then_updater_should_load_complex_data_into_viewmodel()
            {
                Given(a_more_complex_valid_configuration).
                    And(storage_returns_many_charts);
                When(calling_update);
                Then(viewModel_should_contain_complex_data);
            }

            public static int updateFinishedCalled;

            [Test]
            public void Then_update_view_should_trigger_event_when_finished()
            {
                Given(a_valid_configuration).
                    And(storage_returns_a_chart).
                    And(listening_for_chart_updated_event);





                When(calling_update);
                Then("UpdateFinished should have been called once", () => updateFinishedCalled.ShouldBe(1));

            }

            [Test]
            public void Then_update_view_should_trigger_event_even_if_config_is_invalid()

            {
                Given("default configuration").
                    And(listening_for_chart_updated_event);
                When(calling_update);
                Then("UpdateFinished should have been called once", () => updateFinishedCalled.ShouldBe(1));
            }

            private Context listening_for_chart_updated_event = () =>
                        {
                            updateFinishedCalled = 0;
                            updater.UpdateFinished += UpdateFinished;
                        };

            private static void UpdateFinished(object sender, EventArgs e)
            {
                updateFinishedCalled++;
            }

            private static void VerifyThatStorageReaderLoadChartWasCalledWithParameters(string database, string collection)
            {
                storageReaderFake.Verify(s => s.LoadChart(It.Is<string>(d => d == database), It.Is<string>(c => c == collection), It.IsAny<Action<Chart>>()), Times.Exactly(1));
            }

            private When calling_update = () => updater.Update();

            private Context a_valid_configuration = () =>
                    {
                        var series = new Collection<SeriesConfigViewModel>
                                         {
                                             new SeriesConfigViewModel { Action = ChartConfig.SHOW, Database="Charting", Collection="Collection", Name="Row", ChartType = ChartConfig.LINE, Brush=BrushProvider.GetBrushKeys()[0] }
                                         };
                        chartConfig.SetSeries(series);
                        chartConfig.IsConfigured = true;
                    };

            private Context a_more_complex_valid_configuration = () =>
            {
                var series = new Collection<SeriesConfigViewModel>
                                         {
                                             new SeriesConfigViewModel { Action = ChartConfig.SHOW, Database="Charting", Collection="SomeCollection", Name="Row1", ChartType = ChartConfig.LINE, Brush=BrushProvider.GetBrushKeys()[1]},
                                             new SeriesConfigViewModel { Action = ChartConfig.REFERENCE, Database="Charting", Collection="OtherCollection", Name="Row2" },
                                             new SeriesConfigViewModel { Action = ChartConfig.SHOW, Database="Other", Collection="Col", Name="Row3", Legend="Row3Legend", ChartType = ChartConfig.COLUMNS, Brush=BrushProvider.GetBrushKeys()[3]},
                                             new SeriesConfigViewModel { Action = ChartConfig.SHOW, Database="Charting", Collection="SomeCollection", Name="Row4", ChartType = ChartConfig.AREA, Brush=BrushProvider.GetBrushKeys()[4]}
                                         };
                chartConfig.ChartName = "AChartName";
                chartConfig.XAxisName = "XAxis";
                chartConfig.YAxisName = "YAxis";
                chartConfig.XAxisType = ChartConfig.CATEGORY;
                chartConfig.ChartName = "AChartName";
                chartConfig.XAxisName = "XAxis";
                chartConfig.YAxisName = "YAxis";
                chartConfig.XAxisType = ChartConfig.LINEAR;
                chartConfig.SetSeries(series);
                chartConfig.IsConfigured = true;
            };

            private Context storage_returns_a_chart = () =>
                StorageReaderFakeReturns("Charting", "Collection", AChart());

            private static Chart AChart()
            {
                var chart = new Chart("Charting", "Collection");
                chart.DataSets.Add(GenerateDataSet("Row", 1, 5));
                return chart;
            }

            

            private static Chart Chart1()
            {
                var chart = new Chart("Charting", "SomeCollection");
                chart.DataSets.Add(GenerateDataSet("Row1", 0, 5));
                chart.DataSets.Add(GenerateDataSet("Row4", 10, 5));
                return chart;
            }

            private static Chart Chart2()
            {
                var chart = new Chart("Charting", "OtherCollection");
                chart.DataSets.Add(GenerateDataSet("Row2", 5, 5));
                return chart;
            }

            private static Chart Chart3()
            {
                var chart = new Chart("Other", "Col");
                chart.DataSets.Add(GenerateDataSet("Row3", 15, 5));
                return chart;
            }

            private Context storage_returns_many_charts = () =>
                {
                    StorageReaderFakeReturns("Charting", "SomeCollection", Chart1());
                    StorageReaderFakeReturns("Charting", "OtherCollection", Chart2());
                    StorageReaderFakeReturns("Other", "Col", Chart3());
                };

            private Then viewModel_should_contain_complex_data = () =>
            {
                viewModel.Name.ShouldBe("AChartName");
                viewModel.XAxisName.ShouldBe("XAxis");
                viewModel.YAxisName.ShouldBe("YAxis");
                viewModel.XAxisType.ShouldBe(ChartConfig.LINEAR);
                
                viewModel.Lines.ShouldNotBeNull();
                viewModel.Lines.Count.ShouldBe(1);
                viewModel.Columns.Count.ShouldBe(1);
                viewModel.Areas.Count.ShouldBe(1);

                viewModel.Series[2].Name.ShouldBe("Row1");
                viewModel.Series[2].Data.Count.ShouldBe(5);
                viewModel.Series[2].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[1]));
                viewModel.Series[2].Type.ShouldBe(ChartConfig.LINE);

                viewModel.Series[1].Name.ShouldBe("Row3Legend");
                viewModel.Series[1].Data.Count.ShouldBe(5);
                viewModel.Series[1].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[3]));
                viewModel.Series[1].Type.ShouldBe(ChartConfig.COLUMNS);

                viewModel.Series[0].Name.ShouldBe("Row4");
                viewModel.Series[0].Data.Count.ShouldBe(5);
                viewModel.Series[0].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[4]));
                viewModel.Series[0].Type.ShouldBe(ChartConfig.AREA);

                for (int i = 0; i < 5; i++)
                {
                    viewModel.Series[2].Data[i].X.ShouldBe(i + 5);
                    viewModel.Series[2].Data[i].Y.ShouldBe(i);

                    viewModel.Series[1].Data[i].X.ShouldBe(i + 5);
                    viewModel.Series[1].Data[i].Y.ShouldBe(i + 15);

                    viewModel.Series[0].Data[i].X.ShouldBe(i + 5);
                    viewModel.Series[0].Data[i].Y.ShouldBe(i + 10);
                }
            };

            private Then viewModel_should_contain_complex_data_old = () =>
            {
                viewModel.Name.ShouldBe("AChartName");
                viewModel.XAxisName.ShouldBe("XAxis");
                viewModel.YAxisName.ShouldBe("YAxis");
                viewModel.XAxisType.ShouldBe(ChartConfig.LINEAR);

                viewModel.Lines.ShouldNotBeNull();
                viewModel.Lines.Count.ShouldBe(1);
                viewModel.Columns.Count.ShouldBe(1);
                viewModel.Areas.Count.ShouldBe(1);

                viewModel.Lines[0].Name.ShouldBe("Row1");
                viewModel.Lines[0].Data.Count.ShouldBe(5);
                viewModel.Lines[0].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[1]));

                viewModel.Columns[0].Name.ShouldBe("Row3Legend");
                viewModel.Columns[0].Data.Count.ShouldBe(5);
                viewModel.Columns[0].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[3]));

                viewModel.Areas[0].Name.ShouldBe("Row4");
                viewModel.Areas[0].Data.Count.ShouldBe(5);
                viewModel.Areas[0].Brush.ShouldBe(BrushProvider.GetBrushName(BrushProvider.GetBrushKeys()[4]));

                for (int i = 0; i < 5; i++)
                {
                    viewModel.Lines[0].Data[i].X.ShouldBe(i + 5);
                    viewModel.Lines[0].Data[i].Y.ShouldBe(i);

                    viewModel.Columns[0].Data[i].X.ShouldBe(i + 5);
                    viewModel.Columns[0].Data[i].Y.ShouldBe(i + 15);

                    viewModel.Areas[0].Data[i].X.ShouldBe(i + 5);
                    viewModel.Areas[0].Data[i].Y.ShouldBe(i + 10);
                }
            };

            private static DataSet GenerateDataSet(string name, int offset, int number)
            {
                var set = new DataSet {Name = name};
                for (int i=0;i<number; i++)
                    set.DataPoints.Add(offset + i);
                return set;
            }

        }

        public class Shared : ScenarioClass
        {
            protected static ChartUpdater updater;
            protected static ChartConfig chartConfig;
            protected static ChartViewModel viewModel;
            protected static IUIInvoker uiInvoker;
            protected static Mock<IChartStorageReader> storageReaderFake;

            [SetUp]
            public void SetUp()
            {
                Scenario("");
                chartConfig = new ChartConfig(ChartConfig.NewDefaultConfiguration());
                viewModel = new ChartViewModel();
                storageReaderFake = new Mock<IChartStorageReader>();
                uiInvoker = new NoUIInvokation();

                updater = new ChartUpdater(viewModel, storageReaderFake.Object, uiInvoker) {ChartConfig = chartConfig};
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }

            protected static void StorageReaderFakeReturns(string database, string collection, Chart data)
            {
                storageReaderFake.Setup(s => s.LoadChart(
                    It.Is<string>(d => d == database),
                    It.Is<string>(c => c == collection),
                    It.IsAny<Action<Chart>>())).
                    Callback((string db, string col, Action<Chart> callback) =>
                        callback(data));
            }
        }
    }
}
