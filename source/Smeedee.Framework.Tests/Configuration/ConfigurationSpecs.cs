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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System.Collections;
using System.Collections.Generic;
using Smeedee.Framework.Configuration;
using Smeedee.Framework.Settings;
using Smeedee.Tests;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;


namespace Smeedee.Framework.Tests.Configuration.ConfigurationSpecs
{
    [TestFixture]
    public class when_spawned
    {
        [Test]
        public void should_have_owner()
        {
            PropertyTester.TestForExistence<Framework.Configuration.Configuration>(c => c.ForType);
        }

        [Test]
        public void should_implement_Ienumerable_of_configurationEntry()
        {
            IEnumerable<ConfigurationEntry> ienumerable = new Framework.Configuration.Configuration();
            ienumerable.ShouldNotBeNull();
            ienumerable.GetEnumerator();
            ienumerable.ShouldNotBeNull();
        }

        [Test]
        public void should_implement_Ienumerable()
        {
            IEnumerable ienumerable = new Framework.Configuration.Configuration();
            ienumerable.ShouldNotBeNull();
            ienumerable.GetEnumerator();
            ienumerable.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class when_entry_exists
    {
        private const string ENTRY_NAME = "test entry";
        private const int ENTRY_VALUE = 90001;
        private Framework.Configuration.Configuration config;
        private ConfigurationEntry configEntry;

        [SetUp]
        public void Setup()
        {
            this.config = new Framework.Configuration.Configuration();
            this.configEntry = new ConfigurationEntry()
                          {
                              Name = ENTRY_NAME,
                              Value = ENTRY_VALUE
                          };

            config.Add(configEntry);
        }

        [Test]
        public void added_entry_should_exists()
        {
            bool entryExists = config.EntryExists(ENTRY_NAME);
            entryExists.ShouldBeTrue();
        }

        [Test]
        public void added_entry_should_have_correct_value()
        {
            object value = config.ReadEntryValue(ENTRY_NAME);
            value.ShouldBe(ENTRY_VALUE);
        }
    }


    [TestFixture]
    public class when_entry_doesnt_exists
    {
        private const string ENTRY_NAME_DONT_EXIST = "test entry";
        private Framework.Configuration.Configuration config;

        [SetUp]
        public void Setup()
        {
            this.config = new Framework.Configuration.Configuration();
        }

        [Test]
        public void entry_should_not_exists()
        {
            bool entryExists = config.EntryExists(ENTRY_NAME_DONT_EXIST);
            entryExists.ShouldBeFalse();
        }

        [Test]
        [ExpectedException(typeof(ConfigurationException))]
        public void added_entry_should_throw_exception()
        {
            object value = config.ReadEntryValue(ENTRY_NAME_DONT_EXIST);
        }
    }

}
