using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Widget.Admin.Controllers;

using NUnit.Framework;

using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Client.Widget.Admin.ViewModels;
using Moq;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Controllers;
using APD.Tests;
using APD.Client.Framework.Plugins;
using APD.Client.Widget.Admin.Plugins;

namespace APD.Client.Widget.AdminTests.Controllers.ConfigurationControllerSpecs
{
    public class ConfigPlugin : IConfigurationPlugin
    {
        private Configuration configuration;

        #region IConfigurationPlugin Members

        public string Name
        {
            get { return GetType().FullName; }
        }

        public System.Windows.Controls.UserControl UserInterface
        {
            get { throw new NotImplementedException(); }
        }

        public void Load(Configuration configuration)
        {
            this.configuration = configuration;
        }

        public Configuration Save()
        {
            return configuration;
        }

        #endregion
    }

    public class Shared : ScenarioClass
    {
        protected static ConfigurationController controller;
        protected static ConfigurationViewModel viewModel;
        protected static Mock<IRepository<Configuration>> repositoryMock;
        protected static Mock<IInvokeBackgroundWorker<IEnumerable<Configuration>>> backgroundWorkerMock;
        protected static Mock<IPersistDomainModels<Configuration>> persisterRepositoryMock;
        protected static Mock<INotify<EventArgs>> saveNotifierMock;
        protected static Mock<INotify<EventArgs>> refreshNotifierMock;
        protected static IEnumerable<Configuration> persistedConfigurations;
        protected static PropertyChangedRecorder propertyChangeRecorder;
        protected static Mock<ITriggerCommand> saveCommandNotifierMock;
        protected static Mock<ITriggerCommand> refreshCommandNotifierMock;

        protected Context controllers_is_created = () =>
        {
            CreateController();
        };

        private static void CreateController()
        {
            saveNotifierMock = new Mock<INotify<EventArgs>>();
            refreshNotifierMock = new Mock<INotify<EventArgs>>();
            controller = new ConfigurationController(viewModel, 
                repositoryMock.Object, 
                backgroundWorkerMock.Object, 
                persisterRepositoryMock.Object,
                saveNotifierMock.Object, 
                refreshNotifierMock.Object);
        }

        protected Context BackgroundWorker_is_created = () =>
        {
            backgroundWorkerMock = new Mock<IInvokeBackgroundWorker<IEnumerable<Configuration>>>();
            backgroundWorkerMock.Setup(w => w.RunAsyncVoid(It.IsAny<Action>())).Callback<Action>(a =>
                a.Invoke());
        };

        protected Context Repository_is_created = () =>
        {
            persistedConfigurations = null;
            repositoryMock = new Mock<IRepository<Configuration>>();
            persisterRepositoryMock = new Mock<IPersistDomainModels<Configuration>>();
            persisterRepositoryMock.Setup(r => r.Save(It.IsAny<IEnumerable<Configuration>>())).
                Callback<IEnumerable<Configuration>>(data =>
                    persistedConfigurations = data);
        };

        protected Context viewModel_is_created = () =>
        {
            saveCommandNotifierMock = new Mock<ITriggerCommand>();
            refreshCommandNotifierMock = new Mock<ITriggerCommand>();
            viewModel = new ConfigurationViewModel(new NoUIInvocation(),
                saveCommandNotifierMock.Object,
                refreshCommandNotifierMock.Object);
            propertyChangeRecorder = new PropertyChangedRecorder(viewModel);
        };

        protected Context no_configuration_exists = () =>
        {
            var configurations = new List<Configuration>();

            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configurations);
        };

        protected Context configurations_exists = () =>
        {
            var configurations = new List<Configuration>();

            var workingDaysConfig = new Configuration("working-days-left");
            workingDaysConfig.NewSetting("end-date", "01.09.2009");
            configurations.Add(workingDaysConfig);

            var configuration = new Configuration("vcs");
            configuration.NewSetting("supported-providers", "svn", "tfs");
            configuration.NewSetting("provider", "svn");
            configuration.NewSetting("username", "goeran");
            configuration.NewSetting("password", "hansen");
            configuration.NewSetting("url", "https://smeedee.org:8443/svn");
            configuration.NewSetting("plugin-class", typeof (VCSConfigPlugin).AssemblyQualifiedName);
            configurations.Add(configuration);

            var ciConfig = new Configuration("ci");
            ciConfig.NewSetting("supported-providers", "cc.net", "tfs");
            ciConfig.NewSetting("provider", string.Empty);
            ciConfig.NewSetting("url", string.Empty);
            ciConfig.NewSetting("username", string.Empty);
            ciConfig.NewSetting("password", string.Empty);
            ciConfig.NewSetting("plugin-class", typeof(CIConfigPlugin).AssemblyQualifiedName);
            configurations.Add(ciConfig);

            var dashboardConfig = new Configuration("dashboard");
            dashboardConfig.NewSetting("slide-widgets", "");
            dashboardConfig.NewSetting("selected-slide-widgets");
            dashboardConfig.NewSetting("tray-widgets", "");
            dashboardConfig.NewSetting("selected-tray-widgets");
            dashboardConfig.NewSetting("plugin-class", typeof (ConfigPlugin).AssemblyQualifiedName);
            configurations.Add(dashboardConfig);

            var eventLogConfig = new Configuration("event-log");
            eventLogConfig.NewSetting("machine", "goeran");
            eventLogConfig.NewSetting("logs", "system", "security");
            eventLogConfig.NewSetting("plugin-class", typeof (ConfigPlugin).AssemblyQualifiedName);
            configurations.Add(eventLogConfig);

            repositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).Returns(configurations);
        };

        protected When controller_is_spawned = () =>
        {
            CreateController();
        };

        protected GivenSemantics dependencies_are_created()
        {
            return Given(Repository_is_created).
                And(BackgroundWorker_is_created).
                And(viewModel_is_created);
        }

        protected When save = () =>
        {
            saveNotifierMock.Raise(n => n.NewNotification += null, EventArgs.Empty);
        };

        protected ThenSemantics assure_VersionControlSystem_config_is_loaded_into_ViewModel()
        {
            return Then("assure VersionControlSystem config is loaded into ViewModel", () =>
                AssertPluginConfiguration("vcs"));
        }

        protected ThenSemantics assure_ContinuousIntegration_config_is_loaded_into_ViewModel()
        {
            return Then("assure Continuous Integration config is loaded into ViewModel", () =>
                AssertPluginConfiguration("ci"));
        }

        protected void AssertProviderConfiguration(string configName)
        {
            var persistedConfigs = repositoryMock.Object.Get(new AllSpecification<Configuration>());
            var persistedConfig = persistedConfigs.Where(c => c.Name == configName).SingleOrDefault();
            if (persistedConfig == null)
                if (configName == "vcs")
                    persistedConfig = Configuration.DefaultVCSConfiguration();
                else
                    persistedConfig = Configuration.DefaultCIConfiguration();

            var vcsConfig = viewModel.Data.Where(i => i.Name == configName).SingleOrDefault();
            vcsConfig.ShouldNotBeNull();
            vcsConfig.ShouldBeInstanceOfType<ProviderConfigItemViewModel>();

            var config = vcsConfig as ProviderConfigItemViewModel;
            config.Username.ShouldBe(persistedConfig.GetSetting("username").Value);
            config.Password.ShouldBe(persistedConfig.GetSetting("password").Value);
            config.URL.ShouldBe(persistedConfig.GetSetting("url").Value);
            config.SelectedProvider.ShouldBe(persistedConfig.GetSetting("provider").Value);
            config.SupportedProviders.ShouldNotBeNull();

            foreach (var provider in persistedConfig.GetSetting("supported-providers").Vals)
                config.SupportedProviders.ShouldContain(provider);
        }

        protected void AssertProviderViewModelIsPersisted(string configurationName)
        {
            var configVM = viewModel.Data.Where(c => c.Name == configurationName).SingleOrDefault() as ProviderConfigItemViewModel;
            var config = persistedConfigurations.Where(c => c.Name == configurationName).SingleOrDefault();
            config.ShouldNotBeNull();

            config.Name.ShouldBe(configVM.Name);
            config.GetSetting("username").Value.ShouldBe(configVM.Username);
            config.GetSetting("password").Value.ShouldBe(configVM.Password);
            config.GetSetting("url").Value.ShouldBe(configVM.URL);
            config.GetSetting("provider").Value.ShouldBe(configVM.SelectedProvider);

            foreach (var provider in configVM.SupportedProviders)
                config.GetSetting("supported-providers").Vals.ToList().ShouldContain(provider);
        }

        protected void AssertConfigurationIsPersisted(string configName)
        {
            persistedConfigurations.Where(c => c.Name == configName).Count().ShouldBe(1);
        }

        protected ThenSemantics assure_Dashboard_config_is_loaded_into_ViewModel()
        {
            return Then("assure Dashboard config is loaded into ViewModel", () =>
                AssertPluginConfiguration("dashboard"));
        }

        protected static void AssertPluginConfiguration(string name)
        {
            var configItem = viewModel.Data.Where(c => c.Name == name).SingleOrDefault();
            configItem.ShouldNotBeNull();
            var pluginConfigVM = configItem as PluginConfigItemViewModel;
            pluginConfigVM.ShouldNotBeNull();
        }

        [TearDown]
        public void Teardown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class When_spawning : Shared
    {
        [Test]
        public void Assure_ViewModel_is_argument_is_validated()
        {
            this.ShouldThrowException<ArgumentNullException>(() =>
                new ConfigurationController(null, null, null, null, null, null), exception => {});
        }

        [Test]
        public void Assure_ConfigurationRepository_argument_is_validated()
        {
            viewModel_is_created();

            this.ShouldThrowException<ArgumentNullException>(() =>
                new ConfigurationController(viewModel, null, null, null, null, null), exception => {});
        }

        [Test]
        public void Assure_BackgroundWorker_argument_is_validated()
        {
            Given(viewModel_is_created).
                And(Repository_is_created);

            When("spawning");

            Then("assure backgroundWorker argument is validated", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                   new ConfigurationController(viewModel, repositoryMock.Object, null, null, null, null), e => {}));
        }

        [Test]
        public void Assure_DomainModelPersister_argument_is_validated()
        {
            Given(viewModel_is_created).
                And(Repository_is_created).
                And(BackgroundWorker_is_created);

            When("spawning");

            Then("assure DomainModelPersister argument is validated", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new ConfigurationController(viewModel, repositoryMock.Object, backgroundWorkerMock.Object, null, null, null), e => { }));
        }
    }

    [TestFixture]
    public class When_spawned : Shared
    {
        [Test]
        public void Assure_data_is_loaded_from_Repository()
        {
            Scenario("When spawned");
            Given(dependencies_are_created());

            When(controller_is_spawned);
            Then("assure data is loaded from Repository", () =>
                repositoryMock.Verify(r => r.Get(It.IsAny<AllSpecification<Configuration>>()), Times.Once()));
        }
    }

    [TestFixture]
    public class When_notified_to_refresh : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When notified to refresh");
            Given(dependencies_are_created()).
                And(controllers_is_created);

            When("notified to refresh", () =>
                refreshNotifierMock.Raise(n => n.NewNotification += null, EventArgs.Empty));
        }

        [Test]
        public void Assure_data_is_fetched()
        {
            Then("assure data is fetched", () =>
                repositoryMock.Verify(r => r.Get(It.IsAny<Specification<Configuration>>()), Times.Exactly(2)));
        }
    }

    [TestFixture]
    public class When_loading_data : Shared
    {
        [Test]
        public void Assure_data_is_loaded_in_background()
        {
            Scenario("When loading data");
            Given(dependencies_are_created());

            When(controller_is_spawned);
            Then("assure data is loaded in background", () =>
                backgroundWorkerMock.Verify(w => w.RunAsyncVoid(It.IsAny<Action>()), Times.Once()));
        }

        [Test]
        public void Assure_IsLoading_is_set_on_ViewModel()
        {
            Scenario("When loading data");
            Given(dependencies_are_created());

            When(controller_is_spawned);

            Then("assure IsLoading is set on ViewModel", () =>
                propertyChangeRecorder.ChangedProperties.ShouldContain("IsLoading"));
        }
    }

    [TestFixture]
    public class When_data_is_loaded : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When data is loaded");
            Given(dependencies_are_created()).
                And(configurations_exists);

            When(controller_is_spawned);
        }

        [Test]
        public void Assure_IsLoading_is_set_on_ViewModel()
        {
            Then("assure isLoading is set no ViewModel", () =>
                propertyChangeRecorder.ChangedProperties.Where(p => p.Equals("IsLoading")).Count().ShouldBe(2)).
                And("assure it is set to false", () =>
                    viewModel.IsLoading.ShouldBe(false));
        }

        [Test]
        public void Assure_Generic_configurations_are_loaded_into_viewModel()
        {
            IEnumerable<Configuration> configurations = null;

            Then("assure all configurations are loaded into viewModel", () =>
            {
                configurations = repositoryMock.Object.Get(new AllSpecification<Configuration>());

                viewModel.Data.Count.ShouldBe(configurations.Count());

                foreach (var configItem in configurations)
                {
                    var configItemVM = viewModel.Data.Where(i => i.Name == configItem.Name).SingleOrDefault();
                    configItemVM.ShouldNotBeNull();

                    if (configItemVM is GenericConfigItemViewModel)
                    {
                        var itemVM = configItemVM as GenericConfigItemViewModel;
                        foreach (var settingItem in configItem.Settings)
                        {
                            var settingItemVM = itemVM.Data.Where(i => i.Name == settingItem.Name).SingleOrDefault();
                            settingItemVM.ShouldNotBeNull();
                            settingItemVM.ShouldBeInstanceOfType<KeyValueSettingViewModel>();
                            ((KeyValueSettingViewModel)settingItemVM).Value.ShouldNotBeNull();
                        }
                    }
                }
            });
        }

        [Test]
        public void Assure_Plugin_configuration_is_loaded_into_ViewModel()
        {
            Then("assure Plugin configurations is loaded into ViewModel", () =>
            {
                var pluginConfigVM = viewModel.Data.Where(i => i.Name == "event-log").SingleOrDefault() as PluginConfigItemViewModel;
                pluginConfigVM.ShouldNotBeNull();
                pluginConfigVM.Plugin.ShouldNotBeNull();
            });
        }

        [Test]
        public void Assure_Configurations_are_arranged_correctly_in_ViewModel()
        {
            Then("assure Configurations are arranged correctly in ViewModel", () =>
            {
                viewModel.Data[0].Name.ShouldBe("vcs");
                viewModel.Data[1].Name.ShouldBe("ci");
            });
        }

        [Test]
        public void Assure_all_Configurations_has_been_loaded_into_ViewModel()
        {
            Then("assure all Configurations has been loaded into ViewModel", () =>
                viewModel.Data.Count.ShouldBe(repositoryMock.Object.Get(new AllSpecification<Configuration>()).Count()));
        }

        [Test]
        public void Assure_VCS_configuration_is_loaded_into_ViewModel()
        {
            Then(assure_VersionControlSystem_config_is_loaded_into_ViewModel());
        }

        [Test]
        public void Assure_NamePrettyPrint_is_set_in_ViewModel_for_VCS()
        {
            Then("assure NamePrettyPrint is set in viewModel for VCS", () =>
            {
                var configItem = viewModel.Data.Where(c => c.Name == "vcs").SingleOrDefault();
                configItem.NamePrettyPrint.ShouldBe("Version Control System");
            });
        }

        [Test]
        public void Assure_CI_config_is_loaded_into_ViewModel()
        {
            Then(assure_ContinuousIntegration_config_is_loaded_into_ViewModel());
        }

        [Test]
        public void Assure_NamePrettyPrint_is_set_in_ViewModel_for_CI()
        {
            Then("assure NamePrettyPrint is set in viewModel for CI", () =>
            {
                var configItem = viewModel.Data.Where(c => c.Name == "ci").SingleOrDefault();
                configItem.NamePrettyPrint.ShouldBe("Continuous Integration");
            });
        }

        [Test]
        public void Assure_Dashboard_config_is_loaded_into_ViewModel()
        {
            Then(assure_Dashboard_config_is_loaded_into_ViewModel());
        }
    }

    [TestFixture]
    public class When_data_is_loaded_and_no_configs_exists : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When data is loaded an no configs exists");
            Given(dependencies_are_created()).
                And(no_configuration_exists);

            When(controller_is_spawned);
        }

        [Test]
        public void Assure_VCS_configuration_is_loaded_into_ViewModel()
        {
            Then(assure_VersionControlSystem_config_is_loaded_into_ViewModel());
        }

        [Test]
        public void Assure_CI_configuration_is_loaded_into_ViewModel()
        {
            Then(assure_ContinuousIntegration_config_is_loaded_into_ViewModel());
        }

        [Test]
        public void Assure_Dashboard_configuration_is_loaded_into_ViewModel()
        {
            Then(assure_Dashboard_config_is_loaded_into_ViewModel());
        }
    }

    [TestFixture]
    public class When_saving : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("When saving");
            Given(dependencies_are_created()).
                And(configurations_exists).
                And(controllers_is_created);

            When(save);
        }

        [Test]
        public void Assure_persister_Repository_is_called()
        {
            Then("assure persister Repository is called", () =>
                persisterRepositoryMock.Verify(r => r.Save(It.IsAny<IEnumerable<Configuration>>())));
        }

        [Test]
        public void Assure_all_Configurations_are_persisted()
        {
            Then("assure all Configurations are persisted", () =>
                persistedConfigurations.Count().ShouldBe(viewModel.Data.Count));
        }

        [Test]
        public void Assure_VCS_configuration_is_saved()
        {
            Then("assure VCS configuration is saved", () =>
                AssertConfigurationIsPersisted("vcs"));
        }

        [Test]
        public void Assure_CI_configuration_is_saved()
        {
            Then("assure CI configuration is saved", () =>
                AssertConfigurationIsPersisted("ci"));
        }

        [Test]
        public void Assure_Generic_configuration_is_saved()
        {
            Then("assure Generic configurations is saved", () =>
            {
                var persistedGenericConfigs =
                    persistedConfigurations.Where(c => c.Name != "vcs" && c.Name != "ci" && c.Name != "dashboard");

                var genericVMConfigs =
                    viewModel.Data.Where(c => c.Name != "vcs" && c.Name != "ci" && c.Name != "dashboard");

                persistedGenericConfigs.Count().ShouldBe(genericVMConfigs.Count());

                foreach (var configVM in genericVMConfigs)
                {
                    var config = persistedGenericConfigs.Where(c => c.Name == configVM.Name).SingleOrDefault();
                    config.ShouldNotBeNull();

                    if (configVM is GenericConfigItemViewModel)
                    {                   
                        var genericConfigVM = configVM as GenericConfigItemViewModel;

                        foreach (KeyValueSettingViewModel settingVM in genericConfigVM.Data)
                        {
                            config.GetSetting(settingVM.Name).Name.ShouldBe(settingVM.Name);
                            config.GetSetting(settingVM.Name).Value.ShouldBe(settingVM.Value);
                        }
                    }
                }
            });
        }
    }
}
