using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.DomainModel.Config;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;
using APD.Client.Widget.Admin.Factories;
using APD.Client.Widget.AdminTests.Controllers.ConfigurationControllerSpecs;
using TinyBDD.Specification.NUnit;
using Moq;
using APD.Client.Framework.Plugins;

namespace APD.Client.Widget.AdminTests.Factories.ConfigItemViewModelFactorySpecs
{
    public class BadEventLogPlugin : IConfigurationPlugin
    {
        public string  Name
        {
	        get { throw new NotImplementedException(); }
        }

        public System.Windows.Controls.UserControl  UserInterface
        {
	        get { throw new NotImplementedException(); }
        }

        public void Load(Configuration configuration)
        {
 	        throw new NotImplementedException();
        }

        public Configuration  Save()
        {
 	        throw new NotImplementedException();
        }
    }

    public class Shared : ScenarioClass
    {
        protected static ConfigItemViewModelFactory factory;
        protected static APD.DomainModel.Config.Configuration configuration;

        protected GivenSemantics Factory_is_created()
        {
            return Given("Factory is created", () =>
                factory = new ConfigItemViewModelFactory());
        }

        protected Context Plugin_Configuration_is_created = () =>
        {
            configuration = CreateEventLogConfig();
            //this setting will make the factory create a PluginConfigItemViewModel
            configuration.NewSetting("plugin-class", typeof (ConfigPlugin).AssemblyQualifiedName);
            configuration.NewSetting("is-expanded", "true");
        };

        protected Context bad_Plugin_Configuration_is_created = () =>
        {
            configuration = CreateEventLogConfig();
            configuration.NewSetting("plugin-class", typeof(BadEventLogPlugin).AssemblyQualifiedName);
        };

        protected Context generic_Configuration_is_created = () =>
        {
            configuration = CreateEventLogConfig();
        };

        private static Configuration CreateEventLogConfig()
        {
            var conf = new Configuration("event-log");
            conf.NewSetting("computers", "goeran", "server");
            conf.NewSetting("logs", "security");
            return conf;
        }

        protected Then assure_IsExpaned_is_set_on_ViewModel = () =>
        {
            var configVM = factory.Assemble(configuration);
            configVM.IsExpanded.ShouldBeTrue();
        };

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class When_assembling : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("assembling");
            Given(Factory_is_created());
        }

        [Test]
        public void Assure_Configuration_argument_is_validated()
        {
            Given("configuration is NullObj");
            When("assembling");
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    factory.Assemble(null), ex => {}));
        }
    }

    [TestFixture]
    public class When_assembled : Shared
    {
        [Test]
        public void Assure_IsExpanded_is_set_if_setting_exist_in_Configuration()
        {
            Scenario("assembling");
            Given(Factory_is_created()).
                And("configuration is created", () =>
                {
                    configuration = new Configuration("vcs");
                    configuration.NewSetting("is-expanded", "true");
                });
            When("assembling");
            Then(assure_IsExpaned_is_set_on_ViewModel);
        }

        [Test]
        public void Assure_IsExpanded_is_also_set_if_setting_does_not_exist_in_Configuration()
        {
            Scenario("assembling");
            Given(Factory_is_created()).
                And("configuration is created", () =>
                    configuration = new Configuration("vcs"));
            When("assembling");
            Then("assure IsExpanded is set to false on ViewMOdel", () =>
            {
                var configVM = factory.Assemble(configuration);
                configVM.IsExpanded.ShouldBeFalse();
            });
        }
    }

    [TestFixture]
    public class When_assembling_a_Generic_Configuration : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("assembling a Generic Configuration");
            Given(Factory_is_created()).
                And(generic_Configuration_is_created);

            When("assembling");
        }

        [Test]
        public void Assure_GenericConfigItemViewModel_is_created()
        {
            Then("assure GenericConfigItemViewModel is created", () =>
            {
                var configVM = factory.Assemble(configuration) as GenericConfigItemViewModel;
                configVM.ShouldNotBeNull();
                configVM.Name.ShouldBe(configuration.Name);
                foreach (var setting in configuration.Settings)
                {
                    var settingVM = configVM.Data.Where(s => s.Name == setting.Name).SingleOrDefault() as KeyValueSettingViewModel;
                    settingVM.ShouldNotBeNull();
                    settingVM.Value.ShouldBe(setting.Value);
                }
            });
        }
    }

    [TestFixture]
    public class When_assembling_a_Plugin_configuration : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("assembling a Plugin configuration");
            Given(Factory_is_created()).
                And(Plugin_Configuration_is_created);

            When("assembling");
        }

        [Test]
        public void Assure_PluginConfigItemViewModel_is_created()
        {
            Then("assure PluginConfigItemViewModel is created", () =>
            {
                var configVM = factory.Assemble(configuration) as PluginConfigItemViewModel;
                configVM.ShouldNotBeNull();
                configVM.Name.ShouldBe(configuration.Name);
                configVM.Plugin.ShouldNotBeNull();
            });
        }

        [Test]
        public void Assure_Configuration_is_loaded_into_Plugin()
        {
            Then("assure Configuration is loaded into Plugin", () =>
            {
                var configVM = factory.Assemble(configuration) as PluginConfigItemViewModel;
                var config = configVM.Plugin.Save();
                config.ShouldNotBeNull();
                config.Name.ShouldBe(configuration.Name);
            });
        }

        [Test]
        public void Assure_Plugin_Name_is_set_as_NamePrettyPrint()
        {
            Then("assure Plugin Name is set as NamePrettyPrint", () =>
            {
                var configVM = factory.Assemble(configuration) as PluginConfigItemViewModel;
                configVM.NamePrettyPrint.ShouldBe(Activator.CreateInstance<ConfigPlugin>().Name);
            });
        }

        [Test]
        public void Assure_bad_Plugin_Configurations_are_handeled()
        {
            Scenario("assembling a bad Plugin Configuration");
            Given(Factory_is_created()).
                And(bad_Plugin_Configuration_is_created);

            When("assembling");
            Then("assure PluginConfiguration is created", () =>
                factory.Assemble(configuration).ShouldBeInstanceOfType<PluginConfigItemViewModel>()).
            And("errors are handeled", () =>
            {
                var pluginVM = factory.Assemble(configuration) as PluginConfigItemViewModel;
                pluginVM.ErrorMessage.ShouldNotBeNull();
            });
        }
    }
}
