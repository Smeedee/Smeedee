using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Factories;

using TinyBDD.Specification.NUnit;
using APD.Tests;
using System.Collections.ObjectModel;
using Config = APD.DomainModel.Config;

namespace APD.Client.Widget.AdminTests.ViewModels.Configuration.ProviderConfigItemViewModelSpecs
{
    public class Shared : ScenarioClass
    {
        protected static ProviderConfigItemViewModel viewModel;
        protected static PropertyChangedRecorder changeRecorder;
        protected static Config.Configuration configuration;

        protected Context VCS_Default_Configuration_is_created = () =>
        {
            configuration = Config.Configuration.DefaultVCSConfiguration();
        };

        protected Context Configuration_contains_data = () =>
        {
            configuration = Config.Configuration.DefaultVCSConfiguration();
            configuration.NewSetting("username", "goeran");
            configuration.NewSetting("password", "hansen");
            configuration.NewSetting("url", "http://smeedee.org/svn");
            configuration.NewSetting("is-expanded", "false");
            configuration.NewSetting("provider", "svn");
        };

        protected GivenSemantics Dependencies_are_created()
        {
            return Given(VCS_Default_Configuration_is_created).
                And(Configuration_contains_data);
        }

        protected Context ViewModel_is_created = () =>
        {
            CreateViewModel();
        };

        protected When ViewModel_is_spawned = () =>
        {
            CreateViewModel();
        };

        private static void CreateViewModel()
        {
            viewModel = new ProviderConfigItemViewModel(UIInvokerFactory.Assemlble(), configuration);
            changeRecorder = new PropertyChangedRecorder(viewModel);
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class When_spawning : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When spawning");
            Given("ViewModel is not created");
        }

        [Test]
        public void Assure_Configuration_argument_is_validated()
        {
            Given("Configuration is NullObj");
            When("spawning");
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new ProviderConfigItemViewModel(UIInvokerFactory.Assemlble(), null), ex => { }));
        }
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When spawned");
            Given("ViewModel is not created");

            When(ViewModel_is_spawned);
        }

        [Test]
        public void Assure_it_is_a_ConfigurationItemViewModel()
        {
            Given(Dependencies_are_created());
            Then("assure it is a ConfigurationItemViewModel", () =>
                (viewModel is ConfigurationItemViewModel).ShouldBeTrue());
        }
    }

    [TestFixture]
    public class When_spawned_with_Configuration : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When spawned with Configuration");
            Given(Dependencies_are_created()).
                And("ViewModel is not created");
            When(ViewModel_is_spawned);
        }

        [Test]
        public void Assure_Username_is_extracted_from_Configuration()
        {
            Then("assure Username is extracted from Configuration", () =>
                viewModel.Username.ShouldBe(configuration.GetSetting("username").Value));
        }

        [Test]
        public void Assure_Password_is_extracted_from_Configuration()
        {
            Then("asure Password is extracted from Configuration", () =>
                viewModel.Password.ShouldBe(configuration.GetSetting("password").Value));
        }

        [Test]
        public void Assure_URL_is_extracted_from_Configuration()
        {
            Then("assure URL is extracted from Configuration", () =>
                viewModel.URL.ShouldBe(configuration.GetSetting("url").Value));
        }

        [Test]
        public void Assure_SelectedProvider_is_extracted_from_Configuration()
        {
            Then("assure SelectedProvider is extracted from Configuration", () =>
                viewModel.SelectedProvider.ShouldBe(configuration.GetSetting("provider").Value));
        }

        [Test]
        public void Assure_SupportedProviders_is_extracted_from_Configuration()
        {
            Then("assure SupportedProviders is extracted from Configuration", () =>
            {
                foreach (var value in configuration.GetSetting("supported-providers").Vals)
                    viewModel.SupportedProviders.ShouldContain(value);
            });
        }

        [Test]
        public void Assure_Project_is_extracted_from_Configuration()
        {
            Then("assure Project is extracted from Configuration", () =>
                viewModel.Project.ShouldBe(configuration.GetSetting("project").Value));
        }
    }

    [TestFixture]
    public class When_Provider_TFS_is_selected_as_Provider : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("when TFS is selected as Provider");
            Given(Dependencies_are_created()).
                And(ViewModel_is_created);

            When("TFS is selected as Provider", () =>
                viewModel.SelectedProvider = "tfs");
        }

        [Test]
        public void Assure_Project_is_available()
        {
            Then("assure Project is available", () =>
                viewModel.IsProjectEditable.ShouldBeTrue());
        }


        [Test]
        public void Assure_Project_is_prefixed_with_dollarSlash()
        {
            Then("The project property should start with $/", () =>
                viewModel.Project.StartsWith("$/").ShouldBeTrue());
        }
    }

    [TestFixture] 
    public class When_edited : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("when edited");
            Given(VCS_Default_Configuration_is_created).
                And(ViewModel_is_created);
        }

        [Test]
        public void Assure_Username_is_observable()
        {
            When("Username is changed", () =>
                viewModel.Username = "goeran");

            Then("assure observers are notified", () =>
                changeRecorder.ChangedProperties.ShouldContain("Username")).
            And("assure Configuration is updated", () =>
                configuration.GetSetting("username").Value.ShouldBe(viewModel.Username));
        }

        [Test]
        public void Assure_Password_is_observable()
        {
            When("Password is changed", () =>
                viewModel.Password = "hansen");

            Then("assure observers are notified", () =>
                changeRecorder.ChangedProperties.ShouldContain("Password")).
            And("assure Configuration is updated", () =>
                configuration.GetSetting("password").Value.ShouldBe(viewModel.Password));
        }

        [Test]
        public void Assure_URL_is_observable()
        {
            When("URL is changed", () =>
                viewModel.URL = "http://smeedee.org/ccnet");

            Then("assure observers are notified", () =>
                changeRecorder.ChangedProperties.ShouldContain("URL")).
            And("assure Configuration is updated", () =>
                configuration.GetSetting("url").Value.ShouldBe(viewModel.URL));
        }

        [Test]
        public void Assure_SelectedProvider_is_observable()
        {
            When("SelectedProvider is changed", () =>
                viewModel.SelectedProvider = "svn");

            Then("assure observers are notified", () =>
                changeRecorder.ChangedProperties.ShouldContain("SelectedProvider")).
            And("assure Configuration is updated", () =>
                configuration.GetSetting("provider").Value.ShouldBe(viewModel.SelectedProvider));
        }

        [Test]
        public void Assure_Status_is_observable()
        {
            When("Status is changed", () =>
                viewModel.Status = "checking...");

            Then("assure observers are notified", () =>
                changeRecorder.ChangedProperties.ShouldContain("Status"));

        }
    }

}
