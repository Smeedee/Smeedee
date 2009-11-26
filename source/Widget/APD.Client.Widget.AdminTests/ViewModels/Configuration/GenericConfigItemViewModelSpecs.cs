using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using APD.Client.Widget.Admin.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Framework;
using NUnit.Framework;
using APD.Client.Framework.ViewModels;

using TinyBDD.Specification.NUnit;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Factories;


namespace APD.Client.Widget.AdminTests.ViewModels.Configuration.ConfigurationItemViewModelSpecs
{
    public class Shared
    {
        protected static GenericConfigItemViewModel viewModel;

        protected static Context viewModel_is_created = () =>
        {
            viewModel = new GenericConfigItemViewModel(UIInvokerFactory.Assemlble());
        };
    }
    
    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_it_isa_AbstractViewModel()
        {
            viewModel_is_created();

            ( viewModel is AbstractViewModel ).ShouldBeTrue();
        }

        [Test]
        public void Assure_it_has_a_Name()
        {
            viewModel_is_created();

            viewModel.Name.ShouldBeNull();
        }

        [Test]
        public void Assure_it_has_Settings()
        {
            viewModel_is_created();

            viewModel.Data.ShouldNotBeNull();
            viewModel.Data.ShouldBeInstanceOfType<ObservableCollection<SettingViewModel>>();
        }
    }
}
