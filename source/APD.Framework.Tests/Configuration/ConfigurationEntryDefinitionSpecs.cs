#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using APD.Framework.Configuration;
using APD.Tests;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace APD.Framework.Tests.Configuration.ConfigurationEntryDefinitionSpecs
{
    public class Shared
    {
        protected ConfigurationEntryDefinition configDef;
    }

    [TestFixture]
    public class when_spawned : Shared
    {
        [Test]
        public void should_have_type()
        {
            PropertyTester.TestForExistence<ConfigurationEntryDefinition>(ced => ced.Type);
        }

        [Test]
        public void should_have_name()
        {
            PropertyTester.TestForExistence<ConfigurationEntryDefinition>(ced => ced.Name);
        }

        [Test]
        public void should_have_defaultValue()
        {
            PropertyTester.TestForExistence<ConfigurationEntryDefinition>(ced => ced.DefaultValue);
        }

        [Test]
        public void should_have_default_constructor()
        {
            configDef = new ConfigurationEntryDefinition();
            Assert.IsNotNull(configDef);
        }
    }

    [TestFixture]
    public class when_defined_type_doesnt_match_default_value
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void should_throw_argumentexception()
        {
            ConfigurationEntryDefinition definition = 
                new ConfigurationEntryDefinition("int is not string", typeof(string), 1000);
        }
    }

    [TestFixture]
    public class when_setting_default_value_to_null
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void should_throw_argumentexception()
        {
            ConfigurationEntryDefinition definition =
                new ConfigurationEntryDefinition("null is not valid", typeof(string), null);
        }
    }

    [TestFixture]
    public class when_validating_configuration_values
    {
        [Test]
        public void should_return_false_when_not_matching()
        {
            ConfigurationEntryDefinition definition =
                new ConfigurationEntryDefinition("string config entry", typeof(string), "default");

            bool isValidValue = definition.IsValidConfigurationValue(true);
            isValidValue.ShouldBeFalse();
        }

        [Test]
        public void should_return_false_when_value_is_null()
        {
            ConfigurationEntryDefinition definition =
                new ConfigurationEntryDefinition("string config entry", typeof(string), "default");

            bool isValidValue = definition.IsValidConfigurationValue(null);
            isValidValue.ShouldBeFalse();
        }

        [Test]
        public void should_return_true_when_matching()
        {
            ConfigurationEntryDefinition definition =
                new ConfigurationEntryDefinition("string config entry", typeof(string), "default");

            bool isValidValue = definition.IsValidConfigurationValue("my own");
            isValidValue.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class when_validating_configurationEntries
    {
        private ConfigurationEntryDefinition stringDefinition;
        private const string CONFIG_NAME = "string config entry";
        private const string DIFFERENT_CONFIG_NAME = "sumthin else";
        private const string STRING_VALUE = "hello";
        private const bool BOOL_VALUE = false;

        [SetUp]
        public void Setup()
        {
            this.stringDefinition = 
                new ConfigurationEntryDefinition(CONFIG_NAME, typeof(string), STRING_VALUE);
        }

        [Test]
        public void should_return_false_when_name_not_matching()
        {
            ConfigurationEntry entry = new ConfigurationEntry()
                                       {
                                           Name = DIFFERENT_CONFIG_NAME,
                                           Value = STRING_VALUE
                                       };

            bool isValidEntry = stringDefinition.isValidEntry(entry);
            isValidEntry.ShouldBeFalse();
        }

        [Test]
        public void should_return_false_when_value_type_not_matching()
        {

            ConfigurationEntry entry = new ConfigurationEntry()
            {
                Name = CONFIG_NAME,
                Value = BOOL_VALUE
            };

            bool isValidEntry = stringDefinition.isValidEntry(entry);
            isValidEntry.ShouldBeFalse();
        }

        [Test]
        public void should_return_true_when_name_and_value_type_match()
        {
            ConfigurationEntry entry = new ConfigurationEntry()
            {
                Name = CONFIG_NAME,
                Value = "any string here"
            };

            bool isValidEntry = stringDefinition.isValidEntry(entry);
            isValidEntry.ShouldBeTrue();
        }
    }

}
