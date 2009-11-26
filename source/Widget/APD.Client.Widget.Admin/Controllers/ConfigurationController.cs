using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using APD.Client.Framework;
using APD.Client.Framework.Controllers;
using APD.Client.Widget.Admin.Utils;
using APD.Client.Widget.Admin.ViewModels;
using APD.DomainModel.Config;
using APD.DomainModel.Framework;
using APD.Client.Framework.Factories;
using System.Collections.ObjectModel;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Plugins;
using APD.Client.Widget.Admin.Factories;
using APD.Client.Widget.Admin.Plugins;

namespace APD.Client.Widget.Admin.Controllers
{
    public class ConfigurationController
    {
        private ConfigurationViewModel viewModel;
        private IRepository<Configuration> repository;
        private IInvokeBackgroundWorker<IEnumerable<Configuration>> backgroundWorker;
        private IInvokeUI uiInvoker;
        private IPersistDomainModels<Configuration> persisterRepository;
        private INotify<EventArgs> saveNotifier;
        private INotify<EventArgs> refreshNotifier;
        private const string VCS_CONFIG_NAME = "vcs";
        private const string CI_CONFIG_NAME = "ci";
        private const string DASHBOARD_CONFIG_NAME = "dashboard";
        private static Dictionary<string, string> prettyPrintMappings = new Dictionary<string, string>();
        private readonly ConfigItemViewModelFactory configViewModelFactory = new ConfigItemViewModelFactory();

        static ConfigurationController()
        {
        }

        public ConfigurationController(ConfigurationViewModel viewModel, 
            IRepository<Configuration> repository,
            IInvokeBackgroundWorker<IEnumerable<Configuration>> backgroundWorker,
            IPersistDomainModels<Configuration> persisterRepository,
            INotify<EventArgs> saveNotifier, 
            INotify<EventArgs> refreshNotifier)
        {
            ValidateInput(viewModel, repository, backgroundWorker, persisterRepository);

            this.viewModel = viewModel;
            this.repository = repository;
            this.backgroundWorker = backgroundWorker;
            this.persisterRepository = persisterRepository;
            this.saveNotifier = saveNotifier;
            this.refreshNotifier = refreshNotifier;
            this.uiInvoker = UIInvokerFactory.Assemlble();

            if (this.saveNotifier != null)
                this.saveNotifier.NewNotification += saveNotifier_NewNotification;

            if (this.refreshNotifier != null)
                this.refreshNotifier.NewNotification += refreshNotifier_NewNotification;

            LoadData();
        }

        void saveNotifier_NewNotification(object sender, EventArgs e)
        {
            var configurations = new List<Configuration>();

            foreach (var configVM in viewModel.Data)
            {
                configurations.Add(CreateConfigDomainModel(configVM));
            }

            backgroundWorker.RunAsyncVoid(() =>
                persisterRepository.Save(configurations));
        }

        void refreshNotifier_NewNotification(object sender, EventArgs e)
        {
            LoadData();
        }

        private Configuration CreateConfigDomainModel(ConfigurationItemViewModel configItemViewModel)
        {
            if (configItemViewModel is GenericConfigItemViewModel)
            {
                var configuration = new Configuration(configItemViewModel.Name);
                var genericConfigVM = configItemViewModel as GenericConfigItemViewModel;

                foreach (KeyValueSettingViewModel setting in genericConfigVM.Data)
                {
                    configuration.NewSetting(setting.Name, setting.Value);
                }

                return configuration;
            }
            else if (configItemViewModel is PluginConfigItemViewModel)
            {
                var pluginConfigVM = configItemViewModel as PluginConfigItemViewModel;
                var configuration = pluginConfigVM.Plugin.Save();
                return configuration;                
            }

            return null;
        }

        private void ValidateInput(ConfigurationViewModel viewModel, 
            IRepository<Configuration> repository, 
            IInvokeBackgroundWorker<IEnumerable<Configuration>> backgroundWorker,
            IPersistDomainModels<Configuration> persisterRepository) 
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            if (repository == null)
                throw new ArgumentNullException("repository");
            if (backgroundWorker == null)
                throw new ArgumentNullException("backgroundWorker");
            if (persisterRepository == null)
                throw new ArgumentNullException("persisterRepository");
        }

        private void LoadData()
        {
            viewModel.IsLoading = true;
            backgroundWorker.RunAsyncVoid(() =>
            {
                var data = repository.Get(new AllSpecification<Configuration>());

                uiInvoker.Invoke(() =>
                   viewModel.IsLoading = false);

                LoadDataIntoViewModel(data);
            });
        }

        private void LoadDataIntoViewModel(IEnumerable<Configuration> data)
        {
            uiInvoker.Invoke(() =>
            {
                viewModel.Data.Clear();

                foreach (var configVM in CreateConfigItemViewModels(data))
                    viewModel.Data.Add(configVM);

                LoadDefaultConfigs();

                SetReadbleNamesOnViewModels();

                ArrangeConfigurations();

                viewModel.DuckTypedData = viewModel.Data;
            });
        }

        private ObservableCollection<ConfigurationItemViewModel> CreateConfigItemViewModels(IEnumerable<Configuration> configurations)
        {
            var retValue = new ObservableCollection<ConfigurationItemViewModel>();

            foreach (var config in configurations)
                retValue.Add(CreateConfigItemViewModel(config));

            return retValue;
        }

        private ConfigurationItemViewModel CreateConfigItemViewModel(Configuration configuration)
        {
            return configViewModelFactory.Assemble(configuration);
        }

        private void LoadDefaultConfigs()
        {
            if (!IsVersionControlSystemConfigLoaded())
                LoadDefaultVersionControlConfig();
            if (!IsContinuousIntegrationConfigLoaded())
                LoadDefaultContinuousIntegrationConfig();
            if (!IsDashboardConfigLoaded())
                LoadDefaultDashboardConfig();
        }

        private bool IsVersionControlSystemConfigLoaded()
        {
            return viewModel.Data.Where(c => c.Name == VCS_CONFIG_NAME).Count() == 1;
        }

        private void LoadDefaultVersionControlConfig()
        {
            var defaultVCSConfig = Configuration.DefaultVCSConfiguration();
            defaultVCSConfig.NewSetting("plugin-class", typeof(VCSConfigPlugin).AssemblyQualifiedName);
            viewModel.Data.Add(CreateConfigItemViewModel(defaultVCSConfig));
        }

        private bool IsContinuousIntegrationConfigLoaded()
        {
            return viewModel.Data.Where(c => c.Name == CI_CONFIG_NAME).Count() == 1;
        }

        private void LoadDefaultContinuousIntegrationConfig()
        {
            var defaultCIConfig = Configuration.DefaultCIConfiguration();
            defaultCIConfig.NewSetting("plugin-class", typeof(CIConfigPlugin).AssemblyQualifiedName);
            viewModel.Data.Add(CreateConfigItemViewModel(defaultCIConfig));
        }

        private bool IsDashboardConfigLoaded()
        {
            return viewModel.Data.Where(c => c.Name == DASHBOARD_CONFIG_NAME).Count() == 1;
        }

        private void LoadDefaultDashboardConfig()
        {
            var config = new Configuration("dashboard");
            config.NewSetting("plugin-class", typeof(DashboardConfigPlugin).AssemblyQualifiedName);
            viewModel.Data.Add(CreateConfigItemViewModel(config));
        }

        private void ArrangeConfigurations()
        {
            var vcsConfig = viewModel.Data.Where(c => c.Name == VCS_CONFIG_NAME).Single();
            var ciConfig = viewModel.Data.Where(c => c.Name == CI_CONFIG_NAME).Single();
            viewModel.Data.Remove(vcsConfig);
            viewModel.Data.Remove(ciConfig);

            viewModel.Data.Insert(0, vcsConfig);
            viewModel.Data.Insert(1, ciConfig);
        }

        private void SetReadbleNamesOnViewModels()
        {
            foreach (var config in viewModel.Data)
                if (config.NamePrettyPrint == null)
                config.NamePrettyPrint = PrettyPrint(config);
        }

        private string PrettyPrint(ConfigurationItemViewModel config)
        {
            if (prettyPrintMappings.ContainsKey(config.Name))
                return prettyPrintMappings[config.Name];
            else
                return config.Name;
        }
    }
}
