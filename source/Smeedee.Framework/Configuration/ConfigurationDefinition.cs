using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smeedee.Framework.Configuration
{
    public class ConfigurationDefinition
    {
        public string ForType { get; set; }
        public List<ConfigurationEntryDefinition> Entries   { get; set; }
    }
}
