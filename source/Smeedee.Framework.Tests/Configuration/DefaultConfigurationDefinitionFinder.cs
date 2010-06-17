using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Smeedee.Framework.Configuration;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Framework.Tests.Configuration.DefaultConfigurationDefinitionFinderSpecs
{
    [DefineConfigurationEntry("username", typeof(string), "username")]
    [DefineConfigurationEntry("password", typeof(string), "password")]
    [DefineConfigurationEntry("refreshrate", typeof(int), 10)]
    public class TestClassThatDefineSomeConfigurationEntries
    {
        
    }

    [TestFixture]
    public class when_finding_configs_in_assembly
    {
        [Test]
        public void should_return_IEnumerable_of_ConfigurationDefinitions()
        {
            var finder = new DefaultConfigurationDefinitionFinder();
            var list = finder.GetDefinitionsFromAssembly(Assembly.GetExecutingAssembly());

            list.ShouldBeInstanceOfType<IEnumerable<ConfigurationDefinition>>();
        }

        [Test]
        public void should_get_all_the_defined_configurations()
        {

        }
    }
}
