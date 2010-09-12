using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Smeedee.Tasks.Framework.TaskConfiguration
{
    public class TaskInstanceConfiguration : IEnumerable<TaskConfigurationEntry>
    {
        public string Name { get; set; }
        public string TaskName { get; set; }
        public int DispatchInterval { get; set; }
        //private IList<TaskConfigurationEntry> entries = new List<TaskConfigurationEntry
        public IList<TaskConfigurationEntry> Entries { get; private set; }


        public TaskInstanceConfiguration()
        {
            Entries = new List<TaskConfigurationEntry>();
        }

        public void Add(TaskConfigurationEntry entry)
        {
            Entries.Add(entry);
        }

        public bool EntryExists(string name)
        {
            return Entries.Any(e => e.Name == name);
        }

        public object ReadEntryValue(string name)
        {
            try
            {
                return Entries.Single(e => e.Name == name).Value;
            }
            catch (Exception exception)
            {
                throw new ConfigurationException("A task configuration entry with this name does not exist", exception);
            }
        }

        #region Implementation of IEnumerable

        public IEnumerator<TaskConfigurationEntry> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
