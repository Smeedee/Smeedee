using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using APD.Client.Widget.Admin.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Framework.Factories;

using TinyBDD.Specification.NUnit;
using APD.Client.Framework.ViewModels;
using APD.Client.Widget.Admin.ViewModels.Configuration;


namespace APD.Client.Widget.AdminTests.ViewModels.Configuration.KeyValueSettingViewModelSpecs
{
    public class Shared : ScenarioClass
    {
        protected static KeyValueSettingViewModel viewModel;

        protected Context ViewModel_is_created = () =>
        {
            viewModel = new KeyValueSettingViewModel(UIInvokerFactory.Assemlble());
        };

        [TearDown]
        public void Theardown()
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
            Given(ViewModel_is_created);

            When("spawned");
        }

        [Test]
        public void Assure_it_has_a_Name()
        {
            Then("assure it has a name", () =>
                viewModel.Name.ShouldBeNull());
        }

        [Test]
        public void Assure_it_has_a_Value()
        {
            Then("assure it has a Value", () =>
                viewModel.Value.ShouldBeNull());
        }

        [Test]
        public void Assure_it_is_a_SettingViewModel()
        {
            Then("assure it is a AbstractViewModel", () =>
                (viewModel is AbstractViewModel).ShouldBeTrue());
        }
    }
}
