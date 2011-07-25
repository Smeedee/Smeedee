using NUnit.Framework;
using Smeedee.Widgets.GenericCharting.Controllers;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Widgets.Tests.GenericCharting.ViewModels
{
    [TestFixture]
    public class When_chartviewmodel_is_created : Shared
    {
        private static ChartViewModel viewModel;
        private Context viewModel_has_been_created = () => viewModel = new ChartViewModel();

        [Test]
        public void Assure_it_contains_lines()
        {
            Given(viewModel_has_been_created);
            When("it is created");
            Then("It should contain lines", () => viewModel.Lines.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_contains_columns()
        {
            Given(viewModel_has_been_created);
            When("it is created");
            Then("It should contain columns", () => viewModel.Columns.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_contains_areas()
        {
            Given(viewModel_has_been_created);
            When("it is created");
            Then("It should contain areas", () => viewModel.Areas.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_contains_refresh()
        {
            Given(viewModel_has_been_created);
            When("it is created");
            Then("It should contain refresh", () => viewModel.Refresh.ShouldNotBeNull());
        }

        [Test]
        public void Assure_showErrorMessageInsteadOfChart_is_false_when_created()
        {
            Given(viewModel_has_been_created);
            When("it is created");
            Then("showErrorMessageInsteadOfChart should be false", () => viewModel.ShowErrorMessageInsteadOfChart.ShouldBeFalse());
        }
    }

    [TestFixture]
    public class When_ChartSettingsViewModel_is_created : Shared
    {
        private static ChartSettingsViewModel chartSettingsViewModel;

        private Context chartSettingsViewModel_has_been_created =
            () => chartSettingsViewModel = new ChartSettingsViewModel();

        [Test]
        public void Assure_it_has_Databases()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains databases", () => chartSettingsViewModel.Databases.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_Collections()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains collections", () => chartSettingsViewModel.Collections.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_SeriesConfig()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains seriesconfig", () => chartSettingsViewModel.SeriesConfig.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_correct_XAxisTypes()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains correct XAxisTypes", () =>
                                                              {
                                                                  chartSettingsViewModel.XAxisTypes.ShouldNotBeNull();
                                                                  //chartSettingsViewModel.XAxisTypes.ShouldContain(ChartConfig.DATETIME);
                                                                  chartSettingsViewModel.XAxisTypes.ShouldContain(ChartConfig.CATEGORY);
                                                                  chartSettingsViewModel.XAxisTypes.ShouldContain(ChartConfig.LINEAR);
                                                              });
        }
    }

    [TestFixture]
    public class When_SettingsChoices_is_created : Shared
    {
        private static SettingsChoices choices;
        private Context settingsChoices_has_been_created = () => choices = new SettingsChoices();

        [Test]
        public void Assure_it_contains_correct_Actions()
        {
            Given(settingsChoices_has_been_created);
            When("it is created");
            Then("it should contain correct actions", () =>
                                                  {
                                                      choices.Actions.ShouldNotBeNull();
                                                      choices.Actions.Contains(ChartConfig.SHOW);
                                                      choices.Actions.Contains(ChartConfig.HIDE);
                                                      choices.Actions.Contains(ChartConfig.REFERENCE);
                                                      choices.Actions.Contains(ChartConfig.REMOVE);
                                                  });
        }

        [Test]
        public void Assure_it_contains_correct_ChartTypes()
        {
            Given(settingsChoices_has_been_created);
            When("it is created");
            Then("it should contain correct chartTypes", () =>
                                                             {
                                                                 choices.ChartTypes.ShouldNotBeNull();
                                                                 choices.ChartTypes.Contains(ChartConfig.AREA);
                                                                 choices.ChartTypes.Contains(ChartConfig.LINE);
                                                                 choices.ChartTypes.Contains(ChartConfig.COLUMNS);
                                                             });
        }
    }

    public class Shared : ScenarioClass
    {
        [SetUp]
        public void Setup()
        {
            Scenario("");
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}
