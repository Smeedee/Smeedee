using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Widget.Admin.ViewModels.Configuration;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;
using APD.Client.Framework.Factories;

using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.AdminTests.ViewModels.Configuration.DashboardConfigItemViewModelSpecs
{
    public class Shared : ScenarioClass
    {
        protected static DashboardConfigItemViewModel viewModel;

        protected Context ViewModel_is_created = () =>
        {
            viewModel = new DashboardConfigItemViewModel(UIInvokerFactory.Assemlble());
        };

        protected When ViewModel_is_spawned = () =>
        {
            viewModel = new DashboardConfigItemViewModel(UIInvokerFactory.Assemlble());
        };

        [TearDown]
        public void Teardown()
        {
            StartScenario(); 
        }
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            Given("dependencies are created");

            When(ViewModel_is_spawned);
        }

        [Test]
        public void Assure_it_isa_ConfigurationItemViewModel()
        {
            Then("assure it is a ConfigurationItemViewModel", () =>
                (viewModel is ConfigurationItemViewModel).ShouldBeTrue());

        }

        [Test]
        public void Assure_it_has_SlideWidgets()
        {
            Then("assure it has SlideWidgets", () =>
                viewModel.SlideWidgets.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_SelectedSlideWidgets()
        {
            Then("assure it has SelectedSlideWidgets", () =>
                viewModel.SelectedSlideWidgets.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_TrayWidgets()
        {
            Then("assure it has TrayWidgets", () =>
                viewModel.TrayWidgets.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_SelectedTrayWidgets()
        {
            Then("assure it has SelectedTrayWidgets", () =>
                viewModel.SelectedTrayWidgets.ShouldNotBeNull());
        }
    }
}
