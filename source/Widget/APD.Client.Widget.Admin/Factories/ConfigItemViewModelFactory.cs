using System;
using System.Collections.ObjectModel;
using System.Linq;
using APD.Client.Widget.Admin.ViewModels.Configuration;
using APD.Client.Framework.Factories;
using APD.Client.Framework.Plugins;
using APD.DomainModel.Config;


namespace APD.Client.Widget.Admin.Factories
{
    public class ConfigItemViewModelFactory
    {
        private const string PLUGIN_SETTING_NAME = "plugin-class";
        private const string IS_EXPANDED_SETTING = "is-expanded";

        private Configuration configurationToBeAssembled;
        private ConfigurationItemViewModel configurationItem;

        public ConfigurationItemViewModel Assemble(Configuration configuration)
        {
            configurationItem = null;

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            configurationToBeAssembled = configuration;

            AssembleConfiguration();

            return configurationItem;
        }

        private void AssembleConfiguration()
        {
            if (IsPluginConfiguration())
                configurationItem = AssemblePluginConfiguration(configurationToBeAssembled);
            else
                configurationItem = AssembleGenericConfiguration(configurationToBeAssembled);

            configurationItem.IsExpanded = ParseIsExpandedFromConfiguration(configurationToBeAssembled);
        }

        private bool IsPluginConfiguration()
        {
            return configurationToBeAssembled.Settings.Any(s => s.Name == PLUGIN_SETTING_NAME);
        }

        private ConfigurationItemViewModel AssemblePluginConfiguration(Configuration configuration)
        {
            var pluginConfig = new PluginConfigItemViewModel(UIInvokerFactory.Assemlble())
            {
                Name = configuration.Name,
            };

            try
            {
                pluginConfig.Plugin = Activator.CreateInstance(
                                          Type.GetType(configuration.GetSetting(PLUGIN_SETTING_NAME).Value))
                                      as IConfigurationPlugin;
                pluginConfig.Plugin.Load(configuration);
                pluginConfig.NamePrettyPrint = pluginConfig.Plugin.Name;
            }
            catch (Exception ex)
            {
                pluginConfig.ErrorMessage = ex.ToString();
            }

            return pluginConfig;
        }

        private ConfigurationItemViewModel AssembleGenericConfiguration(Configuration configuration)
        {
            var genericConfig = new GenericConfigItemViewModel(UIInvokerFactory.Assemlble())
            {
                Name = configuration.Name,
                Data = CreateSettingViewModels(configuration)
            };

            return genericConfig;
        }

        private ObservableCollection<SettingViewModel> CreateSettingViewModels(Configuration configuration)
        {
            var data = new ObservableCollection<SettingViewModel>();

            var settingItems = from setting in configuration.Settings
                               select new KeyValueSettingViewModel(UIInvokerFactory.Assemlble())
                               {
                                   Name = setting.Name,
                                   Value = setting.Value
                               };

            foreach (var setting in settingItems)
                data.Add(setting);

            return data;
        }

        private bool ParseIsExpandedFromConfiguration(Configuration configuration)
        {
            if (configuration.ContainSetting(IS_EXPANDED_SETTING))
            {
                var isExpanded = false;
                bool.TryParse(configuration.GetSetting(IS_EXPANDED_SETTING).Value,
                              out isExpanded);

                return isExpanded;
            }

            return false;
        }
    }
}
