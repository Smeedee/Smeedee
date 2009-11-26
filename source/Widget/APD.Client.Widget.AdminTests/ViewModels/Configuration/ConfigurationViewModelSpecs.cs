using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Widget.Admin.ViewModels;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using APD.Client.Framework;
using APD.Client.Framework.ViewModels;

using TinyBDD.Specification.NUnit;
using System.Collections.ObjectModel;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Widget.Admin.Controllers;


namespace APD.Client.Widget.AdminTests.ViewModels.Configuration.ConfigurationViewModelSpecs
{
    public class Shared
    {
        protected static ConfigurationViewModel viewModel;
        protected static CommandNotifierWiring<EventArgs> saveCommandNotifier = new CommandNotifierWiring<EventArgs>();
        protected static CommandNotifierWiring<EventArgs> refreshCommandNotifier = new CommandNotifierWiring<EventArgs>();

        protected Context viewModel_is_created = () =>
        {
            viewModel = new ConfigurationViewModel(new NoUIInvocation(), saveCommandNotifier, refreshCommandNotifier);
        };
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_its_a_AbstractViewModel()
        {
            viewModel_is_created();

            ( viewModel is AbstractViewModel ).ShouldBeTrue();
        }

        [Test]
        public void Assure_it_has_items()
        {
            viewModel_is_created();

            viewModel.Data.ShouldNotBeNull();
            viewModel.Data.ShouldBeInstanceOfType<ObservableCollection<ConfigurationItemViewModel>>();
        }
    }
}
