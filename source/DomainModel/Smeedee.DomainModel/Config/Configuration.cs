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

        [DataMember]
        public virtual Guid Id { get; set; }

        [DataMember]
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
            Name = "";
            settings = new List<SettingsEntry>();
            Id = Guid.NewGuid();
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

            NewSetting(new SettingsEntry(name, values));
        }

        public virtual void NewSetting(SettingsEntry entry)
        {
            if (entry == null)
                throw new ArgumentException("Cannot add null entry");

            var existing = settings.Where(e => e.Name == entry.Name).FirstOrDefault();
            if (existing == null)
            {
                settings.Add(entry);
                entry.Configuration = this;
            } else
            {
                existing.Vals = entry.Vals;
            }
        }

        public virtual SettingsEntry GetSetting(string settingName)
        {
            var entry = settings.Where(e => e.Name == settingName).SingleOrDefault();
            if (entry != null)
                return entry;
            else
                return new EntryNotFound();
        }

        public virtual bool ContainsSetting(string settingName)
        {
            return settings.Where(e => e.Name == settingName).Count() > 0;
        }

        public virtual void ChangeSetting(string name, params string[] newValues)
        {
            GetSetting(name).Vals = newValues;
        }

        public virtual Configuration Clone()
        {
            var copy = new Configuration();
            copy.Name = Name;
            copy.IsConfigured = IsConfigured;
            copy.Id = Id;

            foreach (var setting in Settings)
            {
                var settingCopy = new SettingsEntry();
                settingCopy.Name = setting.Name;

                var vals = new List<string>();
                foreach (var val in setting.Vals)
                    vals.Add(val);

                settingCopy.Vals = vals;

                copy.settings.Add(settingCopy);
            }

            return copy;
        }
    }
}
