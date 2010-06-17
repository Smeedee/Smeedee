using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Smeedee.DomainModel.Config
{
    [DataContract(IsReference = true)]
    public class Configuration
    {
        [DataMember]
        public virtual string Name { get; set; }

        public virtual bool IsConfigured { get; set; }

        private List<SettingsEntry> settings;

        [DataMember]
        public virtual IEnumerable<SettingsEntry> Settings
        {
            get { return settings; }
            set
            {
                settings = value.ToList();
                foreach (SettingsEntry setting in value)
                {
                    setting.Configuration = this;
                }
            }
        }

        public Configuration()
        {
            settings = new List<SettingsEntry>();
        }

        public Configuration(string name) : this()
        {
            if (name == null)
                throw new ArgumentException("Name must be specified");

            this.Name = name;
        }

        public virtual void NewSetting(string name, params string[] values)
        {
            if (name == null)
                throw new ArgumentException("Name must be specified");

            if (values == null)
                throw new ArgumentException("Value must be specified");

            if (settings.Where(e => e.Name == name).Count() == 0)
                settings.Add(new SettingsEntry(name, null));

            var entry = settings.Where(e => e.Name == name).Single();

            entry.Vals = values;
            entry.Configuration = this;
        }

        public virtual SettingsEntry GetSetting(string settingName)
        {
            var entry = settings.Where(e => e.Name == settingName).SingleOrDefault();
            if (entry != null)
                return entry;
            else
                return new EntryNotFound();
        }

        public virtual bool ContainSetting(string settingName)
        {
            return settings.Where(e => e.Name == settingName).Count() > 0;
        }

        public static Configuration ProviderConfiguration(string configName, string username, string password, string url, string provider,
            string project,
            params string[] supportedProviders)
        {
            var configuration = new Configuration(configName);
            configuration.NewSetting("supported-providers", supportedProviders);
            configuration.NewSetting("provider", provider);
            configuration.NewSetting("username", username);
            configuration.NewSetting("password", password);
            configuration.NewSetting("url", url);
            configuration.NewSetting("is-expanded", "true");
            configuration.NewSetting("project", project);
            return configuration;
        }

        public static Configuration DefaultVCSConfiguration()
        {
            return ProviderConfiguration("vcs", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "svn", "tfs");
        }


        public static Configuration DefaultCIConfiguration()
        {
            return ProviderConfiguration("ci", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "cc.net",
                                         "tfs - vanilla", "tfs - scrum by conchango");
        }

        public static Configuration DefaultDashboardConfiguration()
        {
            var dashboardConfig = new Configuration("dashboard");
            dashboardConfig.NewSetting("slide-widgets", "latest-commits", "ci", "commit-heroes", "commit-stats", "working-days-left");
            dashboardConfig.NewSetting("selected-slide-widgets", "latest-commits", "ci", "commit-heroes", "commit-stats", "working-days-left");
            dashboardConfig.NewSetting("tray-widgets", "working-days-left", "ci");
            dashboardConfig.NewSetting("selected-tray-widgets", "working-days-left", "ci");
            dashboardConfig.NewSetting("is-expanded", "false");

            return dashboardConfig;
        }

        public static Configuration DefaultHolidayConfiguration()
        {
            var holidayConfiguration = new Configuration("holidays");
            holidayConfiguration.NewSetting("saturdays-are-holidays", "true");
            holidayConfiguration.NewSetting("sundays-are-holidays", "true");

            return holidayConfiguration;
        }
    }
}
