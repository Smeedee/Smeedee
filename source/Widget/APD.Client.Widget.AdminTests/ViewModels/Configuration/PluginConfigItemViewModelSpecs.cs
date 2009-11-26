using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Widget.Admin.ViewModels.Configuration;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;
using APD.Client.Framework.Factories;
using APD.Client.Framework.ViewModels;

using TinyBDD.Specification.NUnit;


namespace APD.Client.Widget.AdminTests.ViewModels.Configuration.PluginConfigItemViewModelSpecs
{
    public class Shared : ScenarioClass
    {
        protected PluginConfigItemViewModel viewModel;

        protected GivenSemantics ViewModel_is_created()
        {
            return Given("ViewModel is created", () =>
                viewModel = new PluginConfigItemViewModel(UIInvokerFactory.Assemlble()));
        }

        [TearDown]
        public void TearDown()
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
            Given(ViewModel_is_created());

            When("object is availble");
        }

        [Test]
        public void Assure_it_ISA_ConfigurationItemViewModel()
        {
            Then("assure it ISA ConfigurationItemViewModel", () =>
                (viewModel is ConfigurationItemViewModel).ShouldBeTrue());
        }

        [Test]
        public void Assure_it_has_a_Plugin()
        {
            Then("assure it has a Plugin", () =>
                viewModel.Plugin.ShouldBeNull());
        }

        [Test]
        public void Assure_it_has_a_IsLoaded()
        {
            Then("assure it has a IsLoaded", () =>
                viewModel.IsLoaded.ShouldBeFalse());
        }

        [Test]
        public void Assure_it_has_a_ErrorMessage()
        {
            Then("assure it has a ErrorMessage", () =>
                viewModel.ErrorMessage.ShouldBeNull());
        }
    }
}
