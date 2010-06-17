using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Framework.Configuration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DefineConfigurationEntryAttribute : Attribute
    {
        public ConfigurationEntryDefinition ConfigurationEntryDefinition { get; private set; }

        public DefineConfigurationEntryAttribute(string name, Type valueType, object defaultValue)
        {
            ConfigurationEntryDefinition = new ConfigurationEntryDefinition(name, valueType, defaultValue);
        }
    }
}
