using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smeedee.Framework.Configuration
{
    public class DefaultConfigurationDefinitionFinder : IFindConfigurationDefinitions
    {
        public IEnumerable<ConfigurationDefinition> GetDefinitionsFromAssembly(Assembly assembly)
        {
            return new List<ConfigurationDefinition>();
        }

        public ConfigurationDefinition GetDefinitionFromType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
