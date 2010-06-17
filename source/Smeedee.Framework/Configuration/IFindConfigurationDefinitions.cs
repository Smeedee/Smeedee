using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Smeedee.Framework.Configuration
{
    interface IFindConfigurationDefinitions
    {
        IEnumerable<ConfigurationDefinition> GetDefinitionsFromAssembly(Assembly assembly);
        ConfigurationDefinition GetDefinitionFromType(Type type);
    }
}
