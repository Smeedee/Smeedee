using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Smeedee.DomainModel.TaskInstanceConfiguration
{
    [DataContract]
    public class TaskConfiguration
    {
        private const int DEFAULT_INTERVAL = 600;
        private List<TaskConfigurationEntry> _settings;

        public TaskConfiguration()
        {
            DispatchInterval = DEFAULT_INTERVAL;
            _settings = new List<TaskConfigurationEntry>();
        }

        [DataMember]
        public virtual string Name { get; set; }

        [DataMember]
        public virtual string TaskName { get; set; }

        [DataMember]
        public virtual int DispatchInterval { get; set; }

        [DataMember]
        public virtual IList<TaskConfigurationEntry> Entries
        {
            get { return _settings; }
            set
            {
                _settings = value.ToList();
                foreach (var setting in value)
                {
                    if (setting == null) continue;
                    setting.TaskConfiguration = this;
                }
            }
        }

        public virtual bool EntryExists(string name)
        {
            return Entries.Any(e => e.Name == name);
        }

        public virtual object ReadEntryValue(string name)
        {
            try
            {
                return Entries.Single(e => e != null && e.Name == name).Value;
            }
            catch (Exception exception)
            {
                throw new TaskConfigurationException("A task configuration entry with this name does not exist");
            }
        }
    }
}